#if BEPINEX
using BepInEx.Configuration;

#elif MELONLOADER
using MelonLoader;

#endif

using UnityEngine;

namespace FastReset {
    public class SceneData {
        public Vector3 position {
            get => new Vector3(posX.Value, posY.Value, posZ.Value);
            set {
                posX.Value = value.x;
                posY.Value = value.y;
                posZ.Value = value.z;
            }
        }

        public Quaternion rotation {
            get => new Quaternion(0f, rotY.Value, 0f, rotW.Value);
            set {
                rotY.Value = value.y;
                rotW.Value = value.w;
            }
        }

        public float rotationY {
            get => _rotationY.Value;
            set {
                _rotationY.Value = value;
            }
        }

#if BEPINEX
        private ConfigEntry<float> posX;
        private ConfigEntry<float> posY;
        private ConfigEntry<float> posZ;
        private ConfigEntry<float> rotY;
        private ConfigEntry<float> rotW;
        private ConfigEntry<float> _rotationY;

        /**
         * <summary>
         * Constructs an instance of SceneData.
         * </summary>
         */
        public SceneData(
            ConfigEntry<float> posX, ConfigEntry<float> posY, ConfigEntry<float> posZ,
            ConfigEntry<float> rotY, ConfigEntry<float> rotW,
            ConfigEntry<float> rotationY
        ) {
            this.posX = posX;
            this.posY = posY;
            this.posZ = posZ;
            this.rotY = rotY;
            this.rotW = rotW;
            this._rotationY = rotationY;
        }

#elif MELONLOADER
        private MelonPreferences_Entry<float> posX;
        private MelonPreferences_Entry<float> posY;
        private MelonPreferences_Entry<float> posZ;
        private MelonPreferences_Entry<float> rotY;
        private MelonPreferences_Entry<float> rotW;
        private MelonPreferences_Entry<float> _rotationY;

        /**
         * <summary>
         * Constructs an instance of SceneData.
         * </summary>
         */
        public SceneData(
            MelonPreferences_Entry<float> posX, MelonPreferences_Entry<float> posY,
            MelonPreferences_Entry<float> posZ,
            MelonPreferences_Entry<float> rotY, MelonPreferences_Entry<float> rotW,
            MelonPreferences_Entry<float> rotationY
        ) {
            this.posX = posX;
            this.posY = posY;
            this.posZ = posZ;
            this.rotY = rotY;
            this.rotW = rotW;
            this._rotationY = rotationY;
        }
#endif
    }
}
