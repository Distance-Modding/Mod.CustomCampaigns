using HarmonyLib;

namespace Mod.CustomCampaigns.Patches
{
    [HarmonyPatch(typeof(LevelIntroTitleLogic), nameof(LevelIntroTitleLogic.Start))]
    internal static class LevelIntroTitleLogic__Start
    {
        [HarmonyPostfix]
        internal static void WrapText(LevelIntroTitleLogic __instance)
        {
            //UNSURE HOW TO DO THIS FOR NOW
            //I NEED TO FIGURE OUT HOW TO GET MY HANDS ON THE TEXT TO SEE HOW IT DOES THE THING
            //__instance.titleLabel_.overflowMethod = UILabel.Overflow.ResizeFreely;
        }
    }
}
