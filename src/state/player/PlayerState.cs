using UnityEngine;

namespace FastReset.State {
    public class PlayerState : BaseState {
        // Temporary point to reset to
        private TempPoint tempPoint = new TempPoint();

        // Saved point to reset to
        private ConfigPoint configPoint = new ConfigPoint();

        // The player's current position and rotation
        private Vector3 position {
            // The position has to be relative to LeavePeakScene to deal with
            // the origin shifter (specifically on ST)
            get => cache.playerTransform.position
                - cache.leavePeakScene.transform.position;

            set => cache.playerTransform.position = value
                + cache.leavePeakScene.transform.position;
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
         * Moves the player to the provided position with the specified
         * camera rotation.
         * </summary>
         * <param name="position">The position to move to (relative to the LeavePeakScene object)</param>
         * <param name="rotationX">The rotation to apply to camX</param>
         * <param name="rotationY">The rotation to apply to camY</param>
         */
        private void MoveTo(Vector3 position, Quaternion rotationX, float rotationY) {
            Plugin.LogDebug($"PlayerState: Attempting to move player to: {position} (relative) | {rotationX} | {rotationY}");

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

            if (cache.routingFlag.isSolemnTempest == true) {
                cache.routingFlag.distanceActivatorST.ForceCheck();
                Plugin.LogDebug("PlayerState: Force checked ST distance activator");
            }

            // Log actual values
            Plugin.LogDebug(
                $"PlayerState: Moved player to: {position} (real) | {rotationX} | {rotationY}"
            );
        }

        protected override void SaveTempState() {
            Plugin.LogDebug("PlayerState: Saving temporary state");
            tempPoint.Set(position, rotationX, rotationY);
        }

        protected override void SaveConfigState() {
            Plugin.LogDebug("PlayerState: Saving config state");
            configPoint.Set(position, rotationX, rotationY);
        }

        protected override bool HasTempState() {
            return tempPoint.IsSet();
        }

        protected override bool HasConfigState() {
            return configPoint.IsSet();
        }

        protected override void RestoreTempState() {
            Plugin.LogDebug("PlayerState: Restoring temporary state");
            MoveTo(tempPoint.position, tempPoint.rotationX, tempPoint.rotationY);
        }

        protected override void RestoreConfigState() {
            Plugin.LogDebug("PlayerState: Restoring config state");
            MoveTo(configPoint.position, configPoint.rotationX, configPoint.rotationY);
        }

        public override void Load() {
            Plugin.LogDebug("PlayerState: Loading states");
            configPoint.Load();
        }

        public override void Unload() {
            Plugin.LogDebug("PlayerState: Unloading states");
            tempPoint.Unload();
            configPoint.Unload();
        }
    }
}
