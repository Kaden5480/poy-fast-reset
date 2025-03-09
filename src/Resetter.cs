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
        private PlayerState player = new PlayerState();

        // An object for managing the scene's state
        private SceneState scene = new SceneState();

        // An object for managing the state of the wind
        private WindResetter windResetter;

        /**
         * <summary>
         * Constructs an instance of Resetter.
         * </summary>
         */
        public Resetter() {
            // Create the peak wind resetter object
            GameObject windResetterObj = new GameObject("Fast Reset Wind Resetter");
            GameObject.DontDestroyOnLoad(windResetterObj);

            windResetter = windResetterObj.AddComponent<WindResetter>();
        }

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

            player.SaveState();
            scene.SaveState();

            audio.PlayPlayer();
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

            windResetter.Reset();
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
            windResetter.Stop();
            player.Unload();
            scene.Unload();
        }
    }
}
