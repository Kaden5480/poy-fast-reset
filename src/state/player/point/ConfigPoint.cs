using System.IO;

using BepInEx.Configuration;
using UnityEngine;

using Cfg = FastReset.Config.Cfg;
using Paths = FastReset.Config.Paths;

namespace FastReset.State {
    public class ConfigPoint {
        private bool isInitialized = false;
        private bool isSet = false;

        private ConfigFile configFile;

        private ConfigEntry<string> _position;
        private ConfigEntry<string> _rotationX;
        private ConfigEntry<float> _rotationY;

        public Vector3 position {
            get => Cfg.StringToVec3(_position.Value);
        }

        public Quaternion rotationX {
            get => Cfg.StringToQuat(_rotationX.Value);
        }

        public float rotationY {
            get => _rotationY.Value;
        }

        private void BindConfig() {
            Plugin.LogDebug("ConfigPoint: Binding config");

            configFile = new ConfigFile(Paths.playerPath, false);
            _position = configFile.Bind("Point", "position", Cfg.Vec3ToString(Vector3.zero));
            _rotationX = configFile.Bind("Point", "rotationX", Cfg.QuatToString(Quaternion.identity));
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
            _position.Value = Cfg.Vec3ToString(position);
            _rotationX.Value = Cfg.QuatToString(rotationX);
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
