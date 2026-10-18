using System.Reflection;
using HarmonyLib;
using UnityEngine;

using SaveManager = FastReset.Saves.SaveManager;
using SavedCustomLoseToolZone = FastReset.Saves.SavedCustomLoseToolZone;

namespace FastReset.State {
    /**
     * <summary>
     * A lose tool zone which is tracked so it can
     * be saved/restored.
     * </summary>
     */
    public class TrackedCustomLoseToolZone : BaseTracked {
        private FieldInfo enteredZone = AccessTools.Field(
            typeof(E_LoseToolZone), "enteredZone"
        );

        private E_LoseToolZone zone = null;
        private Collider collider = null;

        // Initial and temporary states
        private bool initialEnabled = false;
        private bool temporaryEnabled = false;

        public TrackedCustomLoseToolZone(GameObject obj) : base(obj) {}

        /**
         * <summary>
         * A method to simplify restoring different kinds of states.
         * </summary>
         */
        private void Restore(bool enabled) {
            obj.SetActive(enabled);
            collider.enabled = enabled;
            enteredZone.SetValue(zone, !enabled);
        }

        /**
         * <summary>
         * A method to simplify saving different kinds of states.
         * </summary>
         */
        private void Save(ref bool enabled) {
            enabled = collider.enabled;
        }

#region Initial

        /**
         * <summary>
         * Saves the initial state.
         * </summary>
         */
        public override void SaveInitialState() {
            collider = obj.GetComponent<Collider>();
            zone = obj.GetComponent<E_LoseToolZone>();

            Save(ref initialEnabled);
            LogDebug("Saved initial state");
        }

        /**
         * <summary>
         * Restores the initial state.
         * </summary>
         */
        public override void RestoreInitialState() {
            Restore(initialEnabled);
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
            Save(ref temporaryEnabled);
            LogDebug("Saved temporary state");
        }

        /**
         * <summary>
         * Restores the routing flag mode state.
         * </summary>
         */
        public override void RestoreTempState() {
            Restore(temporaryEnabled);
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
            SavedCustomLoseToolZone savedZone = new SavedCustomLoseToolZone(byteId);
            SaveManager.Add(savedZone);

            // Update the saved state
            Save(ref savedZone.enabled);
            LogDebug("Updated state in data store");
        }

        /**
         * <summary>
         * Restores the state stored in the data store.
         * </summary>
         */
        public override void RestoreState() {
            SavedCustomLoseToolZone savedZone = SaveManager.GetCustomLoseToolZone(id);

            if (savedZone == null) {
                LogDebug("No saved state to restore");
                return;
            }

            Restore(savedZone.enabled);
            LogDebug("Restored state from data store");
        }

#endregion

    }
}
