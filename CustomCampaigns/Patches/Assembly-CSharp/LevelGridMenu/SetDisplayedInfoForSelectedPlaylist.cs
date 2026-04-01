using HarmonyLib;

namespace Mod.CustomCampaigns.Patches
{
    [HarmonyPatch(typeof(LevelGridMenu), nameof(LevelGridMenu.SetDisplayedInfoForSelectedPlaylist))]
    internal static class SetDisplayedInfoForSelectedPlaylist
    {
        //Does what it says on the tin. Disables the description text if something is locked
        [HarmonyPostfix]
        internal static void DisableGridDescriptionForAddCampaign(LevelGridMenu __instance)
        {
            bool adventureFlag = __instance.displayType_ == LevelSelectMenuAbstract.DisplayType.Adventure;

            if (adventureFlag)
            {
                if (__instance.displayedEntry_.labelText_ != "Adventure" &&
                __instance.displayedEntry_.labelText_ != "Lost To Echoes" &&
                __instance.displayedEntry_.labelText_ != "Nexus" &&
                __instance.displayedEntry_.labelText_ != "The Other Side")
                {
                    __instance.gridDescription_.text = string.Empty;
                    if (__instance.gridDescription_.gameObject.activeSelf)
                        __instance.gridDescription_.gameObject.SetActive(false);
                }
            }
        }
    }
}
