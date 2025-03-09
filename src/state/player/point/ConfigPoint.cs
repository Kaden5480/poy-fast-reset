using System.IO;

using BepInEx.Configuration;
using UnityEngine;

using Paths = FastReset.Config.Paths;

namespace FastReset.State {
    public class ConfigPoint {
        private bool isInitialized = false;
        private bool isSet = false;

        private ConfigFile configFile;
        private ConfigEntry<float> posX;
        private ConfigEntry<float> posY;
        private ConfigEntry<float> posZ;
        private ConfigEntry<float> rotY;
        private ConfigEntry<float> rotW;
        private ConfigEntry<float> _rotationY;

        public Vector3 position {
            get => new Vector3(posX.Value, posY.Value, posZ.Value);
        }

        public Quaternion rotationX {
            get => new Quaternion(0f, rotY.Value, 0f, rotW.Value);
        }

        public float rotationY {
            get => _rotationY.Value;
        }

        private void BindConfig() {
            Plugin.LogDebug("ConfigPoint: Binding config");

            configFile = new ConfigFile(Paths.playerPath, false);
            posX = configFile.Bind("Point", "posX", 0f);
            posY = configFile.Bind("Point", "posY", 0f);
            posZ = configFile.Bind("Point", "posZ", 0f);
            rotY = configFile.Bind("Point", "rotY", 0f);
            rotW = configFile.Bind("Point", "rotW", 0f);
            _rotationY = configFile.Bind("Point", "rotationY", 0f);
        }

        public bool IsSet() {
            return isSet;
        }

        public void Set(Vector3 position, Quaternion rotationX, float rotationY) {
            if (isInitialized == false) {
                BindConfig();
            }

            Plugin.LogDebug($"ConfigPoint: setting: {position} | {rotationX} | {rotationY}");
            posX.Value = position.x;
            posY.Value = position.y;
            posZ.Value = position.z;
            rotY.Value = rotationX.y;
            rotW.Value = rotationX.w;
            _rotationY.Value = rotationY;

            isSet = true;
        }

        public void Load() {
            if (File.Exists(Paths.playerPath) == false) {
                return;
            }

            Plugin.LogDebug("ConfigPoint: Loading saved config");
            BindConfig();

            isInitialized = true;
            isSet = true;
        }

        public void Unload() {
            Plugin.LogDebug("ConfigPoint: Unloading");
            isInitialized = false;
            isSet = false;
        }
    }
}
