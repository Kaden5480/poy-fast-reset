using System.IO;

using UnityEngine.SceneManagement;

namespace FastReset.Config {
    public class SceneConfig {
        // Shorthands for accessing cache and scene name
        private static Cache cache {
            get => Plugin.instance.cache;
        }
        private static string sceneName {
            get => cache.scene.name;
        }

        // Path to the file containing the position and
        // rotation for resetting the player
        public static string pointPath {
            get => Path.Combine(
                Cfg.profileDir, sceneName, "point.cfg"
            );
        }

        // The loaded point config for resetting the player
        public SavedPoint point = null;

        /**
         * <summary>
         * Loads per scene configs.
         * </summary>
         */
        public void Load() {
            Plugin.LogDebug($"Loading config for scene {sceneName}");

            // Load if files exist
            if (File.Exists(pointPath) == true) {
                // Constructing a saved point loads the config file
                point = new SavedPoint();
            }
        }

        /**
         * <summary>
         * Saves per scene configs.
         * </summary>
         */
        public void Save() {
            Plugin.LogDebug($"Saving config for scene {sceneName}");

            // Save configs
            if (point != null) {
                point.Save();
            }
        }

        /**
         * <summary>
         * Clears the currently loaded configs.
         * </summary>
         */
        public void Clear() {
            point = null;
        }
    }
}
