using System;
using System.Collections.Generic;

using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

#if BEPINEX

using BepInEx;

namespace FastReset {
    [BepInPlugin("com.github.Kaden5480.poy-fast-reset", "FastReset", PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin {
        /**
         * <summary>
         * Executes when the plugin is being loaded.
         * </summary>
         */
        public void Awake() {
            config = new Cfg(
                Config.Bind(
                    "Keybinds", "teleport", defaultTeleportKeybind,
                    "Keybind to teleport to saved position"
                ),
                Config.Bind(
                    "Keybinds", "save", defaultSaveKeybind,
                    "Keybind to save the current position and rotation"
                )
            );

            foreach (KeyValuePair<string, float[]> entry in Scenes.defaultPoints) {
                SceneData data = new SceneData(
                    Config.Bind(entry.Key, "posX", entry.Value[0]),
                    Config.Bind(entry.Key, "posY", entry.Value[1]),
                    Config.Bind(entry.Key, "posZ", entry.Value[2]),
                    Config.Bind(entry.Key, "rotY", entry.Value[3]),
                    Config.Bind(entry.Key, "rotW", entry.Value[4]),
                    Config.Bind(entry.Key, "rotationY", entry.Value[5])
                );

                config.AddSceneData(entry.Key, data);
            }

            Harmony.CreateAndPatchAll(typeof(Plugin.PatchResetPosition));

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;

            CommonAwake();
        }

        /**
         * <summary>
         * Executes when the plugin object is destroyed.
         * </summary>
         */
        public void OnDestroy() {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        /*
         * <summary>
         * Executes whenever a scene loads.
         * </summary>
         * <param name="scene">The scene which loaded</param>
         * <param name="mode">The mode the scene was loaded with</param>
         */
        public void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            CommonSceneLoad(scene.buildIndex, scene.name);
        }

        /**
         * <summary>
         * Executes whenever a scene unloads.
         * </summary>
         * <param name="scene">The scene which was unloaded</param>
         */
        public void OnSceneUnloaded(Scene scene) {
            CommonSceneUnload();
        }

        /*
         * <summary>
         * Executes once per frame.
         * </summary>
         */
        public void Update() {
            CommonUpdate();
        }

        /**
         * <summary>
         * Logs a message.
         * </summary>
         * <param name="message">The message to log</param>
         */
        private void LogInfo(string message) {
            Logger.LogInfo(message);
        }

#elif MELONLOADER

using MelonLoader;
using MelonLoader.Utils;

[assembly: MelonInfo(typeof(FastReset.Plugin), "FastReset", PluginInfo.PLUGIN_VERSION, "Kaden5480")]
[assembly: MelonGame("TraipseWare", "Peaks of Yore")]

namespace FastReset {
    public class Plugin: MelonMod {
        /**
         * <summary>
         * Executes when the mod is being loaded.
         * </summary>
         */
        public override void OnInitializeMelon() {
            string filePath = $"{MelonEnvironment.UserDataDirectory}/com.github.Kaden5480.poy-fast-reset.cfg";
            MelonPreferences_Category keybinds = MelonPreferences.CreateCategory("FastReset_Keybinds");
            keybinds.SetFilePath(filePath);

            config = new Cfg(
                keybinds.CreateEntry<string>("teleport", defaultTeleportKeybind),
                keybinds.CreateEntry<string>("save", defaultSaveKeybind)
            );

            foreach (KeyValuePair<string, float[]> entry in Scenes.defaultPoints) {
                MelonPreferences_Category scene = MelonPreferences.CreateCategory($"FastReset_{entry.Key}");
                scene.SetFilePath(filePath);

                SceneData data = new SceneData(
                    scene.CreateEntry<float>("posX", entry.Value[0]),
                    scene.CreateEntry<float>("posY", entry.Value[1]),
                    scene.CreateEntry<float>("posZ", entry.Value[2]),
                    scene.CreateEntry<float>("rotY", entry.Value[3]),
                    scene.CreateEntry<float>("rotW", entry.Value[4]),
                    scene.CreateEntry<float>("rotationY", entry.Value[5])
                );

                config.AddSceneData(entry.Key, data);
            }

            CommonAwake();
        }

        /*
         * <summary>
         * Executes once per frame.
         * </summary>
         */
        public override void OnUpdate() {
            CommonUpdate();
        }

        /*
         * <summary>
         * Executes whenever a scene loads.
         * </summary>
         * <param name="buildIndex">The build index of the scene</param>
         * <param name="sceneName">The name of the scene</param>
         */
        public override void OnSceneWasInitialized(int buildIndex, string sceneName) {
            CommonSceneLoad(buildIndex, sceneName);
        }

        /**
         * <summary>
         * Executes whenever a scene unloads.
         * </summary>
         * <param name="buildIndex">The build index of the scene</param>
         * <param name="sceneName">The name of the scene</param>
         */
        public override void OnSceneWasUnloaded(int buildIndex, string sceneName) {
            CommonSceneUnload();
        }

        /**
         * <summary>
         * Logs a message.
         * </summary>
         * <param name="message">The message to log</param>
         */
        private void LogInfo(string message) {
            MelonLogger.Msg(message);
        }

#endif
        private string defaultTeleportKeybind = KeyCode.F4.ToString();
        private string defaultSaveKeybind = KeyCode.F8.ToString();

        private Cfg config;
        private SceneObjects sceneObjects;

        private int sceneIndex;
        private string sceneName;

        /**
         * <summary>
         * Common code to run on awake.
         * </summary>
         */
        private void CommonAwake() {
            sceneObjects = new SceneObjects();
            PatchResetPosition.plugin = this;
        }

        /**
         * <summary>
         * Common code to run on a scene load.
         * </summary>
         * <param name="buildIndex">The build index of the scene</param>
         * <param name="sceneName">The name of the scene</param>
         */
        private void CommonSceneLoad(int buildIndex, string sceneName) {
            this.sceneIndex = buildIndex;
            this.sceneName = sceneName;

            sceneObjects.LoadObjects();

            if (sceneObjects.routingFlag == null) {
                LogInfo("Routing flag doesn't exist in scene, unable to teleport");
                return;
            }
        }

        /**
         * <summary>
         * Common code to run on a scene unload.
         * </summary>
         */
        private void CommonSceneUnload() {
            sceneIndex = -1;
            sceneName = null;

            sceneObjects.Reset();
        }

        /**
         * <summary>
         * Common code to run on each update.
         * </summary>
         */
        private void CommonUpdate() {
            if (Input.GetKeyDown(config.saveKeybind) == true
                && CanTeleport() == true
            ) {
                Save();
            }

            if (Input.GetKeyDown(config.teleportKeybind) == true
                && CanTeleport() == true
            ) {
                Teleport();
            }
        }

        /**
         * <summary>
         * Checks whether teleporting is enabled on the current scene.
         * </summary>
         * <return>True if teleporting is enabled, false otherwise</return>
         */
        private bool CanTeleport() {
            // Invalid scenes
            if (config.IsValidScene(sceneName) == false) {
                return false;
            }

            return sceneObjects.CanTeleport();
        }

        /**
         * <summary>
         * Saves the current player position and rotation.
         * </summary>
         */
        private void Save() {
            SceneData data = config.GetSceneData(sceneName);

            if (data == null) {
                return;
            }

            if (sceneObjects.menuClick != null) {
                sceneObjects.menuClick.Play();
            }

            if (sceneObjects.isSolemnTempest == true) {
                data.position = sceneObjects.playerTransform.position - sceneObjects.leavePeakScene.transform.position;
            } else {
                data.position = sceneObjects.playerTransform.position;
            }

            data.rotation = sceneObjects.playerCameraHolder.rotation;
            data.rotationY = sceneObjects.camY.rotationY;
        }

        /**
         * <summary>
         * Teleports to the saved position for the current scene.
         * </summary>
         */
        private void Teleport() {
            SceneData data = config.GetSceneData(sceneName);

            if (data == null) {
                return;
            }

            if (sceneObjects.menuClick != null) {
                sceneObjects.menuClick.Play();
            }

            sceneObjects.climbing.ReleaseLHand(false);
            sceneObjects.climbing.ReleaseRHand(false);

            sceneObjects.iceAxes.ReleaseLeft(false);
            sceneObjects.iceAxes.ReleaseRight(false);

            sceneObjects.playerRB.velocity = Vector3.zero;

            sceneObjects.fallingEvent.fellShortDistance = false;
            sceneObjects.fallingEvent.fellLongDistance = false;
            sceneObjects.fallingEvent.fellToDeath = false;

            sceneObjects.routingFlag.usedFlagTeleport = false;

            if (sceneObjects.isSolemnTempest == true) {
                sceneObjects.playerTransform.position = sceneObjects.leavePeakScene.transform.position + data.position;
            } else {
                sceneObjects.playerTransform.position = data.position;
            }

            sceneObjects.playerCameraHolder.rotation = data.rotation;
            sceneObjects.camY.rotationY = data.rotationY;

            if (sceneObjects.isSolemnTempest == true) {
                sceneObjects.distanceActivator.ForceCheck();
            }
        }

        /**
         * <summary>
         * Patches ResetPosition to use fast reset instead.
         * </summary>
         */
        [HarmonyPatch(typeof(ResetPosition), "OnTriggerEnter")]
        static class PatchResetPosition {
            public static Plugin plugin = null;

            static bool Prefix(Collider other) {
                if (plugin == null) {
                    return true;
                }

                if (ResetPosition.resettingPosition == true) {
                    return true;
                }

                if (other.CompareTag("PlayerTrigger") == false) {
                    return true;
                }

                if (plugin.CanTeleport() == false) {
                    return true;
                }

                plugin.Teleport();
                return false;
            }
        }
    }
}
