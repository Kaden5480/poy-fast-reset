using System.Reflection;
using HarmonyLib;
using UnityEngine;

using SaveManager = FastReset.Saves.SaveManager;
using SavedCustomCollectible = FastReset.Saves.SavedCustomCollectible;

namespace FastReset.State {
    /**
     * <summary>
     * A custom collectible which is tracked so it can
     * be saved/restored.
     * </summary>
     */
    public class TrackedCustomCollectible : BaseTracked {
        private GameObject parent = null;
        private Collider collider = null;
        private Vector3 position = Vector3.zero;
        private Quaternion rotation = Quaternion.identity;

        // Initial and temporary states
        private bool initialEnabled = false;
        private bool temporaryEnabled = false;

        public TrackedCustomCollectible(GameObject obj) : base(obj) {}

        /**
         * <summary>
         * A method to simplify restoring different kinds of states.
         * </summary>
         */
        private void Restore(bool enabled) {
            foreach (Renderer renderer in obj.GetComponentsInChildren<Renderer>()) {
                renderer.enabled = enabled;
            }

            obj.transform.SetParent(parent.transform);
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            collider.enabled = enabled;

            obj.SetActive(enabled);
        }

        /**
         * <summary>
         * A method to simplify saving different kinds of states.
         * </summary>
         */
        private void Save(ref bool enabled) {
            enabled = obj.activeSelf;
        }

#region Initial

        /**
         * <summary>
         * Saves the initial state.
         * </summary>
         */
        public override void SaveInitialState() {
            parent = obj.transform.parent.gameObject;
            collider = obj.GetComponent<Collider>();
            position = obj.transform.position;
            rotation = obj.transform.rotation;

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
            SavedCustomCollectible savedCollectible = new SavedCustomCollectible(byteId);
            SaveManager.Add(savedCollectible);

            // Update the saved state
            Save(ref savedCollectible.enabled);
            LogDebug("Updated state in data store");
        }

        /**
         * <summary>
         * Restores the state stored in the data store.
         * </summary>
         */
        public override void RestoreState() {
            SavedCustomCollectible savedCollectible = SaveManager.GetCustomCollectible(id);

            if (savedCollectible == null) {
                LogDebug("No saved state to restore");
                return;
            }

            Restore(savedCollectible.enabled);
            LogDebug("Restored state from data store");
        }

#endregion

    }
}
