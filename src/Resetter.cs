using UnityEngine;

using StateManager = FastReset.State.StateManager;
using WindResetter = FastReset.Patches.WindResetter;

namespace FastReset {
    /**
     * <summary>
     * A class that determines whether saves/restores
     * are permitted and dispatches these requests
     * to the StateManager.
     * </summary>
     */
    public class Resetter : Loggable {
        // Shorthand for accessing cache and audio
        private Cache cache { get => Plugin.instance.cache; }
        private Audio audio { get => Plugin.instance.audio; }

        // An object for managing the scene's state
        private StateManager stateManager { get; } = new StateManager();

        /**
         * <summary>
         * Checks whether saving/restoring can be
         * used in the current scene.
         *
         * This is also used for early scene load checks
         * to determine whether to create the wind resetter
         * and save initial states.
         * </summary>
         * <returns>True if it can, false otherwise</returns>
         */
        public bool CanUseInScene() {
            // Only allowed in normal mode
            if (GameManager.control.permaDeathEnabled == true
                || GameManager.control.freesoloEnabled == true
            ) {
                LogDebug("yfyd/fs active, unable to use resetter");
                return false;
            }

            // Ensure the cache contains everything required
            if (cache.IsComplete() == false) {
                LogDebug("Cache is incomplete, unable to use resetter");
                return false;
            }

            return true;
        }

        /**
         * <summary>
         * Checks whether saving/restoring can be used.
         * </summary>
         * <returns>True if it can, false otherwise</returns>
         */
        public bool CanUse() {
            if (CanUseInScene() == false) {
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
            // If in routing flag mode, can always save
            if (cache.routingFlag.currentlyUsingFlag == true) {
                return true;
            }

            // If not in routing flag mode, the player
            // must be grounded at the base of a peak
            if (cache.playerMove.IsGrounded() == true
                && cache.timeAttack.isInColliderActivationRange == true
            ) {
                return true;
            }

            return false;
        }

#region Player Controlled Saves/Restores

        /**
         * <summary>
         * Saves the current state
         * </summary>
         * <returns>Whether the state was saved</returns>
         */
        public bool SaveState() {
            if (CanUse() == false
                || CanSave() == false
            ) {
                audio.PlayFailure();
                LogDebug("Unable to save currently");
                return false;
            }

            if (stateManager.SaveState() == true) {
                audio.PlaySave();
                return true;
            }

            audio.PlayFailure();
            return false;
        }

        /**
         * <summary>
         * Restores the saved state and resets wind
         * when appropriate.
         * </summary>
         * <returns>Whether a state was restored</returns>
         */
        public bool RestoreState() {
            if (CanUse() == false) {
                audio.PlayFailure();
                LogDebug("Unable to restore currently");
                return false;
            }

            // Catch failures
            if (stateManager.RestoreState() == false) {
                audio.PlayFailure();
                return false;
            }

            // Try resetting the wind
            WindResetter.Reset();
            audio.PlayRestore();

            return true;
        }

#endregion

#region Scene Loads/Unloads

        /**
         * <summary>
         * Performs any required state updates on a scene load.
         * Currently this is just loading the initial state.
         * </summary>
         */
        public void SceneLoad() {
            // Ignore scenes where fast reset is unavailable
            if (CanUseInScene() == false) {
                return;
            }

            // Create the wind resetter
            WindResetter.Create();

            stateManager.SaveInitialState();
        }

        /**
         * <summary>
         * Performs any required state updates on a scene unload.
         * Currently this is just wiping the state.
         * </summary>
         */
        public void SceneUnload() {
            WindResetter.Destroy();
            stateManager.WipeState();
        }

#endregion
    }
}
