using System.Collections;

using HarmonyLib;
using UnityEngine;

using Cfg = FastReset.Config.Cfg;

namespace FastReset.Patches {
    public class WindResetter : MonoBehaviour {
        // Shorthand for accessing the cache and config
        private Cfg config {
            get => Plugin.instance.config;
        }

        private Cache cache {
            get => Plugin.instance.cache;
        }

        private PeakWind peakWind {
            get => cache.peakWind;
        }

        /**
         * <summary>
         * Restores the state of the peak wind on
         * wuthering crest.
         *
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

        public void Reset() {
            // Only reset if configured to do so
            if (config.resetWind.Value == false) {
                return;
            }

            // Only reset on wuthering crest
            if ("Peak_11_WutheringCrestNEW".Equals(cache.scene.name) == false) {
                return;
            }

            if (peakWind == null) {
                Plugin.LogDebug("WindResetter: No peak wind on map, not resetting");
                return;
            }

            StopAllCoroutines();
            StartCoroutine(ResetWind());
        }

        public void Stop() {
            StopAllCoroutines();
        }
    }
}
