using System.Collections.Generic;

using UnityEngine;

using PositionFix = FastReset.Patches.PositionFix;
using SaveData = FastReset.Saves.SaveData;
using SaveManager = FastReset.Saves.SaveManager;
using SavedBrittleIce = FastReset.Saves.SavedBrittleIce;

namespace FastReset.State {
    /**
     * <summary>
     * An animation which is tracked so it can
     * be saved/restored.
     * </summary>
     */
    public class TrackedBrittleIce : BaseTracked {
        private BrittleIce brittleIce;
        private Rigidbody[] rigidBodies;
        private Renderer[] renderers;

        // Initial state
        private int initialHp = 0;
        private List<float> initialMatStates = new List<float>();
        private List<Vector3> initialPositions = new List<Vector3>();
        private List<Quaternion> initialRotations = new List<Quaternion>();

        public TrackedBrittleIce(GameObject obj) : base(obj) {}

        /**
         * <summary>
         * A method to simplify restoring different kinds of states.
         * </summary>
         */
        private void Restore(
            int hp,
            List<float> matStates,
            List<Vector3> positions,
            List<Quaternion> rotations
        ) {
            // Prevent anything from running
            brittleIce.StopAllCoroutines();

            brittleIce.iceHP = hp;
            bool enabled = hp > 0;

            // Update material states
            foreach (Renderer renderer in renderers) {
                Material mat = renderer.material;

                mat.SetFloat("_IceCracksStrength", matStates[0]);
                mat.SetFloat("_IceStrength1", matStates[1]);
                mat.SetFloat("_IceStrength2", matStates[2]);
                mat.SetFloat("_IceCracksScale", matStates[3]);
            }

            // Restore rigidbody states
            for (int i = 0; i < rigidBodies.Length; i++) {
                Rigidbody rb = rigidBodies[i];

                rb.isKinematic = enabled;
                rb.gameObject.SetActive(enabled);

                // Enable/disable any colliders
                Collider[] colliders = rb.gameObject.GetComponents<Collider>();
                foreach (Collider collider in colliders) {
                    collider.enabled = enabled;
                }

                // Restore positions and rotations
                rb.transform.position = PositionFix.OffsetToReal(positions[i]);
                rb.transform.rotation = rotations[i];
            }

            // Set the component to enabled/disabled
            brittleIce.enabled = enabled;
        }

        /**
         * <summary>
         * A method to simplify saving different kinds of states.
         * </summary>
         */
        private void Save(
            ref int hp,
            List<float> matStates,
            List<Vector3> positions,
            List<Quaternion> rotations
        ) {
            hp = brittleIce.iceHP;

            // Only need to read the first renderer
            matStates.Clear();
            foreach (Renderer renderer in renderers) {
                Material mat = renderer.material;

                matStates.Add(mat.GetFloat("_IceCracksStrength"));
                matStates.Add(mat.GetFloat("_IceStrength1"));
                matStates.Add(mat.GetFloat("_IceStrength2"));
                matStates.Add(mat.GetFloat("_IceCracksStrength"));

                break;
            }

            // Save positions and rotations
            positions.Clear();
            rotations.Clear();
            foreach (Rigidbody rb in rigidBodies) {
                positions.Add(PositionFix.RealToOffset(rb.transform.position));
                rotations.Add(rb.transform.rotation);
            }
        }

#region Initial

        /**
         * <summary>
         * Saves the initial state.
         * </summary>
         */
        public override void SaveInitialState() {
            brittleIce = obj.GetComponent<BrittleIce>();
            rigidBodies = obj.GetComponentsInChildren<Rigidbody>();
            renderers = obj.GetComponentsInChildren<Renderer>();

            Save(ref initialHp, initialMatStates, initialPositions, initialRotations);
            LogDebug("Saved initial state");
        }

        /**
         * <summary>
         * Restores the initial state.
         * </summary>
         */
        public override void RestoreInitialState() {
            Restore(initialHp, initialMatStates, initialPositions, initialRotations);
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
            // Add a new brittle ice to the data store
            SavedBrittleIce savedBrittleIce = new SavedBrittleIce(byteId);
            save.Add(savedBrittleIce);

            // Update the saved state
            Save(
                ref savedBrittleIce.hp,
                savedBrittleIce.matStates,
                savedBrittleIce.positions,
                savedBrittleIce.rotations
            );
            LogDebug("Updated state in data store");
        }

        /**
         * <summary>
         * Restores the state stored in the data store.
         * </summary>
         * <param name="save">The save data to restore from</param>
         */
        public override void RestoreState(SaveData save) {
            SavedBrittleIce savedBrittleIce = save.GetBrittleIce(id);

            if (savedBrittleIce == null) {
                LogDebug("No saved state to restore");
                return;
            }

            Restore(
                savedBrittleIce.hp,
                savedBrittleIce.matStates,
                savedBrittleIce.positions,
                savedBrittleIce.rotations
            );
            LogDebug("Restored state from data store");
        }

#endregion

    }
}
