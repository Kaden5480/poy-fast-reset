using System.IO;

using BepInEx.Configuration;
using UnityEngine;

namespace FastReset.Config {
    public class Cfg {
        // Save/reset keybinds
        public ConfigEntry<KeyCode> saveKeybind;
        public ConfigEntry<KeyCode> resetKeybind;

        // The currently selected profile
        public ConfigEntry<string> profile;
    }
}
