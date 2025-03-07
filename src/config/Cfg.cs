using System.IO;

using BepInEx.Configuration;
using UnityEngine;

namespace FastReset.Config {
    public class Cfg {
        // The directory where configs besides the main one will be located
        public static string configDirName { get; } = "com.github.Kaden5480.poy-fast-reset";

        // The directory for the currently selected profile
        public static string profileDir {
            get => Path.Combine(
                BepInEx.Paths.ConfigPath, configDirName,
                Plugin.instance.config.profile.Value
            );
        }

        // Save/reset keybinds
        public ConfigEntry<KeyCode> saveKeybind;
        public ConfigEntry<KeyCode> resetKeybind;

        // The currently selected profile
        public ConfigEntry<string> profile;

        // Config for the currently loaded scene
        public SceneConfig scene = new SceneConfig();
    }
}
