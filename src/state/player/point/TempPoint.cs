using UnityEngine;

namespace FastReset.State {
    public class TempPoint {
        private bool isSet = false;

        private float posX;
        private float posY;
        private float posZ;
        private float rotY;
        private float rotW;
        private float _rotationY;

        public Vector3 position {
            get => new Vector3(posX, posY, posZ);
        }

        public Quaternion rotationX {
            get => new Quaternion(0f, rotY, 0f, rotW);
        }

        public float rotationY {
            get => _rotationY;
        }

        public bool IsSet() {
            return isSet;
        }

        public void Set(Vector3 position, Quaternion rotationX, float rotationY) {
            Plugin.LogDebug($"TempPoint: setting: {position} | {rotationX} | {rotationY}");

            posX = position.x;
            posY = position.y;
            posZ = position.z;
            rotY = rotationX.y;
            rotW = rotationX.w;
            _rotationY = rotationY;

            isSet = true;
        }

        public void Unload() {
            Plugin.LogDebug("TempPoint: Unloading");
            isSet = false;
        }
    }
}
