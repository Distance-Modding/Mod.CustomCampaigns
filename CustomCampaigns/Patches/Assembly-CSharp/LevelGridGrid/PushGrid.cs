using HarmonyLib;
using Mod.CustomCampaigns.Extensions;
using Mod.CustomCampaigns.Scripts;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Mod.CustomCampaigns.Patches
{
    [HarmonyPatch(typeof(LevelGridGrid), nameof(LevelGridGrid.PushGrid))]
    internal static class LevelGridGrid__PushGrid
    {
        [HarmonyTranspiler]
        internal static IEnumerable<CodeInstruction> AddButton(IEnumerable<CodeInstruction> instructions)
        {
            Mod.Log.LogInfo("Transpiling...");

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (int i = 3; i < codes.Count; i++)
            {
                if ((codes[i - 2].opcode == OpCodes.Callvirt && ((MethodInfo)codes[i - 2].operand).Name == "Push") &&
                    (codes[i].opcode == OpCodes.Call && ((MethodInfo)codes[i].operand).Name == "GridPushChange"))
                {
                    Mod.Log.LogInfo($"call MenuPanel.Push @ {i - 2}");

                    List<Label> labels = new List<Label>(codes[i - 3].labels);
                    codes[i - 3].labels.Clear();
                    codes.InsertRange(i - 3, new CodeInstruction[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0, null),
                        new CodeInstruction(OpCodes.Call, typeof(LevelGridGrid__PushGrid).GetMethod(nameof(AddCampaignOptionButton))),
                    });
                    codes[i - 3].labels.AddRange(labels);

                    break;
                }
            }

            return codes.AsEnumerable();
        }

        public static void AddCampaignOptionButton(LevelGridGrid instance)
        {
            if (instance.levelGridMenu_.displayType_ == LevelSelectMenuAbstract.DisplayType.Arcade)
            {
                instance.GetOrAddComponent<CreateCampaignLogic>();
                MenuPanel menuPanel = instance.gridPanel_.GetComponent<MenuPanel>();

                if (menuPanel && !instance.levelGridMenu_.IsSimpleMenu_)
                {
                    //Mod.Log.LogInfo("Not a simple menu!");
                    if (!instance.playlist_.IsResourcesPlaylist())
                    {
                        //Mod.Log.LogInfo("Resources Playlist!!");
                        menuPanel.SetBottomLeftButton(InputAction.MenuSpecial_3, "CREATE\nCAMPAIGN");
                    }
                }
            }
        }
    }
}
