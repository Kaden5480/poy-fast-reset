using UnityEngine;

namespace FastReset.State {
    public class TrackedObject {
        // The ID of this tracked object
        public string id { get; }

        // The object being tracked
        public GameObject obj { get; }

        // Components to track on this object
        public BaseAnimation animation;

        /**
         * <summary>
         * Constructs a new instance of TrackedObject.
         * </summary>
         * <param name="obj">The object to track</param>
         * <param name="useConfig">Whether this object will be saved to a file</param>
         */
        public TrackedObject(GameObject obj, bool useConfig = false) {
            this.obj = obj;
            id = GenerateID(obj);
        }

        /**
         * <summary>
         * Generates an ID for a GameObject.
         * </summary>
         */
        public static string GenerateID(GameObject obj) {
            string tempId = obj.name;

            Transform parent = obj.transform.parent;
            while (parent != null) {
                tempId = $"{parent.gameObject.name}/{tempId}";
                parent = parent.parent;
            }

            // Add on position info
            Vector3 position = obj.transform.position;
            return $"{tempId}-{position.x}_{position.y}_{position.z}";
        }

        public void RestoreState() {
        }

        public void SaveState() {
        }
    }
}
