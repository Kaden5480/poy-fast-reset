using System.Security.Cryptography;
using System.Text;

using UnityEngine;

namespace FastReset.State {
    public abstract class TrackedObject {
        protected bool boundConfig = false;

        protected GameObject obj { get; }
        protected string id { get; }

        public abstract void RestoreInitialState();
        public abstract void RestoreTempState();
        public abstract void RestoreConfigState();

        protected abstract void SaveInitialState();
        public abstract void SaveTempState();
        public abstract void SaveConfigState();

        protected abstract void BindConfig();

        public void RebindConfig() {
            boundConfig = false;
            BindConfig();
        }

        private string SHA1(string id) {
            SHA1Managed sha1 = new SHA1Managed();
            StringBuilder hash = new StringBuilder();

            foreach (byte b in sha1.ComputeHash(Encoding.UTF8.GetBytes(id))) {
                hash.Append(b.ToString("x2"));
            }

            return hash.ToString();
        }

        public TrackedObject(GameObject obj) {
            this.obj = obj;

            string tempId = obj.name;

            Transform parent = obj.transform.parent;
            while (parent != null) {
                tempId = $"{parent.gameObject.name}/{tempId}";
                parent = parent.parent;
            }

            // Add on position info
            Vector3 position = obj.transform.position;

            tempId = $"{tempId}-{position.x}_{position.y}_{position.z}";
            id = SHA1(tempId);

            Plugin.LogDebug($"TrackedObject: Tracking object {tempId} as {id}");

            SaveInitialState();
            BindConfig();
        }
    }
}
