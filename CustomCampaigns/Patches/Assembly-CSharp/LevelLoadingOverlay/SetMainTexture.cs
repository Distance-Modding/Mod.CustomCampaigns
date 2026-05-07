using HarmonyLib;
using Steamworks;

namespace Mod.CustomCampaigns.Patches
{
    [HarmonyPatch(typeof(LevelLoadingOverlay), nameof(LevelLoadingOverlay.SetMainTexture))]
    internal static class SetMainTexture
    {
        [HarmonyPrefix]
        internal static void ChangeLoadingTexture(LevelLoadingOverlay __instance)
        {
            GameModeID modeID = __instance.gameManager_.NextGameModeID_;
            string levelName = __instance.gameManager_.NextLevelName_;
            string levelPath = __instance.gameManager_.NextLevelPath_;

            if (modeID == GameModeID.Nexus && 
                levelName != "Mobilization" &&
                levelName != "Resonance" &&
                levelName != "Deterrance" &&
                levelName != "Terminus" &&
                levelName != "Collapse")
            {
                __instance.loadingTexture_.mainTexture = Resource.LoadLevelPreviewTexture(levelPath, G.Sys.LevelSets_.GetLevelType(levelPath));
            }
        }
    }
}
