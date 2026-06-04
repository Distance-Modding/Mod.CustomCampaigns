using HarmonyLib;

namespace Mod.CustomCampaigns.Patches
{
    [HarmonyPatch(typeof(AdventureMode), nameof(AdventureMode.OnEventCarInstantiate))]
    internal static class AdventureMode__OnEventCarInstantiate
    {
        [HarmonyPostfix]
        internal static void RemoveResetHint(AdventureMode __instance, Events.Player.CarInstantiate.Data data)
        {
            LocalPlayerControlledCar localCar = data.car.GetComponent<LocalPlayerControlledCar>();
            if (!localCar.ExistsAndIsEnabled() || !__instance.gameMan_.IsCampaignModeNormal_)
                return;

            if (__instance is NexusMode)
            {
                if (__instance.gameMan_.LevelName_ != "Mobilization" &&
                __instance.gameMan_.LevelName_ != "Resonance" &&
                __instance.gameMan_.LevelName_ != "Deterrance" &&
                __instance.gameMan_.LevelName_ != "Terminus" &&
                __instance.gameMan_.LevelName_ != "Collapse")
                {
                    localCar.showBackToResetWarning_ = false;
                }
            }
        }
    }
}
