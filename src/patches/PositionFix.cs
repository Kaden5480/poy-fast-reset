using UnityEngine;

namespace FastReset.Patches {
    public class PositionFix {
        private static Cache cache {
            get => Plugin.instance.cache;
        }

        public static Vector3 RealToOffset(Vector3 real) {
            return real - cache.leavePeakScene.transform.position;
        }

        public static Vector3 OffsetToReal(Vector3 offset) {
            return offset + cache.leavePeakScene.transform.position;
        }
    }
}
