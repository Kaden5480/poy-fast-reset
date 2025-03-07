using UnityEngine;

using Cfg = FastReset.Config.Cfg;
using Player = FastReset.Models.Player;

namespace FastReset {
    public class Resetter {
        // Shorthands for accessing audio, config, and cache
        private Audio audio { get => Plugin.instance.audio; }
        private Cfg config { get => Plugin.instance.config; }
        private Cache cache { get => Plugin.instance.cache; }

        // An object for managing the player's state
        private Player player = new Player();

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
                Plugin.LogDebug("yfyd/fs active, unable to use resetter");
                return false;
            }

            // Ensure the cache contains everything required
            if (cache.IsComplete() == false) {
                Plugin.LogError("Cache is incomplete, unable to use resetter");
                return false;
            }

            // If the player is receiving a score, prevent resetting
            if (cache.timerLocked == false && TimeAttack.receivingScore == true) {
                Plugin.LogDebug("Receiving score, unable to use resetter");
                return false;
            }

            // The bivouac, crampons, and rope must not be in use
            if (Bivouac.currentlyUsingBivouac == true
                || Bivouac.movingToBivouac == true
                || Crampons.cramponsActivated == true
                || cache.ropeAnchor.attached == true
            ) {
                Plugin.LogDebug("Using bivouac/crampons/rope, unable to use resetter");
                return false;
            }

            // Check load states, stamping, placing summit flag
            if (InGameMenu.isCurrentlyNavigationMenu == true
                || EnterPeakScene.enteringPeakScene == true
                || ResetPosition.resettingPosition == true
                || StamperPeakSummit.currentlyStampingPeakJournal == true
                || SummitFlag.placingEvent == true
            ) {
                Plugin.LogDebug("Other misc case, unable to use resetter");
                return false;
            }

            // On ST, the distance activator is required
            if (cache.routingFlag.isSolemnTempest == true
                && cache.routingFlag.distanceActivatorST == null
            ) {
                Plugin.LogDebug("Missing distance activator on ST, unable to use resetter");
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
         * Saves the current state of the scene.
         * </summary>
         */
        public void SaveState() {
            if (CanUse() == false) {
                return;
            }

            // Check if saving is permitted
            if (CanSave() == false) {
                Plugin.LogDebug("Saving state is not permitted currently");
                return;
            }

            audio.PlayPlayer();
            player.SaveState();
        }

        /**
         * <summary>
         * Restores the saved state of the scene.
         * </summary>
         * <returns>True if restoring was successful, false otherwise</returns>
         */
        public bool RestoreState() {
            if (CanUse() == false) {
                return false;
            }

            // If the player's state wasn't restored, do nothing else
            if (player.RestoreState() == false) {
                return false;
            }

            audio.PlayPlayer();
            return true;
        }

        /**
         * <summary>
         * Performs any required actions on a scene unload.
         * Such as resetting the player's temporary reset point.
         * </summary>
         */
        public void OnSceneUnloaded() {
            player.OnSceneUnloaded();
        }
    }
}
