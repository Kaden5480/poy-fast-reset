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
        private static Cfg config {
            get => Plugin.instance.config;
        }
        private static Cache cache {
            get => Plugin.instance.cache;
        }
        private static PeakWind peakWind {
            get => cache.peakWind;
        }

        private static WindResetter instance = null;

        /**
         * <summary>
         * Logs debug messages.
         * </summary>
         * <param name="message">The message to log</param>
         */
        private static void LogDebug(string message) {
            Plugin.LogDebug($"[WindResetter] {message}");
        }

        /**
         * <summary>
         * Called when this object gets destroyed.
         * </summary>
         */
        public void OnDestroy() {
            instance = null;
            LogDebug("Destroyed");
        }

        /**
         * <summary>
         * Sets up an instance of WindResetter.
         * </summary>
         */
        public static void Create() {
            if ("Peak_11_WutheringCrestNEW".Equals(cache.scene.name) == false) {
                LogDebug("Not creating, not on wuthering crest");
                return;
            }

            if (instance != null) {
                LogDebug("Wind resetter already exists in current scene");
                return;
            }

            GameObject obj = new GameObject("Fast Reset Wind Resetter");
            instance = obj.AddComponent<WindResetter>();

            LogDebug("Created wind resetter");
        }

        /**
         * <summary>
         * Destroys the current instance of WindResetter.
         * </summary>
         */
        public static void Destroy() {
            if (instance == null) {
                LogDebug("Wind resetter already destroyed");
                return;
            }

            Destroy(instance.gameObject);
        }

        /**
         * <summary>
         * Sets the wind to use the maximum possible duration
         * before reverting to old RNG behaviour.
         * </summary>
         */
        private IEnumerator ResetWind() {
            LogDebug("Stopping wind cycle");
            peakWind.StopCoroutine("AddWindToHands");

            peakWind.harshWindSound.volume = 0f;
            peakWind.playerWindForce.force = peakWind.windVectorDirection;
            float waitTime = peakWind.waitBeforeHarshWindMax;

            LogDebug($"Waiting for: {waitTime}s");
            yield return new WaitForSeconds(waitTime);

            LogDebug("Restarting wind cycle");
            peakWind.StartCoroutine("AddWindToHands");
        }

        /**
         * <summary>
         * Resets the wind if possible.
         * </summary>
         * <returns>Whether the wind was reset</returns>
         */
        public static bool Reset() {
            if (peakWind == null) {
                LogDebug("No peak wind on map, not resetting");
                return false;
            }

            if (instance == null) {
                LogDebug("No instance found, not resetting");
                return false;
            }

            if (config.resetWind.Value == false) {
                LogDebug("Resetting wind disabled, not resetting");
                return false;
            }

            // Stop running the old coroutine
            // before starting another
            instance.StopAllCoroutines();
            instance.StartCoroutine(instance.ResetWind());
            return true;
        }
    }
}
