namespace FastReset.State {
    public class TrackedObject {
        public string id { get; }
        public GameObject obj { get; }

        public BaseAnimation animation;

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

            GameObject parent = obj.transform.parent;
            while (parent != null) {
                tempId = $"{parent.name}/{tempId}";
                parent = obj.transform.parent;
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
