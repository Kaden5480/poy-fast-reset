using System.Collections;

using HarmonyLib;
using UnityEngine;

using Cfg = FastReset.Config.Cfg;

namespace FastReset.Patches {
    /**
     * <summary>
     * A class used for controlling the wind on
     * wuthering crest.
     * </summary>
     */
    public class WindResetter : MonoBehaviour {
        private PeakWind peakWind {
            get => Plugin.instance.cache.peakWind;
        }

        /**
         * <summary>
         * Sets the wind to use the maximum possible duration
         * before reverting to old RNG behaviour.
         * </summary>
         */
        private IEnumerator ResetWind() {
            Plugin.LogDebug("WindResetter: Peak wind found on map, stopping wind cycle");
            peakWind.StopCoroutine("AddWindToHands");
            peakWind.harshWindSound.volume = 0f;
            peakWind.playerWindForce.force = peakWind.windVectorDirection;

            Plugin.LogDebug($"WindResetter: Waiting for: {peakWind.waitBeforeHarshWindMax}s");
            yield return new WaitForSeconds(peakWind.waitBeforeHarshWindMax);

            Plugin.LogDebug("WindResetter: Restarting wind cycle");
            peakWind.StartCoroutine("AddWindToHands");
        }

        /**
         * <summary>
         * Resets the wind if possible.
         * </summary>
         */
        public void Reset() {
            if (peakWind == null) {
                Plugin.LogDebug("WindResetter: No peak wind on map, not resetting");
                return;
            }

            // Stop running the old coroutine
            // before starting another
            StopAllCoroutines();
            StartCoroutine(ResetWind());
        }
    }
}
