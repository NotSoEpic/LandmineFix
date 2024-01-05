using BepInEx;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        static void PostFix(ref PlayerControllerB playerControllerB)
        {
            typeof(PlayerControllerB).GetField("teleportingThisFrame", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(playerControllerB, false);
        }
    }
}