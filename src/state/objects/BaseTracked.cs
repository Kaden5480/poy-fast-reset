using System.Security.Cryptography;
using System.Text;

using UnityEngine;

using SaveManager = FastReset.Saves.SaveManager;

namespace FastReset.State {
    public abstract class BaseTracked {
        // The GameObject which contains the component
        // being tracked
        protected GameObject obj { get; }

        // The ID of this object, a SHA1 digest based upon
        // the object's full path in the object hierarchy
        // and it's position upon loading the scene
        protected byte[] byteId { get; }

        // The ID of this object as a string
        protected string id { get; }

        /**
         * <summary>
         * Helper methods for logging.
         * </summary>
         */
        protected void LogDebug(string message) {
            Plugin.LogDebug($"[{this.GetType()}] {obj.name}: {message}");
        }
        protected void LogInfo(string message) {
            Plugin.LogInfo($"[{this.GetType()}] {obj.name}: {message}");
        }
        protected void LogError(string message) {
            Plugin.LogError($"[{this.GetType()}] {obj.name}: {message}");
        }

        /**
         * <summary>
         * Methods which are used for saving/restoring the
         * initial state of this object.
         * </summary>
         */
        public virtual void SaveInitialState() {
            LogError("SaveInitialState not implemented");
        }
        public virtual void RestoreInitialState() {
            LogError("RestoreInitialState not implemented");
        }

        /**
         * <summary>
         * Methods which are used for saving/restoring
         * state from fast reset's data store.
         * </summary>
         */
        public virtual void SaveState() {
            LogError("SaveState not implemented");
        }
        public virtual void RestoreState() {
            LogError("RestoreState not implemented");
        }

        /**
         * <summary>
         * Computes the SHA1 digest of a given string.
         * Used as the ID of the tracked object.
         * </summary>
         * <param name="id">The ID to compute the SHA1 digest of</param>
         * <returns>The SHA1 digest</returns>
         */
        public byte[] SHA1(string id) {
            SHA1Managed sha1 = new SHA1Managed();
            return sha1.ComputeHash(Encoding.UTF8.GetBytes(id));
        }

        /**
         * <summary>
         * Constructs a BaseTracked.
         * </summary>
         * <param name="obj">The object being tracked</param>
         */
        public BaseTracked(GameObject obj) {
            this.obj = obj;

            // Compute the full path in the object hierarchy
            string tempId = obj.name;
            Transform parent = obj.transform.parent;
            while (parent != null) {
                tempId = $"{parent.gameObject.name}/{tempId}";
                parent = parent.parent;
            }

            // Add on position info
            Vector3 position = obj.transform.position;
            tempId = $"{tempId}-{position.x}_{position.y}_{position.z}";

            // Compute the SHA1 digest
            byteId = SHA1(tempId);
            id = SaveManager.BytesToString(byteId);

            LogDebug($"Tracking object {tempId} as {id}");

            // Make sure to save this object's initial state
            SaveInitialState();
        }
    }
}
