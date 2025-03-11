using System;
using System.Collections;

using BepInEx;
using PeterO.Cbor;
using UnityEngine;
using UnityEngine.SceneManagement;


using Cfg = FastReset.Config.Cfg;
using SaveManager = FastReset.Saves.SaveManager;

namespace FastReset {
    [BepInPlugin("com.github.Kaden5480.poy-fast-reset", "Fast Reset", PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin {
        // An instance of this object accessible statically
        public static Plugin instance = null;

        public Cfg config { get; } = new Cfg();
        public Audio audio { get; } = new Audio();
        public Cache cache { get; } = new Cache();
        public Patcher patcher { get; } = new Patcher();
        public Resetter resetter { get; } = new Resetter();
        public SaveManager saveManager { get; } = new SaveManager();
        public UI.Window ui { get; } = new UI.Window();

        // Default keybinds
        private const KeyCode defaultSaveKeybind = KeyCode.F8;
        private const KeyCode defaultResetKeybind = KeyCode.F4;
        private const KeyCode defaultToggleModifier = KeyCode.LeftShift;

        /**
         * <summary>
         * Executes when the plugin is being loaded.
         * </summary>
         */
        public void Awake() {
            instance = this;

            config.saveKeybind = Config.Bind(
                "General", "saveKeybind", defaultSaveKeybind,
                "The keybind for saving the current player and/or scene state"
            );
            config.resetKeybind = Config.Bind(
                "General", "resetKeybind", defaultResetKeybind,
                "The keybind to restore your saved player and scene state"
            );
            config.toggleModifier = Config.Bind(
                "General", "toggleModifier", defaultToggleModifier,
                "The key to press along with `saveKeybind` to open the UI"
            );
            config.profile = Config.Bind(
                "General", "profile", "Default",
                "The current profile in use"
            );
            config.resetWind = Config.Bind(
                "General", "resetWind", false,
                "Whether to reset the wind on wuthering crest"
            );
            config.modifyPlayerState = Config.Bind(
                "State", "modifyPlayerState", true,
                "Whether to modify the player state when saving"
            );
            config.modifySceneState = Config.Bind(
                "State", "modifySceneState", true,
                "Whether to modify the scene state when saving"
            );
            config.useInitialPlayerState = Config.Bind(
                "State", "useInitialPlayerState", false,
                "Whether to reset to the player's spawn point instead"
            );
            config.useInitialSceneState = Config.Bind(
                "State", "useInitialSceneState", false,
                "Whether to reset the scene's initial state instead"
            );

            // Apply early patches
            patcher.PatchEarly();

            // Track scene changes
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        /**
         * <summary>
         * Executes when the plugin is destroyed.
         * </summary>
         */
        public void OnDestroy() {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        /**
         * <summary>
         * Executes each frame.
         * </summary>
         */
        public void Update() {
            ui.Update();

            if (Input.GetKeyDown(config.saveKeybind.Value) == true) {
                Plugin.LogDebug($"Plugin: {config.saveKeybind.Value} is down");
                if (Input.GetKey(config.toggleModifier.Value) == true) {
                    Plugin.LogDebug($"Plugin: {config.toggleModifier.Value} is down");
                    ui.Toggle();
                }
                else {
                    resetter.SaveState();
                }
            }

            if (Input.GetKeyDown(config.resetKeybind.Value) == true) {
                Plugin.LogDebug($"Plugin: {config.resetKeybind.Value} is down");
                resetter.RestoreState();
            }
        }

        /**
         * <summary>
         * Executes to render the UI.
         * </summary>
         */
        public void OnGUI() {
            ui.Render();
        }

        /**
         * <summary>
         * Executes when a scene is loaded.
         * </summary>
         * <param name="scene">The scene which loaded</param>
         * <param name="mode">The mode the scene loaded with</param>
         */
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            // Make sure the cache is loaded first
            cache.FindObjects();

            // Load the config for this scene
            saveManager.Load();

            // Save the initial state of the scene
            resetter.SceneLoad();
        }

        /**
         * <summary>
         * Executes when a scene is unloaded.
         * </summary>
         * <param name="scene">The scene which unloaded</param>
         */
        private void OnSceneUnloaded(Scene scene) {
            // Save state to the disk
            saveManager.Save();

            // Wipe the tracked state
            resetter.SceneUnload();

            // Wipe the cache last
            cache.Clear();
        }

        /**
         * <summary>
         * Logs a debug message.
         * </summary>
         * <param name="message">The message to log</param>
         */
        public static void LogDebug(string message) {
#if DEBUG
            if (instance == null) {
                Console.WriteLine($"[Debug] FastReset: {message}");
                return;
            }

            instance.Logger.LogInfo(message);
#else
            if (instance != null) {
                instance.Logger.LogDebug(message);
            }
#endif
        }

        /**
         * <summary>
         * Logs an informational message.
         * </summary>
         * <param name="message">The message to log</param>
         */
        public static void LogInfo(string message) {
            if (instance == null) {
                Console.WriteLine($"[Info] FastReset: {message}");
                return;
            }
            instance.Logger.LogInfo(message);
        }

        /**
         * <summary>
         * Logs an error message.
         * </summary>
         * <param name="message">The message to log</param>
         */
        public static void LogError(string message) {
            if (instance == null) {
                Console.WriteLine($"[Error] FastReset: {message}");
                return;
            }
            instance.Logger.LogError(message);
        }
    }
}
