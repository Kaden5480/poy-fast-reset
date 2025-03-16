using HarmonyLib;
using UnityEngine;

namespace FastReset.Patches {
    /**
     * <summary>
     * Patches ResetPosition to restore state where possible.
     * </summary>
     */
    [HarmonyPatch(typeof(ResetPosition), "OnTriggerEnter")]
    static class ResetPositionEnter {
        static void LogDebug(string message) {
            Plugin.LogDebug($"[{typeof(ResetPositionEnter)}] {message}");
        }

        static bool Prefix(ResetPosition __instance, Collider other) {
            // Don't override normal execution in these cases
            if (ResetPosition.resettingPosition == true
                || other.CompareTag("PlayerTrigger") == false
            ) {
                return true;
            }

            // If restoring state failed, don't override normal execution
            if (Plugin.instance.resetter.RestoreState() == false) {
                LogDebug("Not bypassed");
                return true;
            }

            LogDebug("Bypassed, updating falls");

            int falls = Plugin.instance.cache.fallingEvent.falls;
            int globalFalls = GameManager.control.global_stats_falls;

            // Update falls
            Plugin.instance.cache.fallingEvent.falls++;
            GameManager.control.global_stats_falls++;
            GameManager.control.SaveAllStats();

            LogDebug($"Increased falls: {falls} -> {Plugin.instance.cache.fallingEvent.falls}");
            LogDebug($"Increased global falls: {globalFalls} -> {GameManager.control.global_stats_falls}");

            // Bypass normal execution
            return false;
        }
    }

    /**
     * <summary>
     * Bypasses the OnTriggerStay for ResetPosition if
     * the state can be restored.
     * </summary>
     */
    [HarmonyPatch(typeof(ResetPosition), "OnTriggerStay")]
    static class ResetPositionStay {
        static void LogDebug(string message) {
            Plugin.LogDebug($"[{typeof(ResetPositionStay)}] {message}");
        }

        static bool Prefix(Collider other) {
            // Don't override normal execution in these cases
            if (ResetPosition.resettingPosition == true
                || other.CompareTag("PlayerTrigger") == false
            ) {
                return true;
            }

            // If saving/restoring is not permitted
            // continue with normal execution
            if (Plugin.instance.resetter.CanUse() == false) {
                LogDebug("Not bypassed");
                return true;
            }

            LogDebug("Bypassed");
            return false;
        }
    }
}
