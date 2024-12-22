using System.Collections.Generic;

#if BEPINEX
using BepInEx.Configuration;

#elif MELONLOADER
using MelonLoader;

#endif

using UnityEngine;

namespace FastReset {
    public class Cfg {
        private Dictionary<string, SceneData> points = new Dictionary<string, SceneData>();

        public KeyCode teleportKeybind {
            get => (KeyCode) System.Enum.Parse(typeof(KeyCode), configTeleportKeybind.Value);
            set {
                configTeleportKeybind.Value = value.ToString();
            }
        }

        public KeyCode saveKeybind {
            get => (KeyCode) System.Enum.Parse(typeof(KeyCode), configSaveKeybind.Value);
            set {
                configSaveKeybind.Value = value.ToString();
            }
        }

#if BEPINEX
        private ConfigEntry<string> configTeleportKeybind;
        private ConfigEntry<string> configSaveKeybind;

        /**
         * <summary>
         * Constructs an instance of Cfg.
         * </summary>
         * <param name="configTeleportKeybind">The entry for the teleport keybind</param>
         * <param name="configSaveKeybind">The entry for the save keybind</param>
         */
        public Cfg(
            ConfigEntry<string> configTeleportKeybind,
            ConfigEntry<string> configSaveKeybind
        ) {
            this.configTeleportKeybind = configTeleportKeybind;
            this.configSaveKeybind = configSaveKeybind;
        }

#elif MELONLOADER
        private MelonPreferences_Entry<string> configTeleportKeybind;
        private MelonPreferences_Entry<string> configSaveKeybind;

        /**
         * <summary>
         * Constructs an instance of Cfg.
         * </summary>
         * <param name="configTeleportKeybind">The entry for the teleport keybind</param>
         * <param name="configSaveKeybind">The entry for the save keybind</param>
         */
        public Cfg(
            MelonPreferences_Entry<string> configTeleportKeybind,
            MelonPreferences_Entry<string> configSaveKeybind
        ) {
            this.configTeleportKeybind = configTeleportKeybind;
            this.configSaveKeybind = configSaveKeybind;
        }

#endif

        /**
         * <summary>
         * Adds scene data for a specified scene.
         * </summary>
         * <param name="name">The name of the scene</param>
         * <param name="data">The data for this scene</param>
         */
        public void AddSceneData(string name, SceneData data) {
            points.Add(name, data);
        }

        /**
         * <summary>
         * Checks if a scene can be teleported in.
         * </summary>
         * <param name="name">The name of the scene</param>
         * <return>True if the scene can be teleported in, false otherwise</return>
         */
        public bool IsValidScene(string name) {
            if (name == null) {
                return false;
            }

            return points.ContainsKey(name);
        }

        /**
         * <summary>
         * Gets the scene data for a given scene.
         * </summary>
         * <param name="name">The name of the scene</param>
         * <return>The scene data for this scene, null otherwise</return>
         */
        public SceneData GetSceneData(string name) {
            SceneData data = null;

            if (name == null) {
                return null;
            }

            if (points.TryGetValue(name, out data) == false) {
                return null;
            }

            return data;
        }
    }
}
