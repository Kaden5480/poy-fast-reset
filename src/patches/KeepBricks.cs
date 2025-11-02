using System.Collections;

using HarmonyLib;
using UnityEngine;

namespace FastReset.Patches {
    /**
     * <summary>
     * Patches bricks to prevent them from being
     * deleted once popped out.
     * </summary>
     */
    [HarmonyPatch(typeof(BrickHold), "DisableBrickFunctionality")]
    static class KeepBricksDisable {
        /**
         * <summary>
         * Replaces the coroutine.
         * </summary>
         */
        static bool Prefix(BrickHold __instance, ref IEnumerator __result) {
            __result = KeepBricksCoroutine(__instance);
            return false;
        }

        static IEnumerator KeepBricksCoroutine(BrickHold brick) {
            AudioSource brickSound = (AudioSource) AccessTools.Field(
                typeof(BrickHold), "brickSound"
            ).GetValue(brick);
            Rigidbody rb = (Rigidbody) AccessTools.Field(
                typeof(BrickHold), "rb"
            ).GetValue(brick);
            Collider col = (Collider) AccessTools.Field(
                typeof(BrickHold), "col"
            ).GetValue(brick);

            // Disable the brick
            if (brickSound != null) { brickSound.Stop(); }
            yield return new WaitForSeconds(9f);

            if (rb != null)         { rb.isKinematic = true;      }
            yield return new WaitForSeconds(0.1f);

            if (brickSound != null) { brickSound.enabled = false; }
            if (col != null)        { col.enabled = false;        }
        }
    }
}
