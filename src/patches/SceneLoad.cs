using HarmonyLib;
using UnityEngine.SceneManagement;

namespace FastReset.Patches {
    /**
     * <summary>
     * Hooks into Fast Reset's scene load methods.
     * </summary>
     */
    static class SceneLoad {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(EnterPeakScene), "Start")]
        [HarmonyPatch(typeof(EnterRoomSegmentScene), "Start")]
        static void Postfix() {
            Plugin.instance.OnSceneLoaded(SceneManager.GetActiveScene());
        }
    }
}
