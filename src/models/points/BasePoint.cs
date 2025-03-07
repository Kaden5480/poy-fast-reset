using UnityEngine;

namespace FastReset.Models {
    /**
     * <summary>
     * A class which represents a position and camera rotation.
     * Used for making saving/restoring points a bit easier.
     * </summary>
     */
    public abstract class BasePoint {
        // Position
        public abstract float posX { get; set; }
        public abstract float posY { get; set; }
        public abstract float posZ { get; set; }

        // Nice getter/setter for dealing with position more easily
        public Vector3 position {
            get => new Vector3(posX, posY, posZ);
            set {
                posX = value.x;
                posY = value.y;
                posZ = value.z;
            }
        }

        // Rotation x
        public abstract float rotY { get; set; }
        public abstract float rotW { get; set; }

        // Nice getter/setter for dealing with rotationX more easily
        public Quaternion rotationX {
            get => new Quaternion(0f, rotY, 0f, rotW);
            set {
                rotY = value.y;
                rotW = value.w;
            }
        }

        // Rotation y
        public abstract float rotationY { get; set; }
    }
}
