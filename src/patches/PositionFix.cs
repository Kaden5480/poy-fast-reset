using UnityEngine;

namespace FastReset.Patches {
    /**
     * <summary>
     * A class with some helper methods to deal with
     * the custom origin shifter.
     * </summary>
     */
    public class PositionFix {
        private static Cache cache {
            get => Plugin.instance.cache;
        }

        /**
         * <summary>
         * Converts a "real" position to an offset
         * from the LeavePeakScene object.
         * </summary>
         * <param name="real">The object's current position</param>
         * <returns>The offset from LeavePeakScene</returns>
         */
        public static Vector3 RealToOffset(Vector3 real) {
            return real - cache.leavePeakScene.transform.position;
        }

        /**
         * <summary>
         * Converts an offset from the LeavePeakScene object
         * to a "real" position.
         * </summary>
         * <param name="offset">The offset from the LeavePeakScene object</param>
         * <returns>The "real" position</returns>
         */
        public static Vector3 OffsetToReal(Vector3 offset) {
            return offset + cache.leavePeakScene.transform.position;
        }
    }
}
