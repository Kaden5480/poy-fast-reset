using System.Reflection;

using HarmonyLib;
using UnityEngine;

using SaveManager = FastReset.Saves.SaveManager;
using SavedBrick = FastReset.Saves.SavedBrick;

namespace FastReset.State {
    /**
     * <summary>
     * A BrickHold which is tracked so it can
     * be saved/restored.
     * </summary>
     */
    public class TrackedBrick : BaseTracked {
        private static MethodInfo start = AccessTools.Method(
            typeof(BrickHold), "Start"
        );
        private static FieldInfo playPopout = AccessTools.Field(
            typeof(BrickHold), "playPopout"
        );
        private static FieldInfo poppedOut = AccessTools.Field(
            typeof(BrickHold), "poppedOut"
        );
        private static FieldInfo waitUntilDetach = AccessTools.Field(
            typeof(BrickHold), "waitUntilDetach"
        );

        private BrickHold brickHold;
        private AudioSource brickSound;
        private Collider col;
        private Rigidbody rb;

        // Initial and temporary states
        private bool initialEnabled = false;
        private bool initialKinematic = false;
        private Vector3 initialPosition = Vector3.zero;
        private Quaternion initialRotation = Quaternion.identity;
        private float initialDuration = 0f;

        private bool temporaryEnabled = false;
        private bool temporaryKinematic = false;
        private Vector3 temporaryPosition = Vector3.zero;
        private Quaternion temporaryRotation = Quaternion.identity;
        private float temporaryDuration = 0f;

        public TrackedBrick(GameObject obj) : base(obj) {}

        /**
         * <summary>
         * A method to simplify restoring different kinds of states.
         * </summary>
         * <param name="enabled">Whether the brick should be enabled</param>
         * <param name="kinematic">Whether the brick is kinematic</param>
         * <param name="position">The position to restore to</param>
         * <param name="rotation">The rotation to restore to</param>
         * <param name="duration">The pop out duration to restore to</param>
         */
        private void Restore(
            bool enabled,
            bool kinematic,
            Vector3 position,
            Quaternion rotation,
            float duration
        ) {
            brickHold.StopAllCoroutines();

            poppedOut.SetValue(brickHold, !enabled);

            brickHold.transform.position = position;
            brickHold.transform.rotation = rotation;
            brickHold.popoutDuration = duration;

            brickSound.enabled = enabled;
            col.enabled = enabled;
            rb.isKinematic = kinematic;

            if (enabled == true) {
                playPopout.SetValue(brickHold, false);
                waitUntilDetach.SetValue(brickHold, 0f);

                start.Invoke(brickHold, new object[] {});
            }
        }

        /**
         * <summary>
         * A method to simplify saving different kinds of states.
         * </summary>
         * <param name="enabled">Whether the brick should be enabled</param>
         * <param name="kinematic">Whether the brick is kinematic</param>
         * <param name="position">The position to restore to</param>
         * <param name="rotation">The rotation to restore to</param>
         * <param name="duration">The pop out duration to restore to</param>
         */
        private void Save(
            ref bool enabled,
            ref bool kinematic,
            ref Vector3 position,
            ref Quaternion rotation,
            ref float duration
        ) {
            enabled = !((bool) poppedOut.GetValue(brickHold));
            position = brickHold.transform.position;
            rotation = brickHold.transform.rotation;
            duration = brickHold.popoutDuration;
            kinematic = rb.isKinematic;
        }

#region Initial

        /**
         * <summary>
         * Saves the initial state.
         * </summary>
         */
        public override void SaveInitialState() {
            // Get important objects
            brickHold = obj.GetComponent<BrickHold>();

            brickSound = (AudioSource) AccessTools.Field(
                typeof(BrickHold), "brickSound"
            ).GetValue(brickHold);

            col = (Collider) AccessTools.Field(
                typeof(BrickHold), "col"
            ).GetValue(brickHold);

            rb = (Rigidbody) AccessTools.Field(
                typeof(BrickHold), "rb"
            ).GetValue(brickHold);

            // Save the state
            Save(
                ref initialEnabled, ref initialKinematic,
                ref initialPosition, ref initialRotation,
                ref initialDuration
            );
            LogDebug("Saved initial state");
        }

        /**
         * <summary>
         * Restores the initial state.
         * </summary>
         */
        public override void RestoreInitialState() {
            Restore(
                initialEnabled, initialKinematic,
                initialPosition, initialRotation,
                initialDuration
            );
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
            Save(
                ref temporaryEnabled, ref temporaryKinematic,
                ref temporaryPosition, ref temporaryRotation,
                ref temporaryDuration
            );
            LogDebug("Saved temporary state");
        }

        /**
         * <summary>
         * Restores the routing flag mode state.
         * </summary>
         */
        public override void RestoreTempState() {
            Restore(
                temporaryEnabled, temporaryKinematic,
                temporaryPosition, temporaryRotation,
                temporaryDuration
            );
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
            // Add a new brick to the data store
            SavedBrick savedBrick = new SavedBrick(byteId);
            SaveManager.Add(savedBrick);

            Save(
                ref savedBrick.enabled, ref savedBrick.kinematic,
                ref savedBrick.position, ref savedBrick.rotation,
                ref savedBrick.duration
            );

            LogDebug("Updated state in data store");
        }

        /**
         * <summary>
         * Restores the state stored in the data store.
         * </summary>
         */
        public override void RestoreState() {
            SavedBrick savedBrick = SaveManager.GetBrick(id);

            if (savedBrick == null) {
                LogDebug("No saved state to restore");
                return;
            }

            Restore(
                savedBrick.enabled, savedBrick.kinematic,
                savedBrick.position, savedBrick.rotation,
                savedBrick.duration
            );

            LogDebug("Restored state from data store");
        }

#endregion

    }
}
