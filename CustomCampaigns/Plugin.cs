using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Mod.CustomCampaigns
{
    /*
     * Added:
     * 
     * To Add:
     * Make it remember progress through a campaign - DUBIOUS???
     * Give them custom descriptions based on their Steam Collection description
     * Give a campaign a custom icon based off of the workshop collection image??? (How do I get the image?)
     * A toggle for whether a campaign will use level thumbnails as transitions
     * Make it so a level thumbnail isn't used when backing out to the main menu
     * Create a means to change whether a campaign uses the Nexus campaign as a base or LtE as a base
     * Implement a word wrapping of some kind for the titles so long titles can appear without getting their words sliced into a new line
     * Save information to the player's profile???
     * 
     */

    [BepInPlugin(modGUID, modName, modVersion)]
    public sealed class Mod : BaseUnityPlugin
    {
        //Mod Details
        private const string modGUID = "Distance.CustomCampaigns";
        private const string modName = "Custom Campaigns";
        private const string modVersion = "2.0.1";

        //Config Entries

        //Public Variables

        //Private Variables
        Dictionary<ulong, bool> collectionIDs = new Dictionary<ulong, bool>();
        string campaignTitle = string.Empty;
        string campaignDesc = string.Empty;

        //Other
        private static readonly Harmony harmony = new Harmony(modGUID);
        public static ManualLogSource Log = new ManualLogSource(modName);
        public static Mod Instance;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            
            Log = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            Logger.LogInfo("Thanks for using Custom Campaigns!");

            //Config Setup

            //Apply Patches
            Logger.LogInfo("Loading...");
            harmony.PatchAll();
            Logger.LogInfo("Loaded!");
        }

        private void OnConfigChanged(object sender, EventArgs e)
        {
            SettingChangedEventArgs settingChangedEventArgs = e as SettingChangedEventArgs;

            if (settingChangedEventArgs == null) return;
        }

        public bool ValidateUrl(string url, out string errorMessage)
        {
            if (url.StartsWith("https://steamcommunity.com/sharedfiles/filedetails/?id="))
            {
                Regex ConfirmWorkshopItem = new Regex(@"^(https:\/\/steamcommunity\.com\/sharedfiles\/filedetails\/\?id=)\d*$");
                if (ConfirmWorkshopItem.IsMatch(url))
                {
                    errorMessage = "THERE WAS NO ERROR YIPPEE!!!";
                    return true;
                }
                else
                {
                    errorMessage = "THIS STEAM LINK IS NOT A WORKSHOP ITEM";
                    return false;
                }
            }
            else if (url.StartsWith("https:"))
            {
                errorMessage = "THIS LINK IS NOT A VALID LINK";
                return false;
            }
            else if (url == "")
            {
                errorMessage = "YOU DIDN'T SUBMIT ANYTHING";
                return false;
            }
            errorMessage = "CANNOT PARSE. PLEASE TRY AGAIN";
            return false;
        }

        public IEnumerator ParseCollection(string url)
        {
            //Needed for feedback purposes
            MenuPanelManager MPM = G.Sys.MenuPanelManager_;

            //Setting up UGC and starting the progress text loading screen
            SteamworksUGC UGC = G.Sys.SteamworksManager_.UGC_;
            UGC.SetupSteamProgressText("Downloading Campaign...");
            UGC.progressBar_.value = 0f / 2f;

            //Setting up the web request
            UnityEngine.Networking.UnityWebRequest request = new UnityEngine.Networking.UnityWebRequest(url);
            request.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();
            request.method = UnityEngine.Networking.UnityWebRequest.kHttpVerbGET;

            yield return request.Send();

            

            if (request.isError)
            {
                Log.LogError("Failed to request the link!");
                string error = request.error;
                if (error == null)
                {
                    error = "Unknown Error!";
                }

                Log.LogError(error);
                MPM.ShowError("Failed to connect to the web page");
                MPM.MenuInputEnabled_ = true;
                UGC.DestroySteamProgressText();
                yield break;
            }

            UGC.ProgressTextLabel_.text = "Parsing the web page...";
            UGC.progressBar_.value = 1f / 2f;
            collectionIDs = new Dictionary<ulong, bool>();
            campaignTitle = string.Empty;
            campaignDesc = string.Empty;


            try
            {
                //Log.LogInfo("This is definitely a web page I'm looking at! :3");
                string requestHTML = request.downloadHandler.text;
                //Log.LogInfo(requestHTML);
                if (requestHTML.Contains("<span class=\"breadcrumb_separator\">&gt;&nbsp;</span><a data-panel=\"{&quot;noFocusRing&quot;:true}\" href=\"https://steamcommunity.com/workshop/browse/?section=collections&appid=233610\">Collections</a>"))
                {
                    //Log.LogInfo("This confirms that this is a Distance workshop collection!");
                    Regex MatchAllIDs = new Regex(@"(?<={""id"":"")\d*");
                    if (MatchAllIDs.IsMatch(requestHTML))
                    {
                        MatchCollection matchIDs = MatchAllIDs.Matches(requestHTML);
                        foreach(Match mID in matchIDs)
                        {
                            Log.LogInfo("ID Found: " + mID.Value);
                            WorkshopLevelInfo wInfo;
                            bool levelInstalled = false;
                            try
                            {
                                levelInstalled = UGC.storedPublishedFileIDs_.TryGetWorkshopLevelInfo((ulong)Convert.ToInt64(mID.Value), out wInfo);
                                if (!string.IsNullOrEmpty(wInfo.title_))
                                {
                                    Log.LogInfo("Level Already Installed: " + wInfo.title_);
                                }
                            }
                            catch (Exception aE)
                            {
                                Log.LogWarning("Level is not already installed: Will attempt to download");
                                //Log.LogWarning(aE);
                            }

                            
                            collectionIDs.Add((ulong)Convert.ToInt64(mID.Value), levelInstalled);
                        }
                    }
                    else
                    {
                        Log.LogWarning("Failed to find any level IDs in the collection");
                    }

                    //Here is where I would attempt to download an image but uuuh...not sure how I would honestly moving on!
                    Regex matchTitle = new Regex(@"(?<=<div class=""workshopItemTitle"">).*(?=</div>)");
                    if (matchTitle.IsMatch(requestHTML))
                    {
                        Match mTitle = matchTitle.Match(requestHTML);
                        //Log.LogInfo("Title: " + mTitle.Value);
                        campaignTitle = mTitle.Value;
                    }

                    Regex matchDescription = new Regex(@"(?<=<div class=""workshopItemDescription"" id=""highlightContent"">).*(?=</div>)");
                    if (matchDescription.IsMatch(requestHTML))
                    {
                        Match mDesc = matchDescription.Match(requestHTML);
                        string betterDesc = mDesc.Value.Replace("<br>", "\n");
                        //Log.LogInfo("Description: " + betterDesc);
                        campaignDesc = betterDesc;
                    }
                    else
                    {
                        Log.LogWarning("Failed to find the description in the collection");
                    }

                    //With that done it should start trying to build the playlist from this collection. Not sure if possible but will try!!!
                    //Use the TryGetWorkshopLevel thing from WorkshopLevelInfos i think

                }
                else
                {
                    MPM.ShowError("This Workshop link is not a Distance Workshop Collection link! \nPlease submit a link that is a Distance Workshop Collection");
                    MPM.MenuInputEnabled_ = true;
                    UGC.DestroySteamProgressText();
                    yield break;
                }

                if (collectionIDs.Count == 0)
                {
                    MPM.ShowError("This collection has no levels!");
                    MPM.MenuInputEnabled_ = true;
                    UGC.DestroySteamProgressText();
                    yield break;
                }
            }
            catch (Exception e)
            {
                Log.LogError("Encountered an error when parsing the link!");
                string error = e.ToString();
                if (error == null)
                {
                    error = "Unknown Error!";
                }

                Log.LogError(error);
                MPM.ShowError("Failed to parse the web page");
                MPM.MenuInputEnabled_ = true;
                UGC.DestroySteamProgressText();
                yield break;
            }

            //You won't get here unless collectionIDs has something in it
            List<Steamworks.PublishedFileId_t> pFileIds = new List<Steamworks.PublishedFileId_t>();
            foreach(KeyValuePair<ulong, bool> kvp in collectionIDs)
            {
                if(!kvp.Value)
                {
                    Log.LogInfo("Level ID to Download: " + kvp.Key);
                    pFileIds.Add(new Steamworks.PublishedFileId_t(kvp.Key));
                }
            }

            if (pFileIds.Count > 0)
            {
                Log.LogInfo("Beginning download process...");
                UGC.StartWorkshopLevelsUpdate(WorkshopUpdateType.Individual, pFileIds.ToArray(), new MenuPanel.OnPanelPop(ConvertLevelsToCampaign),
                    new MenuPanel.OnPanelPop(() =>
                    {
                        MPM.ShowError("Failed to download workshop levels from the collection");
                    }));
                
            }
            else
            {
                ConvertLevelsToCampaign();
            }

            MPM.MenuInputEnabled_ = true;
            UGC.DestroySteamProgressText();
            yield break;
        }

        //I need to write a function here that will happen on update finished and when that happens I'll be the cream of the crop.
        //Basically that function will be the thing that makes the playlist file exist I hope
        //That collectionIDs thing is going to have to be a global dictionary so the function can use it.
        //If I can't make that playlist ima cry!
        private void ConvertLevelsToCampaign()
        {
            MenuPanelManager MPM = G.Sys.MenuPanelManager_;
            SteamworksUGC UGC = G.Sys.SteamworksManager_.UGC_;
            List<LevelNameAndPathPair> campaignLevels = new List<LevelNameAndPathPair>();

            Log.LogInfo("Preparing to create campaign playlist...");
            foreach (KeyValuePair<ulong, bool> kvp in collectionIDs)
            {
                WorkshopLevelInfo wInfo;
                UGC.storedPublishedFileIDs_.TryGetWorkshopLevelInfo(kvp.Key, out wInfo);
                Log.LogInfo("Level Title: " + wInfo.title_ + " Path: " + Resource.GetAbsoluteLevelPath(wInfo.relativePath_));
                campaignLevels.Add(new LevelNameAndPathPair(wInfo.title_, Resource.GetAbsoluteLevelPath(wInfo.relativePath_)));
            }

            Log.LogInfo("Creating Playlist...");
            LevelSet campaignLevelSet = new LevelSet();
            campaignLevelSet.resourcesLevelNameAndPathPairsInSet_ = campaignLevels;

            LevelPlaylist campaignPlaylist = LevelPlaylist.Create(campaignLevelSet, campaignTitle, GameModeID.Nexus);
            campaignPlaylist.Awake();
            campaignPlaylist.Save();
            MPM.ShowError("Return to the main menu to refresh the campaign list", "Campaign Installed");
        }
    }
}
