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
        Harmony HarmonyInstance;

        // stuff to do right away
        void Awake()
        {
            speedMultiplier = Config.Bind("Speed", "Multiplier", 1.0f, "Speed multiplier for fast mode.\nAllowed Values: 1-∞\nBe Reasonable! A too high value will make the game completely unplayable.\nA Multiplier of 10 results in an overall speed of 30 seconds per second.");
            Logger.LogInfo("Configuration Loaded. Speed Multiplier set to: " + speedMultiplier.Value.ToString());
            if (speedMultiplier.Value < 1f)
            {
                Logger.LogWarning("Speed Multiplier was less than one, which is not yet supported. Defaulting to 1f.");
                speedMultiplier.Value = 1f;
            }
            if (speedMultiplier.Value > 1f)
            {
                HarmonyInstance = new Harmony("justastranger.fastmodemultiplier");
                HarmonyInstance.PatchAll();
                Logger.LogInfo("Harmony Patch Applied");
            }
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
