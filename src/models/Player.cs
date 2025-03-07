using UnityEngine;

using Cfg = FastReset.Config.Cfg;
using SavedPoint = FastReset.Config.SavedPoint;

namespace FastReset.Models {
    public class Player {
        // Shorthands for accessing config and cache
        private Cfg config {
            get => Plugin.instance.config;
        }
        private Cache cache {
            get => Plugin.instance.cache;
        }

        // The player's current position and rotation
        public Vector3 position {
            // The position has to be relative to LeavePeakScene to deal with
            // the origin shifter (specifically on ST)
            get => cache.playerTransform.position
                - cache.leavePeakScene.transform.position;
        }
        public Quaternion rotationX {
            get => cache.playerCamX.gameObject.transform.rotation;
        }
        public float rotationY {
            get => cache.playerCamY.rotationY;
        }

        // The temporary save point used in routing flag mode
        BasePoint tempPoint = null;

        /**
         * <summary>
         * Resets the player's state.
         * e.g. releasing grip, resetting velocity and FallingEvent flags
         * </summary>
         */
        private void ResetState() {
            Plugin.LogDebug("Resetting player state");

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
            Plugin.LogDebug($"Attempting to move player to: {position} (relative) | {rotationX} | {rotationY}");
            ResetState();

            // Calculate the real position to move to, as the provided position is an offset
            cache.playerTransform.position = cache.leavePeakScene.transform.position + position;
            cache.playerCamX.gameObject.transform.rotation = rotationX;
            cache.playerCamY.rotationY = rotationY;

            if (cache.routingFlag.isSolemnTempest == true) {
                cache.routingFlag.distanceActivatorST.ForceCheck();
                Plugin.LogDebug("Force checked ST distance activator");
            }

            // Log actual values
            Vector3 newPos = cache.playerTransform.position;
            Quaternion newRotX = cache.playerCamX.gameObject.transform.rotation;
            float newRotY = cache.playerCamY.rotationY;

            Plugin.LogDebug($"Moved player to: {newPos} (real) | {newRotX} | {newRotY}");
        }

        /**
         * <summary>
         * Saves the temporary reset position used
         * in routing flag mode.
         * </summary>
         */
        private void SaveTempState() {
            if (tempPoint == null) {
                tempPoint = new Point();
            }

            Plugin.LogDebug($"Saving temporary point: {position} | {rotationX} | {rotationY}");
            tempPoint.position = position;
            tempPoint.rotationX = rotationX;
            tempPoint.rotationY = rotationY;
        }

        /**
         * <summary>
         * Restores the temporary reset position used
         * in routing flag mode.
         *
         * This will default to the saved position (if it exists)
         * if a temporary position hasn't been set.
         * </summary>
         * <returns>True if the player was moved, false otherwise</returns>
         */
        private bool RestoreTempState() {
            BasePoint point = config.scene.point;

            // Use the temporary point
            if (tempPoint != null) {
                Plugin.LogDebug("Temporary point found, using instead of saved");
                point = tempPoint;
            }

            // Check if the point exists
            if (point == null) {
                Plugin.LogDebug("Unable to reset player, neither temporary or saved points exist");
                return false;
            }

            Plugin.LogDebug($"Moving player to temporary/saved point");
            MoveTo(point.position, point.rotationX, point.rotationY);
            return true;
        }

        /**
         * <summary>
         * Saves the player's current position and rotation.
         *
         * If the player is in routing flag mode, the point is
         * only saved temporarily.
         *
         * Otherwise, the point is saved to a file permanently.
         * </summary>
         */
        public void SaveState() {
            // If in routing flag mode, save a temporary point
            if (cache.routingFlag.currentlyUsingFlag == true) {
                Plugin.LogDebug("In routing flag mode, saving temporary point");
                SaveTempState();
                return;
            }

            Plugin.LogDebug(
                $"Creating/updating saved point: {position} | {rotationX} | {rotationY}"
            );

            SavedPoint point = config.scene.point;

            // If the config doesn't have a saved point, create one
            // based upon the current state
            if (point == null) {
                Plugin.LogDebug("Creating new config file for point");
                config.scene.point = new SavedPoint(
                    position, rotationX, rotationY
                );
                return;
            }

            // Otherwise, update the state
            Plugin.LogDebug("Updating already existing point");
            point.position = position;
            point.rotationX = rotationX;
            point.rotationY = rotationY;
        }

        /**
         * <summary>
         * Restores the player's position and rotation.
         *
         * If the player is in routing flag mode, the temporary
         * point is used. If the temporary point is unset, the saved
         * point is used.
         *
         * If the player isn't in routing flag mode, only the saved
         * point is used.
         *
         * If both the temporary and saved points are unset, the player isn't moved.
         * </summary>
         * <returns>True if the player was moved, false otherwise</returns>
         */
        public bool RestoreState() {
            // If in routing flag mode, restore the temporary point
            if (cache.routingFlag.currentlyUsingFlag == true) {
                return RestoreTempState();
            }

            SavedPoint point = config.scene.point;

            // If there is no saved point, do nothing
            if (point == null) {
                Plugin.LogDebug("No saved point, not teleporting");
                return false;
            }

            // Otherwise, move to the stored point
            Plugin.LogDebug($"Moving player to saved point");
            MoveTo(point.position, point.rotationX, point.rotationY);
            return true;
        }

        /**
         * <summary>
         * Clears the temporary point when the scene is unloaded.
         * </summary>
         */
        public void OnSceneUnloaded() {
            tempPoint = null;
        }
    }
}
