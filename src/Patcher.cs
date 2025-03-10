using HarmonyLib;

namespace FastReset {
    /**
     * <summary>
     * A class which applies any harmony patches
     * for fast reset.
     * </summary>
     */
    public class Patcher {
        /**
         * <summary>
         * Applies early patches.
         * These patches are applied on Awake.
         * </summary>
         */
        public void PatchEarly() {
            Harmony.CreateAndPatchAll(typeof(Patches.InventoryFix));
            Harmony.CreateAndPatchAll(typeof(Patches.ResetPositionEnter));
            Harmony.CreateAndPatchAll(typeof(Patches.ResetPositionStay));
            Plugin.LogDebug("Patcher: Applied early patches");
        }
    }
}
