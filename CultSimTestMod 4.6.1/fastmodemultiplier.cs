using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace fastmodemultiplier
{
    [BepInPlugin("justastranger.fastmodemultiplier", "Fast Mode Multiplier", "1.0")]
    [BepInProcess("cultistsimulator.exe")]
    public class fastmodemultiplier : BaseUnityPlugin
    {

        public static ConfigEntry<float> speedMultiplier;
        public static fastmodemultiplier INSTANCE;
        // private static Heart tabletopManagerHeart;

        void Awake()
        {
            // stuff to do right away
            speedMultiplier = Config.Bind("Speed", "Multiplier", 1.0f, "Speed multiplier for fast mode.");
            Logger.LogInfo("Configuration Loaded.");
            Logger.LogInfo("Speed Multiplier set to: " + speedMultiplier.Value.ToString());
            INSTANCE = this;
            var harmony = new Harmony("justastranger.fastmodemultiplier");
            harmony.PatchAll();
            Logger.LogInfo("Harmony Patch Applied");
        }
    }

    [HarmonyPatch(typeof(Heart), "Beat")]
    public class HeartPatch
    {
        static void Postfix(Heart __instance)
        {
            float usualInterval = (float)AccessTools.Field(typeof(Heart), "usualInterval").GetValue(__instance);
            if (__instance.GetGameSpeed() == GameSpeed.Fast)
            {
                __instance.AdvanceTime(((usualInterval * 3) * fastmodemultiplier.speedMultiplier.Value) - ((usualInterval * 3)));
            }
        }
    }
}
