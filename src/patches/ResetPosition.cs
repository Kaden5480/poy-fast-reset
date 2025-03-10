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
        static bool Prefix(ResetPosition __instance, Collider other) {
            // Don't override normal execution in these cases
            if (ResetPosition.resettingPosition == true
                || other.CompareTag("PlayerTrigger") == false
            ) {
                return true;
            }

            // If restoring state failed, don't override normal execution
            if (Plugin.instance.resetter.RestoreState() == false) {
                Plugin.LogDebug("ResetPositionEnter: not bypassed");
                return true;
            }

            Plugin.LogDebug("ResetPositionEnter: bypassed, updating falls");

            int falls = Plugin.instance.cache.fallingEvent.falls;
            int globalFalls = GameManager.control.global_stats_falls;

            // Update falls
            Plugin.instance.cache.fallingEvent.falls++;
            GameManager.control.global_stats_falls++;
            GameManager.control.SaveAllStats();

            Plugin.LogDebug($"ResetPositionEnter: Increased falls: {falls} -> {Plugin.instance.cache.fallingEvent.falls}");
            Plugin.LogDebug($"ResetPositionEnter: Increased global falls: {globalFalls} -> {GameManager.control.global_stats_falls}");

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
                Plugin.LogDebug("ResetPositionStay: not bypassed");
                return true;
            }

            Plugin.LogDebug("ResetPositionStay: bypassed");
            return false;
        }
    }
}
