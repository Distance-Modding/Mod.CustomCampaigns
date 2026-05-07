using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;

namespace Mod.CustomCampaigns
{
    /*
     * To Add:
     * Make it remember progress through a campaign - DUBIOUS???
     * Give them custom descriptions based on their Steam Collection description
     * Make a function in plugin that searches for a playlist name on Steam and tries to fetch the collections description. Saves into Dictionary<CampaignName, Description>
     * Save information to the player's profile???
     * This should occur right before the first loading screen
     * 
     * 
     */

    [BepInPlugin(modGUID, modName, modVersion)]
    public sealed class Mod : BaseUnityPlugin
    {
        //Mod Details
        private const string modGUID = "Distance.CustomCampaigns";
        private const string modName = "Custom Campaigns";
        private const string modVersion = "1.1.0";

        //Config Entries

        //Public Variables

        //Private Variables

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
    }
}
