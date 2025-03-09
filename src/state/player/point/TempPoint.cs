using UnityEngine;

namespace FastReset.State {
    public class TempPoint {
        private bool isSet = false;

        private Vector3 _position;
        private Quaternion _rotationX;
        private float _rotationY;

        public Vector3 position { get => _position; }
        public Quaternion rotationX { get => _rotationX; }
        public float rotationY { get => _rotationY; }

        public bool IsSet() {
            return isSet;
        }

        public void Set(Vector3 position, Quaternion rotationX, float rotationY) {
            Plugin.LogDebug($"TempPoint: setting: {position} | {rotationX} | {rotationY}");

            _position = position;
            _rotationX = rotationX;
            _rotationY = rotationY;

            isSet = true;
        }

        public void Unload() {
            Plugin.LogDebug("TempPoint: Unloading");
            isSet = false;
        }
    }
}
