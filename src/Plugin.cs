using System;
using System.Collections.Generic;
using System.Reflection;

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
                    Config.Bind(entry.Key, "rotX", entry.Value[3]),
                    Config.Bind(entry.Key, "rotY", entry.Value[4]),
                    Config.Bind(entry.Key, "rotZ", entry.Value[5]),
                    Config.Bind(entry.Key, "rotW", entry.Value[6]),
                    Config.Bind(entry.Key, "rotationY", entry.Value[7])
                );

                config.AddSceneData(entry.Key, data);
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        /**
         * <summary>
         * Executes when the plugin object is destroyed.
         * </summary>
         */
        public void OnDestroy() {
            SceneManager.sceneLoaded -= OnSceneLoaded;
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
            MelonPreferences_Category keybinds = MelonPreferences.CreateCategory("Keybinds");
            keybinds.SetFilePath(filePath);

            config = new Cfg(
                keybinds.CreateEntry<string>("teleport", defaultTeleportKeybind),
                keybinds.CreateEntry<string>("save", defaultSaveKeybind)
            );

            foreach (KeyValuePair<string, float[]> entry in Scenes.defaultPoints) {
                MelonPreferences_Category scene = MelonPreferences.CreateCategory(entry.Key);
                scene.SetFilePath(filePath);

                SceneData data = new SceneData(
                    scene.CreateEntry<float>("posX", entry.Value[0]),
                    scene.CreateEntry<float>("posY", entry.Value[1]),
                    scene.CreateEntry<float>("posZ", entry.Value[2]),
                    scene.CreateEntry<float>("rotX", entry.Value[3]),
                    scene.CreateEntry<float>("rotY", entry.Value[4]),
                    scene.CreateEntry<float>("rotZ", entry.Value[5]),
                    scene.CreateEntry<float>("rotW", entry.Value[6]),
                    scene.CreateEntry<float>("rotationY", entry.Value[7])
                );

                config.AddSceneData(entry.Key, data);
            }
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

        private int sceneIndex;
        private string sceneName;

        private AudioSource menuClick;

        private Barometer barometer;
        private CameraLook camY;
        private Climbing climbing;
        private FallingEvent fallingEvent;
        private IceAxe iceAxes;
        private Rigidbody playerRB;
        private RopeAnchor ropeAnchor;
        private RoutingFlag routingFlag;
        private PlayerMove playerMove;
        private Transform playerTransform;
        private Transform playerCameraHolder;

        private LeavePeakScene leavePeakScene;
        private bool isSolemnTempest;
        private DistanceActivator distanceActivator;

        /**
         * <summary>
         * Gets a private instance field from the routing flag.
         * </summary>
         * <param name="flag">An instance of the routing flag</param>
         * <param name="fieldName">The name of the field to get</param>
         */
        private T GetField<T>(RoutingFlag flag, string fieldName) {
            return (T) typeof(RoutingFlag).GetField(fieldName,
                BindingFlags.NonPublic | BindingFlags.Instance
            ).GetValue(flag);
        }

        /**
         * <summary>
         * Resets fields to default states.
         * </summary>
         */
        private void Reset() {
            sceneIndex = -1;
            sceneName = null;

            menuClick = null;

            barometer = null;
            camY = null;
            iceAxes = null;
            climbing = null;
            fallingEvent = null;
            playerRB = null;
            ropeAnchor = null;
            routingFlag = null;
            playerMove = null;
            playerTransform = null;
            playerCameraHolder = null;

            leavePeakScene = null;
            isSolemnTempest = false;
            distanceActivator = null;
        }

        /**
         * <summary>
         * Common code to run on a scene load.
         * </summary>
         * <param name="buildIndex">The build index of the scene</param>
         * <param name="sceneName">The name of the scene</param>
         */
        private void CommonSceneLoad(int buildIndex, string sceneName) {
            Barometer[] barometers = GameObject.FindObjectsOfType<Barometer>();
            InGameMenu[] menus = GameObject.FindObjectsOfType<InGameMenu>();
            LeavePeakScene[] leavePeakScenes = GameObject.FindObjectsOfType<LeavePeakScene>();
            RoutingFlag[] flags = GameObject.FindObjectsOfType<RoutingFlag>();
            RoutingFlag flag;

            Reset();

            this.sceneIndex = buildIndex;
            this.sceneName = sceneName;

            if (barometers.Length >= 1) {
                barometer = barometers[0];
            }

            if (menus.Length >= 1) {
                menuClick = menus[0].menuClick;
            }

            if (leavePeakScenes.Length >= 1) {
                leavePeakScene = leavePeakScenes[0];
            }

            if (flags.Length < 1) {
                LogInfo("Routing flag doesn't exist in scene, unable to teleport");
                return;
            }

            flag = flags[0];

            camY = GameObject.Find("CamY").GetComponent<CameraLook>();
            fallingEvent = GameObject.FindObjectOfType<FallingEvent>();
            iceAxes = GetField<IceAxe>(flag, "iceAxes");
            climbing = GetField<Climbing>(flag, "climbing");
            playerRB = GetField<Rigidbody>(flag, "playerRB");
            ropeAnchor = GetField<RopeAnchor>(flag, "ropeanchor");
            routingFlag = flag;
            playerMove = GetField<PlayerMove>(flag, "playermove");
            playerTransform = GetField<Transform>(flag, "playerTransform");
            playerCameraHolder = GameObject.Find("PlayerCameraHolder").transform;

            isSolemnTempest = flag.isSolemnTempest;
            distanceActivator = flag.distanceActivatorST;
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
            // Only allowed in normal mode
            if (GameManager.control.permaDeathEnabled || GameManager.control.freesoloEnabled) {
                return false;
            }

            // Invalid scenes
            if (config.IsValidScene(sceneName) == false) {
                return false;
            }

            // Can't use while getting a score
            if (TimeAttack.receivingScore || TimeAttack.aboutToReceiveScore) {
                return false;
            }

            // Cannot teleport while crampons are in a wall, roped, or in a bivouac
            if (Crampons.cramponsActivated || Bivouac.currentlyUsingBivouac || Bivouac.movingToBivouac) {
                return false;
            }
            if (ropeAnchor != null && ropeAnchor.attached == true) {
                return false;
            }

            // Other conditions where the routing flag can't be used
            if (
                InGameMenu.isCurrentlyNavigationMenu
                || EnterPeakScene.enteringPeakScene
                || ResetPosition.resettingPosition
                || StamperPeakSummit.currentlyStampingPeakJournal
                || SummitFlag.placingEvent
            ) {
                return false;
            }

            // Check if something strange happened (shouldn't happen)
            if (isSolemnTempest == true) {
                if (distanceActivator == null || leavePeakScene == null) {
                    return false;
                }
            }

            // Make sure things needed for teleporting exist
            return climbing != null
                && fallingEvent != null
                && iceAxes != null
                && playerMove != null
                && playerRB != null
                && playerTransform != null;
        }

        /**
         * <summary>
         * Saves the current player position and rotation.
         * </summary>
         */
        private void Save() {
            SceneData data = config.GetSceneData(sceneName);

            if (data == null || barometer == null) {
                return;
            }

            if (playerMove.IsGrounded() == false || Mathf.Abs(barometer.currentMetresUp) >= 3f) {
                return;
            }

            if (menuClick != null) {
                menuClick.Play();
            }

            if (isSolemnTempest == true) {
                data.position = playerTransform.position - leavePeakScene.transform.position;
            } else {
                data.position = playerTransform.position;
            }

            data.rotation = playerCameraHolder.rotation;
            data.rotationY = camY.rotationY;
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

            if (menuClick != null) {
                menuClick.Play();
            }

            climbing.ReleaseLHand(false);
            climbing.ReleaseRHand(false);

            iceAxes.ReleaseLeft(false);
            iceAxes.ReleaseRight(false);

            playerRB.velocity = Vector3.zero;

            fallingEvent.fellShortDistance = false;
            fallingEvent.fellLongDistance = false;
            fallingEvent.fellToDeath = false;

            routingFlag.usedFlagTeleport = false;

            if (isSolemnTempest == true) {
                playerTransform.position = leavePeakScene.transform.position + data.position;
            } else {
                playerTransform.position = data.position;
            }

            playerCameraHolder.rotation = data.rotation;
            camY.rotationY = data.rotationY;

            if (isSolemnTempest == true) {
                distanceActivator.ForceCheck();
            }
        }
    }
}
