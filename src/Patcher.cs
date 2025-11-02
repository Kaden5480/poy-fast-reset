using HarmonyLib;

namespace FastReset {
    /**
     * <summary>
     * A class which applies any harmony patches
     * for fast reset.
     * </summary>
     */
    public class Patcher : Loggable {
        /**
         * <summary>
         * Applies early patches.
         * These patches are applied on Awake.
         * </summary>
         */
        public void PatchEarly() {
            Harmony.CreateAndPatchAll(typeof(Patches.KeepBricksDisable));
            Harmony.CreateAndPatchAll(typeof(Patches.InventoryFix));
            Harmony.CreateAndPatchAll(typeof(Patches.ResetPositionEnter));
            Harmony.CreateAndPatchAll(typeof(Patches.ResetPositionStay));
            Harmony.CreateAndPatchAll(typeof(Patches.TimeAttackWindUI));
            Harmony.CreateAndPatchAll(typeof(Patches.SceneLoad));
            LogDebug("Applied early patches");
        }
    }
}
