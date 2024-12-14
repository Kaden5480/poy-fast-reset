using System;
using System.Reflection;

using UnityEngine;
using UnityEngine.SceneManagement;

#if BEPINEX

using BepInEx;
using BepInEx.Configuration;

namespace FastReset {
    [BepInPlugin("com.github.Kaden5480.poy-fast-reset", "FastReset", PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin {
        private ConfigEntry<string> configKeybind;

        /**
         * <summary>
         * Executes when the plugin is being loaded.
         * </summary>
         */
        public void Awake() {
            configKeybind = Config.Bind("General", "keybind", defaultKeybind, "Keybind to teleport");
            SceneManager.sceneLoaded += OnSceneLoaded;

            CommonAwake();
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

[assembly: MelonInfo(typeof(FastReset.Plugin), "FastReset", PluginInfo.PLUGIN_VERSION, "Kaden5480")]
[assembly: MelonGame("TraipseWare", "Peaks of Yore")]

namespace FastReset {
    public class Plugin: MelonMod {
        private MelonPreferences_Category configCategory;
        private MelonPreferences_Entry<string> configKeybind;

        /**
         * <summary>
         * Executes when the mod is being loaded.
         * </summary>
         */
        public override void OnInitializeMelon() {
            configCategory = MelonPreferences.CreateCategory("FastReset");
            configKeybind = configCategory.CreateEntry("keybind", defaultKeybind);

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
         * Logs a message.
         * </summary>
         * <param name="message">The message to log</param>
         */
        private void LogInfo(string message) {
            MelonLogger.Msg(message);
        }

#endif

        private string defaultKeybind = KeyCode.F4.ToString();
        private KeyCode keybind;


        private int sceneIndex;
        private string sceneName;

        private AudioSource menuClick;

        private CameraLook camY;
        private Climbing climbing;
        private IceAxe iceAxes;
        private Rigidbody playerRB;
        private RopeAnchor ropeAnchor;
        private Transform playerTransform;
        private Transform playerCameraHolder;

        private LeavePeakScene leavePeakScene;
        private bool isSolemnTempest;
        private DistanceActivator distanceActivator;

        private T GetField<T>(RoutingFlag flag, string fieldName) {
            return (T) typeof(RoutingFlag).GetField(fieldName,
                BindingFlags.NonPublic | BindingFlags.Instance
            ).GetValue(flag);
        }

        private void Reset() {
            sceneIndex = -1;
            sceneName = null;

            menuClick = null;

            camY = null;
            iceAxes = null;
            climbing = null;
            playerRB = null;
            ropeAnchor = null;
            playerTransform = null;
            playerCameraHolder = null;

            leavePeakScene = null;
            isSolemnTempest = false;
            distanceActivator = null;
        }

        private void CommonAwake() {
            keybind = (KeyCode) System.Enum.Parse(typeof(KeyCode), configKeybind.Value);
        }

        private void CommonSceneLoad(int buildIndex, string sceneName) {
            InGameMenu[] menus = GameObject.FindObjectsOfType<InGameMenu>();
            LeavePeakScene[] leavePeakScenes = GameObject.FindObjectsOfType<LeavePeakScene>();
            RoutingFlag[] flags = GameObject.FindObjectsOfType<RoutingFlag>();
            RoutingFlag flag;

            Reset();

            this.sceneIndex = buildIndex;
            this.sceneName = sceneName;

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
            iceAxes = GetField<IceAxe>(flag, "iceAxes");
            climbing = GetField<Climbing>(flag, "climbing");
            playerRB = GetField<Rigidbody>(flag, "playerRB");
            ropeAnchor = GetField<RopeAnchor>(flag, "ropeanchor");
            playerTransform = GetField<Transform>(flag, "playerTransform");
            playerCameraHolder = GameObject.Find("PlayerCameraHolder").transform;

            isSolemnTempest = flag.isSolemnTempest;
            distanceActivator = flag.distanceActivatorST;
        }

        private void CommonUpdate() {
            if (CanTeleport() == false) {
                return;
            }

            if (Input.GetKeyDown(keybind) == true) {
                Teleport();
            }
        }

        private bool CanTeleport() {
            // Only allowed in normal mode
            if (GameManager.control.permaDeathEnabled || GameManager.control.freesoloEnabled) {
                return false;
            }

            // Other conditions where the routing flag can't be used
            if (InGameMenu.isCurrentlyNavigationMenu || EnterPeakScene.enteringPeakScene || ResetPosition.resettingPosition) {
                return false;
            }

            // Cannot teleport while crampons are in a wall, roped, or in a bivouac
            if (Crampons.cramponsActivated || Bivouac.currentlyUsingBivouac) {
                return false;
            }
            if (ropeAnchor != null && ropeAnchor.attached == true) {
                return false;
            }

            // Invalid scenes
            if (Scenes.GetScene(sceneName) == null) {
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
                && iceAxes != null
                && playerRB != null
                && playerTransform != null;
        }

        private void Teleport() {
            SceneData data = Scenes.GetScene(sceneName);

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
