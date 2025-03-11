using System;
using System.IO;

using BepInEx.Configuration;
using UnityEngine;

namespace FastReset.Config {
    /**
     * <summary>
     * A class containing the config for fast reset.
     *
     * This does not include player/scene state as these
     * are handled separately in the data store by SaveManager.
     * </summary>
     */
    public class Cfg {
        // Save/reset keybinds
        public ConfigEntry<KeyCode> saveKeybind;
        public ConfigEntry<KeyCode> resetKeybind;
        public ConfigEntry<KeyCode> toggleModifier;

        // The currently selected profile
        public ConfigEntry<string> profile;

        // Whether to reset the wind on wuthering crest
        public ConfigEntry<bool> resetWind;

        // Whether modifying the saved player/scene states is enabled
        public ConfigEntry<bool> modifyPlayerState;
        public ConfigEntry<bool> modifySceneState;

        // Whether to use the initial scene state when resetting
        public ConfigEntry<bool> useInitialPlayerState;
        public ConfigEntry<bool> useInitialSceneState;
    }
}
