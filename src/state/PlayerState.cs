using System;
using System.Reflection;

using HarmonyLib;
using UnityEngine;

using Cfg = FastReset.Config.Cfg;
using PositionFix = FastReset.Patches.PositionFix;
using SaveManager = FastReset.Saves.SaveManager;
using SavedPlayer = FastReset.Saves.SavedPlayer;

namespace FastReset.State {
    public class PlayerState : Loggable, BaseState {
        // Shorthand for accessing the cache and config
        private Cache cache {
            get => Plugin.instance.cache;
        }

        private Cfg config {
            get => Plugin.instance.config;
        }

        // Initial and temporary states
        private Vector3 initialPosition = Vector3.zero;
        private Quaternion initialRotationX = Quaternion.identity;
        private float initialRotationY = 0f;

        // The player's current position and rotation
        private Vector3 position {
            get => PositionFix.RealToOffset(cache.playerTransform.position);
            set => cache.playerTransform.position = PositionFix.OffsetToReal(value);
        }
        private Quaternion rotationX {
            get => cache.playerCamX.gameObject.transform.rotation;
            set => cache.playerCamX.gameObject.transform.rotation = value;
        }
        private float rotationY {
            get => cache.playerCamY.rotationY;
            set => cache.playerCamY.rotationY = value;
        }

        /**
         * <summary>
         * Reapplies coffee using Fast Coffee.
         * </summary>
         */
        private void ReapplyCoffee() {
            if (config.reapplyCoffee.Value == false) {
                LogDebug("Reapplying coffee disabled, skipping");
                return;
            }

            Type fastCoffee = AccessTools.TypeByName("FastCoffee.Coffee");
            if (fastCoffee == null) {
                LogDebug("Unable to resolve FastCoffee.Coffee, skipping");
                return;
            }

            MethodInfo reapply = AccessTools.Method(fastCoffee, "Reapply");
            if (reapply == null) {
                LogDebug("Unable to resolve FastCoffee.Coffee.Reapply, skipping");
                return;
            }

            reapply.Invoke(null, new object[] { config.slurpSound.Value });
            LogDebug("Invoked fast coffee's reapply");
        }

        /**
         * <summary>
         * Moves the player to the provided position with the specified
         * camera rotation.
         * </summary>
         * <param name="position">The position to move to (relative to the LeavePeakScene object)</param>
         * <param name="rotationX">The rotation to apply to camX</param>
         * <param name="rotationY">The rotation to apply to camY</param>
         */
        private void MoveTo(Vector3 position, Quaternion rotationX, float rotationY) {
            // Release grip
            cache.climbing.ReleaseLHand(false);
            cache.climbing.ReleaseRHand(false);
            cache.iceAxes.ReleaseLeft(false);
            cache.iceAxes.ReleaseRight(false);

            cache.playerRb.velocity = Vector3.zero;

            // Prevent death on reset
            cache.fallingEvent.fellShortDistance = false;
            cache.fallingEvent.fellLongDistance = false;
            cache.fallingEvent.fellToDeath = false;

            // Detach from the routing flag
            cache.routingFlag.usedFlagTeleport = false;

            // Update the player's position and rotation
            this.position = position;
            this.rotationX = rotationX;
            this.rotationY = rotationY;

            cache.playerCamX.PlayerGrabbed();

            if (cache.routingFlag.isSolemnTempest == true) {
                cache.routingFlag.distanceActivatorST.ForceCheck();
                LogDebug("Force checked ST distance activator");
            }

            // Reapply coffee by integrating with Fast Coffee
            ReapplyCoffee();

            // Log actual values
            LogDebug(
                $"Moved player to: {position} (relative) | {rotationX} | {rotationY}"
            );
        }

#region Initial

        /**
         * <summary>
         * Methods which are used for saving/restoring the
         * initial state of the player.
         * </summary>
         */
        public void SaveInitialState() {
            initialPosition = position;
            initialRotationX = rotationX;
            initialRotationY = rotationY;
            LogDebug("Saved initial state");
        }

        public void RestoreInitialState() {
            MoveTo(initialPosition, initialRotationX, initialRotationY);
            LogDebug("Restored initial state");
        }

#endregion

#region Saved

        /**
         * <summary>
         * Methods which are used for saving/restoring
         * player state from fast reset's data store.
         * </summary>
         */
        public bool HasSavedState() {
            return SaveManager.GetPlayer() != null;
        }

        public void SaveState() {
            SavedPlayer player = new SavedPlayer();

            // Update the saved state
            player.position = position;
            player.rotationX = rotationX;
            player.rotationY = rotationY;

            SaveManager.WipePlayer();
            SaveManager.AddPlayer(player);

            LogDebug("Added new player state to data store");
        }

        public void RestoreState() {
            SavedPlayer player = SaveManager.GetPlayer();

            if (player == null) {
                LogDebug("No saved player state to restore");
                return;
            }

            MoveTo(player.position, player.rotationX, player.rotationY);
            LogDebug("Restored state from data store");
        }

#endregion

#region Cleaning Up

        /**
         * <summary>
         * Wipes any stored states, typically used
         * on scene unloads.
         *
         * WARNING: This doesn't save anything
         * </summary>
         */
        public void WipeState() {
            initialPosition = Vector3.zero;
            initialRotationX = Quaternion.identity;
            initialRotationY = 0f;
        }

#endregion

    }
}
