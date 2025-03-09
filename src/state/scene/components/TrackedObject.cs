using UnityEngine;

namespace FastReset.State {
    public abstract class TrackedObject {
        protected GameObject obj { get; }
        protected string id { get; }

        public abstract void RestoreInitialState();
        public abstract void RestoreTempState();
        public abstract void RestoreConfigState();

        protected abstract void SaveInitialState();
        public abstract void SaveTempState();
        public abstract void SaveConfigState();

        protected abstract void BindConfig();

        public TrackedObject(GameObject obj) {
            Plugin.LogDebug($"TrackedObject: Creating tracked object for: {obj.name}");
            this.obj = obj;

            string tempId = obj.name;

            Transform parent = obj.transform.parent;
            while (parent != null) {
                tempId = $"{parent.gameObject.name}/{tempId}";
                parent = parent.parent;
            }

            // Add on position info
            Vector3 position = obj.transform.position;
            id = $"{tempId}-{position.x}_{position.y}_{position.z}";

            SaveInitialState();
            BindConfig();
        }
    }
}
