using UnityEngine;

using SaveManager = FastReset.Saves.SaveManager;
using SavedJoint = FastReset.Saves.SavedJoint;

namespace FastReset.State {
    /**
     * <summary>
     * A ConfigurableJoint which is tracked so it can
     * be saved/restored.
     * </summary>
     */
    public class TrackedJoint : BaseTracked {
        private Rigidbody jointRb = null;

        // Initial and temporary states
        private Quaternion initialRotation;
        private Quaternion temporaryRotation;

        public TrackedJoint(GameObject obj) : base(obj) {}

        /**
         * <summary>
         * A method to simplify restoring different kinds of states.
         * </summary>
         * <param name="rotation">The rotation to restore</param>
         */
        private void Restore(Quaternion rotation) {
            jointRb.angularVelocity = Vector3.zero;
            jointRb.velocity = Vector3.zero;

            obj.transform.rotation = rotation;
        }

#region Initial

        /**
         * <summary>
         * Saves the initial state.
         * </summary>
         */
        public override void SaveInitialState() {
            jointRb = obj.GetComponent<Rigidbody>();

            initialRotation = obj.transform.rotation;
            LogDebug("Saved initial state");
        }

        /**
         * <summary>
         * Restores the initial state.
         * </summary>
         */
        public override void RestoreInitialState() {
            Restore(initialRotation);
            LogDebug("Restored initial state");
        }

#endregion

#region Temporary

        /**
         * <summary>
         * Saves the current state temporarily (routing flag mode).
         * </summary>
         */
        public override void SaveTempState() {
            temporaryRotation = obj.transform.rotation;
            LogDebug("Saved temporary state");
        }

        /**
         * <summary>
         * Restores the routing flag mode state.
         * </summary>
         */
        public override void RestoreTempState() {
            Restore(temporaryRotation);
            LogDebug("Restored temporary state");
        }

#endregion

#region Saved

        /**
         * <summary>
         * Saves the current state to the data store.
         * </summary>
         */
        public override void SaveState() {
            SavedJoint savedJoint = SaveManager.GetJoint(id);

            // Add a new joint to the data store
            if (savedJoint == null) {
                savedJoint = new SavedJoint(byteId);
                SaveManager.Add(savedJoint);
            }

            // Update the rotation
            savedJoint.rotation = obj.transform.rotation;
            LogDebug("Updated state in data store");
        }

        /**
         * <summary>
         * Restores the state stored in the data store.
         * </summary>
         */
        public override void RestoreState() {
            SavedJoint savedJoint = SaveManager.GetJoint(id);

            if (savedJoint == null) {
                LogDebug("No saved state to restore");
                return;
            }

            Restore(savedJoint.rotation);
            LogDebug("Restored state from data store");
        }

#endregion

    }
}
