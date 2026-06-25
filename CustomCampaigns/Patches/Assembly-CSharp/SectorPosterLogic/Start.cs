using HarmonyLib;

namespace Mod.CustomCampaigns.Patches
{
    [HarmonyPatch(typeof(SectorPosterLogic), nameof(SectorPosterLogic.Start))]
    internal static class SectorPosterLogic__Start
    {
        //This should prevent posters from displaying the wrong thing during the campaign
        [HarmonyPrefix]
        internal static bool StartOverride(SectorPosterLogic __instance)
        {
            GameManager gameManager = G.Sys.GameManager_;
            if (gameManager.playlist_.FirstModeID_ == GameModeID.Nexus || gameManager.playlist_.FirstModeID_ == GameModeID.LostToEchoes)
                return false;
            else
                return true;
        }
    }
}
