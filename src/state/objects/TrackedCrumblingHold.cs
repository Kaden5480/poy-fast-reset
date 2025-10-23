using UnityEngine;

using SaveManager = FastReset.Saves.SaveManager;
using SavedCrumblingHold = FastReset.Saves.SavedCrumblingHold;

namespace FastReset.State {
    /**
     * <summary>
     * An crumbling hold which is tracked so it can
     * be saved/restored.
     * </summary>
     */
    public class TrackedCrumblingHold : BaseTracked {
        private CrumblingHoldRegular hold = null;

        // Initial state
        private bool initialEnabled = false;
        private Vector3 initialScale = Vector3.zero;

        public TrackedCrumblingHold(GameObject obj) : base(obj) {}

        /**
         * <summary>
         * A method to simplify restoring different kinds of states.
         * </summary>
         * <param name="enabled">Whether this hold should be enabled</param>
         * <param name="enabled">The scale to set on this hold</param>
         */
        private void Restore(bool enabled, Vector3 scale) {
            obj.transform.localScale = scale;
            hold.col1.enabled = enabled;
            hold.col2.enabled = enabled;
            hold.meshesHolder.SetActive(enabled);
        }

        /**
         * <summary>
         * A method to simplify saving different kinds of states.
         * </summary>
         * <param name="enabled">Where to store the enabled state of the hold</param>
         * <param name="scale">Where to store the scale of the hold</param>
         */
        private void Save(ref bool enabled, ref Vector3 scale) {
            LogDebug($"Enabled: {enabled}");
            LogDebug($"Scale: {scale}");

            if (hold == null) {
                LogDebug("Hold is null");
            }
            else if (hold.meshesHolder == null) {
                LogDebug("Hold meshes are null");
            }

            enabled = hold.meshesHolder.activeSelf;
            scale = obj.transform.localScale;
        }

#region Initial

        /**
         * <summary>
         * Saves the initial state.
         * </summary>
         */
        public override void SaveInitialState() {
            hold = obj.GetComponent<CrumblingHoldRegular>();

            Save(ref initialEnabled, ref initialScale);
            LogDebug("Saved initial state");
        }

        /**
         * <summary>
         * Restores the initial state.
         * </summary>
         */
        public override void RestoreInitialState() {
            Restore(initialEnabled, initialScale);
            LogDebug("Restored initial state");
        }

#endregion

#region Saved

        /**
         * <summary>
         * Saves the current state to the data store.
         * </summary>
         */
        public override void SaveState() {
            // Add a new crumbling hold to the data store
            SavedCrumblingHold savedHold = new SavedCrumblingHold(byteId);
            SaveManager.Add(savedHold);

            // Update the saved state
            Save(ref savedHold.enabled, ref savedHold.scale);
            LogDebug("Updated state in data store");
        }

        /**
         * <summary>
         * Restores the state stored in the data store.
         * </summary>
         */
        public override void RestoreState() {
            SavedCrumblingHold savedHold = SaveManager.GetCrumblingHold(id);

            if (savedHold == null) {
                LogDebug("No saved state to restore");
                return;
            }

            Restore(savedHold.enabled, savedHold.scale);
            LogDebug("Restored state from data store");
        }

#endregion

    }
}
