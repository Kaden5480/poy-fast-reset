using StateManager = FastReset.State.StateManager;

namespace FastReset {
    /**
     * <summary>
     * A class that determines whether saves/restores
     * are permitted and dispatches these requests
     * to the StateManager.
     * </summary>
     */
    public class Resetter : Loggable {
        // Shorthand for accessing cache
        private Cache cache { get => Plugin.instance.cache; }

        // An object for managing the scene's state
        public StateManager stateManager { get; } = new StateManager();

        /**
         * <summary>
         * Checks whether saving/restoring can be used.
         * </summary>
         * <returns>True if it can, false otherwise</returns>
         */
        public bool CanUse() {
            // Only allowed in normal mode
            if (GameManager.control.permaDeathEnabled == true
                || GameManager.control.freesoloEnabled == true
            ) {
                LogDebug("yfyd/fs active, unable to use resetter");
                return false;
            }

            // Ensure the cache contains everything required
            if (cache.IsComplete() == false) {
                LogError("Cache is incomplete, unable to use resetter");
                return false;
            }

            // If the player is receiving a score, prevent resetting
            if (cache.timerLocked == false && TimeAttack.receivingScore == true) {
                LogDebug("Receiving score, unable to use resetter");
                return false;
            }

            // The bivouac, crampons, and rope must not be in use
            if (Bivouac.currentlyUsingBivouac == true
                || Bivouac.movingToBivouac == true
                || Crampons.cramponsActivated == true
                || cache.ropeAnchor.attached == true
            ) {
                LogDebug("Using bivouac/crampons/rope, unable to use resetter");
                return false;
            }

            // Check load states, stamping, placing summit flag
            if (InGameMenu.isCurrentlyNavigationMenu == true
                || EnterPeakScene.enteringPeakScene == true
                || ResetPosition.resettingPosition == true
                || StamperPeakSummit.currentlyStampingPeakJournal == true
                || SummitFlag.placingEvent == true
            ) {
                LogDebug("Other misc case, unable to use resetter");
                return false;
            }

            // On ST, the distance activator is required
            if (cache.routingFlag.isSolemnTempest == true
                && cache.routingFlag.distanceActivatorST == null
            ) {
                LogDebug("Missing distance activator on ST, unable to use resetter");
                return false;
            }

            return true;
        }

        /**
         * <summary>
         * Checks whether saving is currently permitted.
         * </summary>
         * <returns>True if it is, false otherwise</returns>
         */
        private bool CanSave() {
            // Player must be grounded
            if (cache.playerMove.IsGrounded() == false) {
                return false;
            }

            // If not in routing flag mode, the player
            // must also be at the base of a peak
            if (cache.routingFlag.currentlyUsingFlag == false
                && cache.timeAttack.isInColliderActivationRange == false
            ) {
                return false;
            }

            return true;
        }

        /**
         * <summary>
         * Saves the current state
         * </summary>
         */
        public void SaveState() {
            if (CanUse() == false
                || CanSave() == false
            ) {
                LogDebug("Unable to save currently");
                return;
            }

            stateManager.SaveState();
        }

        /**
         * <summary>
         * Restores the saved state
         * </summary>
         */
        public bool RestoreState() {
            if (CanUse() == false) {
                LogDebug("Unable to restore currently");
                return false;
            }

            stateManager.RestoreState();
            return true;
        }
    }
}
