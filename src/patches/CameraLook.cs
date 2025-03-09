using HarmonyLib;
using UnityEngine;

using Cfg = FastReset.Config.Cfg;

namespace FastReset.Patches {
    [HarmonyPatch(typeof(CameraLook), "Test")]
    static class CameraLookTest {
        private static Cfg config {
            get => Plugin.instance.config;
        }
        private static Cache cache {
            get => Plugin.instance.cache;
        }
        private static Resetter resetter {
            get => Plugin.instance.resetter;
        }

        static void Prefix(CameraLook __instance) {
            if (__instance != cache.playerCamX) {
                return;
            }

            // Check for inputs for saves/restores
            if (Input.GetKeyDown(config.saveKeybind.Value) == true) {
                resetter.SaveState();
            }

            if (Input.GetKeyDown(config.resetKeybind.Value) == true) {
                resetter.RestoreState();
                __instance.limitViewWhileLookingAtInventoryGrounded = false;
            }

        }
    }
}
