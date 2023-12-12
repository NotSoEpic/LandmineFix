using BepInEx;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace LandmineFix
{
    [BepInPlugin("beeisyou.LandmineFix", "Landmine Fix", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource log = new ManualLogSource("Landmine Fix");
        public static Harmony harmony = new Harmony("beeisyou.LandmineFix");
        private void Awake()
        {
            BepInEx.Logging.Logger.Sources.Add(log);
            harmony.PatchAll(typeof(FixTeleportedValue));
            log.LogInfo("Plugin Landmine Fix is loaded!");
        }
    }

    [HarmonyPatch(typeof(PlayerControllerB), "Update")]
    public class FixTeleportedValue
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            Plugin.log.LogWarning("Beginning transpilation of GameNetworkManager.ResetSavedGameValues()");
            var codes = new List<CodeInstruction>(instructions);

            var sign1 = typeof(PlayerControllerB).GetField("teleportedLastFrame");
            var sign2 = typeof(PlayerControllerB).GetField("jetpackControls");
            bool success = false;

            for (int i = 0; i < codes.Count - 7; i++)
            {
                /* 
                 * removes "if (this.jetpackControls || this.disablingJetpackControls)"
                 * that blocks teleportedThisFrame from resetting properly
                 */
                if (codes[i].StoresField(sign1) && codes[i+2].LoadsField(sign2))
                {
                    List<Label> labels = codes[i + 1].labels;
                    codes[i+1] = new CodeInstruction(OpCodes.Nop);
                    codes[i+1].labels = labels;
                    codes.RemoveRange(i + 2, 5);
                    success = true;
                    break;
                }
            }

            if (success)
            {
                Plugin.log.LogInfo("Successfully patched PlayerControllerB.Update");
            }
            else
            {
                Plugin.log.LogError("Failed to patch PlayerControllerB.Update");
            }

            return codes.AsEnumerable();
        }
    }
}