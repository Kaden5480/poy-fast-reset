using HarmonyLib;

using WindUI = FastReset.UI.WindUI;

namespace FastReset.Patches {
    /**
     * <summary>
     * Only shows the wind UI when time attack
     * is active.
     * </summary>
     */
    [HarmonyPatch(typeof(TimeAttack), "Update")]
    static class TimeAttackWindUI {
        static void Prefix(TimeAttack __instance) {
            if (WindUI.ui == null) {
                return;
            }

            // Disable when receiving score
            if (TimeAttack.receivingScore == true
                && TimeAttack.aboutToReceiveScore == true
            ) {
                WindUI.ui.SetActive(false);
                return;
            }

            // Otherwise, update the ui's position and show
            // if time attack is enabled
            if (__instance.watchReady == true) {
                WindUI.UpdatePosition();
            }
            WindUI.ui.SetActive(__instance.watchReady);
        }
    }
}
