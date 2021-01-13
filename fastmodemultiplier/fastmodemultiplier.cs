using Assets.TabletopUi.Scripts.Infrastructure;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Noon;
using System;
using System.CodeDom;
using UnityEngine;

namespace fastmodemultiplier
{
    [BepInPlugin("justastranger.fastmodemultiplier", "Fast Mode Multiplier", "1.0")]
    [BepInProcess("cultistsimulator.exe")]
    public class fastmodemultiplier : BaseUnityPlugin
    {

        public static ConfigEntry<float> speedMultiplier;
        Harmony HarmonyInstance;
        public static new ManualLogSource Logger = new ManualLogSource("justastranger.fastmodemultiplier");

        // stuff to do right away
        void Awake()
        {
            speedMultiplier = Config.Bind("Config", "Speed Multiplier", 1.0f, new ConfigDescription("Speed multiplier for fast mode.\nBe Reasonable! A too high value will make the game completely unplayable.\nA Multiplier of 10 results in an overall speed of 30 seconds per second.", new AcceptableValueRange<float>(1f, 100f)));
            NoonUtility.Log("Configuration Loaded. Speed Multiplier set to: " + speedMultiplier.Value.ToString(), 0, VerbosityLevel.Essential);
            if (speedMultiplier.Value < 1f)
            {
                NoonUtility.Log("Speed Multiplier was less than one, which is not yet supported. Defaulting to 1f.", 0, VerbosityLevel.Essential);
                speedMultiplier.Value = 1f;
            }
            HarmonyInstance = new Harmony("justastranger.fastmodemultiplier");
            HarmonyInstance.PatchAll();
            NoonUtility.Log("Harmony Patches Applied", 0, VerbosityLevel.Essential);
        }
    }

    [HarmonyPatch(typeof(Heart), "Update")]
    public class HeartPatch
    {
        static bool Prefix(Heart __instance)
        {
            try
            {
                float usualInterval = (float)AccessTools.Field(typeof(Heart), "BEAT_INTERVAL_SECONDS").GetValue(__instance);
                float timerBetweenBeats = (float)AccessTools.Field(typeof(Heart), "timerBetweenBeats").GetValue(__instance);
                GameSpeedState gameSpeedState = (GameSpeedState)AccessTools.Field(typeof(Heart), "gameSpeedState").GetValue(__instance);
                if (fastmodemultiplier.speedMultiplier.Value < 1f)
                {
                    NoonUtility.Log("Speed Multiplier was less than one, which is not yet supported. Defaulting to 1f.", 0, VerbosityLevel.Essential);
                    fastmodemultiplier.speedMultiplier.Value = 1f;
                }
                timerBetweenBeats += Time.deltaTime;
                AccessTools.Field(typeof(Heart), "timerBetweenBeats").SetValue(__instance, timerBetweenBeats);
                if (timerBetweenBeats > 0.05f)
                {
                    timerBetweenBeats -= 0.05f;
                    AccessTools.Field(typeof(Heart), "timerBetweenBeats").SetValue(__instance, timerBetweenBeats);
                    if (gameSpeedState.GetEffectiveGameSpeed() == GameSpeed.Fast)
                    {
                        // NoonUtility.Log("Beat for: " + (0.15f * fastmodemultiplier.speedMultiplier.Value).ToString(), 0, VerbosityLevel.Essential);
                        // __instance.Beat(0.15f);
                        __instance.Beat(0.15f * fastmodemultiplier.speedMultiplier.Value);
                    }
                    else if (gameSpeedState.GetEffectiveGameSpeed() == GameSpeed.Normal)
                    {
                        __instance.Beat(0.05f);
                    }
                    else if (gameSpeedState.GetEffectiveGameSpeed() == GameSpeed.Paused)
                    {
                        __instance.Beat(0f);
                    }
                    else
                    {
                        NoonUtility.Log("Unknown game speed state: " + gameSpeedState.GetEffectiveGameSpeed(), 0, VerbosityLevel.Trivia);
                    }
                }
            }
            catch (Exception e)
            {
                NoonUtility.LogException(e);
                throw;
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(SpeedControlUI), "RespondToSpeedControlCommand")]
    public class SpeedControllerPatch
    {
        static void Postfix(SpeedControlUI __instance)
        {
            try
            {
                GameSpeedState uiShowsGameSpeed = (GameSpeedState)AccessTools.Field(typeof(SpeedControlUI), "uiShowsGameSpeed").GetValue(__instance);
                if (uiShowsGameSpeed.GetEffectiveGameSpeed() == GameSpeed.Fast)
                {
                    // GameSpeed is already fast, so that means we need to make it faster!
                    fastmodemultiplier.speedMultiplier.Value += 1f;
                    // NoonUtility.Log("GetEffectiveGameSpeed() == GameSpeed.Fast, multiplier value set to: " + fastmodemultiplier.speedMultiplier.Value);
                }
                else if (uiShowsGameSpeed.GetEffectiveGameSpeed() == GameSpeed.Normal)
                {
                    // decrease speed multiplier to baseline
                    fastmodemultiplier.speedMultiplier.Value = 1f;
                    // NoonUtility.Log("Set Normal Speed Multiplier");
                }
            }
            catch (Exception e)
            {
                NoonUtility.LogException(e);
            }
        }
    }
}
