using HarmonyLib;

namespace FastReset {
    public class Patcher {
        /**
         * <summary>
         * Applies early patches.
         * </summary>
         */
        public void Patch() {
            Harmony.CreateAndPatchAll(typeof(Patches.ResetPositionEnter));
            Harmony.CreateAndPatchAll(typeof(Patches.ResetPositionStay));
            Plugin.LogDebug("Patcher: Applied patches");
        }
    }
}
