using UnityEngine;

using PositionFix = FastReset.Patches.PositionFix;
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

        // Initial state
        private Vector3 initialPosition;
        private Quaternion initialRotation;

        public TrackedJoint(GameObject obj) : base(obj) {}

        /**
         * <summary>
         * A method to simplify restoring different kinds of states.
         * </summary>
         * <param name="position">The position to restore</param>
         * <param name="rotation">The rotation to restore</param>
         */
        private void Restore(Vector3 position, Quaternion rotation) {
            obj.transform.position = PositionFix.OffsetToReal(position);
            obj.transform.rotation = rotation;

            jointRb.angularVelocity = Vector3.zero;
            jointRb.velocity = Vector3.zero;
        }

        /**
         * <summary>
         * A method to simplify saving different kinds of states.
         * </summary>
         * <param name="position">The position to save to</param>
         * <param name="rotation">The rotation to save to</param>
         */
        private void Save(ref Vector3 position, ref Quaternion rotation) {
            position = PositionFix.RealToOffset(obj.transform.position);
            rotation = obj.transform.rotation;
        }

#region Initial

        /**
         * <summary>
         * Saves the initial state.
         * </summary>
         */
        public override void SaveInitialState() {
            jointRb = obj.GetComponent<Rigidbody>();

            Save(ref initialPosition, ref initialRotation);
            LogDebug("Saved initial state");
        }

        /**
         * <summary>
         * Restores the initial state.
         * </summary>
         */
        public override void RestoreInitialState() {
            Restore(initialPosition, initialRotation);
            LogDebug("Restored initial state");
        }

#endregion

#region Saved

        /**
         * <summary>
         * Saves the current state to the data store.
         * </summary>
         * <param name="save">The save data to save to</param>
         */
        public override void SaveState(SaveData save) {
            // Add a new joint to the data store
            SavedJoint savedJoint = new SavedJoint(byteId);
            save.Add(savedJoint);

            // Update the position and rotation
            Save(ref savedJoint.position, ref savedJoint.rotation);
            LogDebug("Updated state in data store");
        }

        /**
         * <summary>
         * Restores the state stored in the data store.
         * </summary>
         * <param name="save">The save data to restore from</param>
         */
        public override void RestoreState(SaveData save) {
            SavedJoint savedJoint = save.GetJoint(id);

            if (savedJoint == null) {
                LogDebug("No saved state to restore");
                return;
            }

            Restore(savedJoint.position, savedJoint.rotation);
            LogDebug("Restored state from data store");
        }

#endregion

    }
}
