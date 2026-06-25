using HarmonyLib;
using Steamworks;
using System.Collections.Generic;
using System.Linq;

namespace Mod.CustomCampaigns.Patches
{
    [HarmonyPatch(typeof(LevelGridMenu), nameof(LevelGridMenu.CreateEntries))]
    internal static class LevelGridMenu__CreateEntries
    {
       [HarmonyPostfix]
       internal static void CustomPlaylist(LevelGridMenu __instance)
        {
            bool adventureFlag = __instance.displayType_ == LevelSelectMenuAbstract.DisplayType.Adventure;
            bool gameModeFlag = __instance.modeID_ == GameModeID.Sprint;

            if (adventureFlag)
            {
                //Mod.Log.LogInfo("Total Count: " + __instance.buttonList_.ScrollableEntryCount_);

                foreach(LevelPlaylist playlist in GetLocalPlaylists())
                {

                    __instance.CreateAndAddEntry(playlist, LevelGridMenu.PlaylistEntry.Type.Campaign, true);
                }

                string[] legacyFileNames = G.Sys.LevelSets_.LegacyFileNames_;
                List<LevelNameAndPathPair> nameAndPathPairs = G.Sys.LevelSets_.GetSet(GameModeID.Sprint).GetAllLevelNameAndPathPairs();
                LevelSet set = new LevelSet();

                for (int index = 0; index < nameAndPathPairs.Count; ++index)
                {
                    string withoutExtension = Resource.GetFileNameWithoutExtension(nameAndPathPairs[index].levelPath_);
                    if (((IEnumerable<string>)legacyFileNames).Contains(withoutExtension))
                    {
                        set.AddLevel(nameAndPathPairs[index].levelName_, Resource.GetAbsoluteOfficialLevelPath(withoutExtension), LevelType.Official);
                        break;
                    }
                }

                if (set.ResourcesLevelsCount_ > 0)
                {
                    __instance.AddEntry(new AddCampaignEntry(__instance, LevelPlaylist.Create(set, "Add Campaign", GameModeID.Sprint)));
                }

                //Mod.Log.LogInfo("New Total Count: " + __instance.buttonList_.ScrollableEntryCount_);

                __instance.buttonList_.SortAndUpdateVisibleButtons();
            }
        }

        private static List<LevelPlaylist> GetLocalPlaylists()
        {
            List<LevelPlaylist> levelPlaylists = new List<LevelPlaylist>();

            if (DirectoryEx.Exists(Resource.PersonalLevelPlaylistsDirPath_))
            {
                List<string> localPlaylistFilePaths = Resource.GetFilePathsInDirWithPattern(Resource.PersonalLevelPlaylistsDirPath_, "*.xml");
                localPlaylistFilePaths.RemoveAll(s => !Resource.FileExist(s));

                foreach (string playlistPath in localPlaylistFilePaths)
                {
                    LevelPlaylist playlist = LevelGridMenu.LoadPlaylist(playlistPath);
                    //Mod.Log.LogInfo("Playlist Name: " + playlist.Name_);
                    if (!(bool)(UnityEngine.Object)playlist)
                    {
                        Mod.Log.LogError("Failed to load: " + playlistPath);
                    }
                    if (playlist.Count_ == 0)
                    {
                        continue;
                    }
                    //Mod.Log.LogInfo("    Playlist Mode: " + playlist.FirstModeID_);
                    if (playlist.FirstModeID_ == GameModeID.LostToEchoes || playlist.FirstModeID_ == GameModeID.Nexus)
                    {
                        levelPlaylists.Add(playlist);
                    }
                }
            }
            //Mod.Log.LogInfo("CAMPAIGN PLAYLIST SIZE = " + levelPlaylists.Count);
            return levelPlaylists;
        }

        private class AddCampaignEntry : LevelGridMenu.PlaylistEntry
        {
            //This will always be locked. The point of this is overriding On Click.
            public AddCampaignEntry(LevelGridMenu menu, LevelPlaylist playlist) : base(menu, "Add Campaign", playlist, Type.Official, false, UnlockStyle.None, true)
            {
                //Gee I hope this works
            }

            public override void OnClick()
            {
                InputPromptPanel.Create(new InputPromptPanel.OnSubmit(SearchCollectionOnSteam), new InputPromptPanel.OnPop(InputPop), "Enter Campaign Link:");
                //Mod.Log.LogInfo("I clicked the Campaign Button :D   AND SOMETHING HAPPENED");
                //https://steamcommunity.com/sharedfiles/filedetails/?id=1933353754 This is the Dakka Map Pack
                /*UGCQueryHandle_t CollectionQuery = SteamUGC.CreateQueryUGCDetailsRequest(new PublishedFileId_t[1]
                                                    {
                                                        new PublishedFileId_t((ulong)1933353754)
                                                    }, (uint)5);

                SteamAPICall_t CollectionSearch = SteamUGC.SendQueryUGCRequest(CollectionQuery);*/

                
                
            }

            private bool SearchCollectionOnSteam(out string errorMessage, string input)
            {
                //Mod.Log.LogInfo("This is where the link gets searched for on Steam");

                if (Mod.Instance.ValidateUrl(input, out errorMessage))
                {
                    Mod.Instance.StartCoroutine(Mod.Instance.ParseCollection(input));
                    return true;
                }
                else
                {
                    return false;
                }
            }

            private void InputPop()
            {
                //Pop is when the menu closes
                //Mod.Log.LogInfo("Pop happens at this time!");
            }
        }
    }
}
