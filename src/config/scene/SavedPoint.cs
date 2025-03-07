using BepInEx.Configuration;
using UnityEngine;

namespace FastReset.Config {
    /**
     * <summary>
     * A class which is used for saved points.
     * </summary>
     * <see cref="Models.Point">See Models.Point for a temporary point</see>
     */
    public class SavedPoint : Models.BasePoint {
        // The config file this point is contained in
        private ConfigFile configFile;

        // Position to teleport to
        public ConfigEntry<float> _posX;
        public ConfigEntry<float> _posY;
        public ConfigEntry<float> _posZ;

        public override float posX {
            get => _posX.Value;
            set => _posX.Value = value;
        }

        public override float posY {
            get => _posY.Value;
            set => _posY.Value = value;
        }

        public override float posZ {
            get => _posZ.Value;
            set => _posZ.Value = value;
        }

        // Rotation x
        public ConfigEntry<float> _rotY;
        public ConfigEntry<float> _rotW;

        public override float rotY {
            get => _rotY.Value;
            set => _rotY.Value = value;
        }

        public override float rotW {
            get => _rotW.Value;
            set => _rotW.Value = value;
        }

        // Rotation y
        public ConfigEntry<float> _rotationY = null;

        public override float rotationY {
            get => _rotationY.Value;
            set => _rotationY.Value = value;
        }

        /**
         * <summary>
         * Constructs an instance of SavedPoint.
         * </summary>
         * <param name="position">The position of this point</param>
         * <param name="rotationX">The camera x rotation</param>
         * <param name="rotationY">The camera y rotation</param>
         */
        public SavedPoint(
            Vector3? position = null,
            Quaternion? rotationX = null,
            float rotationY = 0f
        ) {
            Vector3 _position = position ?? Vector3.zero;
            Quaternion _rotationX = rotationX ?? Quaternion.identity;

            configFile = new ConfigFile(SceneConfig.pointPath, false);
            _posX = configFile.Bind("Point", "posX", _position.x);
            _posY = configFile.Bind("Point", "posY", _position.y);
            _posZ = configFile.Bind("Point", "posZ", _position.z);
            _rotY = configFile.Bind("Point", "rotY", _rotationX.y);
            _rotW = configFile.Bind("Point", "rotW", _rotationX.w);
            _rotationY = configFile.Bind("Point", "rotationY", rotationY);
        }

        /**
         * <summary>
         * Saves the point to a file.
         * </summary>
         */
        public void Save() {
            configFile.Save();
        }
    }
}
