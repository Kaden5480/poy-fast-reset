using HarmonyLib;
using UnityEngine;

using Cfg = FastReset.Config.Cfg;
using Window = FastReset.UI.Window;

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
        private static Window ui {
            get => Plugin.instance.ui;
        }

        static void Prefix(CameraLook __instance) {
            if (__instance != cache.playerCamX) {
                return;
            }

            // Check for inputs for saves/restores
            if (Input.GetKeyDown(config.saveKeybind.Value) == true) {
                Plugin.LogDebug($"CameraLookTest: {config.saveKeybind.Value} is down");
                if (Input.GetKey(config.toggleModifier.Value) == true) {
                    Plugin.LogDebug($"CameraLookTest: {config.toggleModifier.Value} is down");
                    ui.Toggle();
                }
                else {
                    resetter.SaveState();
                }
            }

            if (Input.GetKeyDown(config.resetKeybind.Value) == true) {
                Plugin.LogDebug($"CameraLookTest: {config.resetKeybind.Value} is down");
                resetter.RestoreState();
                __instance.limitViewWhileLookingAtInventoryGrounded = false;
            }

        }
    }
}
