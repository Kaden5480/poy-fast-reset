using UnityEngine;

using Cfg = FastReset.Config.Cfg;
using PlayerState = FastReset.State.PlayerState;
using SceneState = FastReset.State.SceneState;
using WindResetter = FastReset.Patches.WindResetter;

namespace FastReset {
    public class Resetter {
        // Shorthands for accessing audio, config, and cache
        private Audio audio { get => Plugin.instance.audio; }
        private Cfg config { get => Plugin.instance.config; }
        private Cache cache { get => Plugin.instance.cache; }

        // An object for managing the player's state
        public PlayerState player { get; } = new PlayerState();

        // An object for managing the scene's state
        public SceneState scene { get; } = new SceneState();

        // An object for managing the state of the wind
        private WindResetter windResetter = null;

        // UI state for changing resetter behaviour
        private UI.State uiState { get => Plugin.instance.ui.state; }

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
                Plugin.LogDebug("Resetter: yfyd/fs active, unable to use resetter");
                return false;
            }

            // Ensure the cache contains everything required
            if (cache.IsComplete() == false) {
                Plugin.LogError("Resetter: Cache is incomplete, unable to use resetter");
                return false;
            }

            // If the player is receiving a score, prevent resetting
            if (cache.timerLocked == false && TimeAttack.receivingScore == true) {
                Plugin.LogDebug("Resetter: Receiving score, unable to use resetter");
                return false;
            }

            // The bivouac, crampons, and rope must not be in use
            if (Bivouac.currentlyUsingBivouac == true
                || Bivouac.movingToBivouac == true
                || Crampons.cramponsActivated == true
                || cache.ropeAnchor.attached == true
            ) {
                Plugin.LogDebug("Resetter: Using bivouac/crampons/rope, unable to use resetter");
                return false;
            }

            // Check load states, stamping, placing summit flag
            if (InGameMenu.isCurrentlyNavigationMenu == true
                || EnterPeakScene.enteringPeakScene == true
                || ResetPosition.resettingPosition == true
                || StamperPeakSummit.currentlyStampingPeakJournal == true
                || SummitFlag.placingEvent == true
            ) {
                Plugin.LogDebug("Resetter: Other misc case, unable to use resetter");
                return false;
            }

            // On ST, the distance activator is required
            if (cache.routingFlag.isSolemnTempest == true
                && cache.routingFlag.distanceActivatorST == null
            ) {
                Plugin.LogDebug("Resetter: Missing distance activator on ST, unable to use resetter");
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
                Plugin.LogDebug("Resetter: Unable to save currently");
                return;
            }

            Plugin.LogDebug("Resetter: Saving state");

            if (uiState.editPlayerState == true) {
                player.SaveState();
            }

            if (uiState.editSceneState == true) {
                scene.SaveState();
                audio.PlayScene();
            }

            // Play normal audio if not editing scene
            if (uiState.editPlayerState == true
                && uiState.editSceneState == false
            ) {
                audio.PlayPlayer();
            }

        }

        /**
         * <summary>
         * Restores the saved state
         * </summary>
         */
        public bool RestoreState() {
            if (CanUse() == false) {
                Plugin.LogDebug("Resetter: Unable to restore currently");
                return false;
            }

            // Try restoring state
            Plugin.LogDebug("Resetter: Restoring state");

            if (player.RestoreState() == false) {
                Plugin.LogDebug("Resetter: Failed restoring player state");
                return false;
            }

            if (windResetter != null) {
                windResetter.Reset();
            }

            scene.RestoreState();

            audio.PlayPlayer();

            return true;
        }

        /**
         * <summary>
         * Loads any saved states.
         * </summary>
         */
        public void LoadStates() {
            Plugin.LogDebug("Resetter: Loading saved states");

            // Create the peak wind resetter object
            if (config.resetWind.Value == true
                && "Peak_11_WutheringCrestNEW".Equals(cache.scene.name) == true
            ) {
                Plugin.LogDebug("Resetter: Creating wind resetter");
                GameObject windResetterObj = new GameObject("Fast Reset Wind Resetter");
                windResetter = windResetterObj.AddComponent<WindResetter>();
            }

            player.Load();
            scene.Load();
        }

        /**
         * <summary>
         * Unloads states to prepare
         * for the next scene.
         * </summary>
         */
        public void UnloadStates() {
            Plugin.LogDebug("Resetter: Unloading states");

            if (windResetter != null) {
                Plugin.LogDebug("Resetter: Destroying wind resetter");
                GameObject.Destroy(windResetter.gameObject);
                windResetter = null;
            }

            player.Unload();
            scene.Unload();
        }

        /**
         * <summary>
         * Reloads config states.
         * Used for profile changes.
         * </summary>
         */
        public void ReloadStates() {
            Plugin.LogDebug("Resetter: Reloading states");
            player.Reload();
            scene.Reload();
        }
    }
}
