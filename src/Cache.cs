using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FastReset {
    /**
     * <summary>
     * This class contains objects which make fast
     * reset work properly.
     * </summary>
     */
    public class Cache : Loggable {
        // The current scene
        public Scene scene;

        public Climbing climbing;
        public FallingEvent fallingEvent;
        public IceAxe iceAxes;
        public InGameMenu inGameMenu;
        public Inventory inventory;
        public LeavePeakScene leavePeakScene;
        public PeakSummited peakSummited;
        public PlayerManager playerManager;
        public PlayerMove playerMove;
        public RopeAnchor ropeAnchor;
        public RoutingFlag routingFlag;
        public TimeAttack timeAttack;
        public bool timerLocked {
            get => GetField<TimeAttack, bool>(timeAttack, "timerLocked");
        }
        public bool canActivateInventory {
            get => GetField<Inventory, bool>(inventory, "canActivateInventory");
            set => SetField<Inventory, bool>(inventory, "canActivateInventory", value);
        }

        // Objects used for resetting the player
        public Rigidbody playerRb;
        public Transform playerTransform {
            get => playerRb.gameObject.transform;
        }
        public CameraLook playerCamX;
        public CameraLook playerCamY;

        // Objects used for resetting the wind
        public PeakWind peakWind;

        /**
         * <summary>
         * Gets a field of type `FT` from a provided instance of an object of type `T`.
         * </summary>
         * <param name="instance">The instance to access the field for</param>
         * <param name="fieldName">The name of the field to access</param>
         */
        private FT GetField<T, FT>(T instance, string fieldName) {
            return (FT) AccessTools.Field(typeof(T), fieldName).GetValue(instance);
        }

        /**
         * <summary>
         * Sets a field of type `FT` for a provided instance of an object of type `T`.
         * </summary>
         * <param name="instance">The instance to change the field for</param>
         * <param name="fieldName">The name of the field to set</param>
         * <param name="value">The value to set the field to</param>
         */
        private void SetField<T, FT>(T instance, string fieldName, FT value) {
            AccessTools.Field(typeof(T), fieldName).SetValue(instance, value);
        }

        /**
         * <summary>
         * Checks if all objects required for fast reset to function
         * are available in the cache.
         *
         * This is limited to objects required for resetting/saving
         * the player's position.
         * </summary>
         * <returns>True if they are, false otherwise</returns>
         */
        public bool IsComplete() {
            return climbing != null
                && fallingEvent != null
                && iceAxes != null
                && leavePeakScene != null
                && playerMove != null
                && ropeAnchor != null
                && routingFlag != null
                && timeAttack != null
                && playerRb != null
                && playerCamX != null
                && playerCamY != null;
        }

        /**
         * <summary>
         * Caches objects in the scene.
         * </summary>
         */
        public void FindObjects() {
            LogDebug("Caching objects in scene");
            scene = SceneManager.GetActiveScene();

            climbing = GameObject.FindObjectOfType<Climbing>();
            fallingEvent = GameObject.FindObjectOfType<FallingEvent>();
            iceAxes = GameObject.FindObjectOfType<IceAxe>();
            inGameMenu = GameObject.FindObjectOfType<InGameMenu>();
            inventory = GameObject.FindObjectOfType<Inventory>();
            leavePeakScene = GameObject.FindObjectOfType<LeavePeakScene>();
            peakSummited = GameObject.FindObjectOfType<PeakSummited>();
            peakWind = GameObject.FindObjectOfType<PeakWind>();
            playerManager = GameObject.FindObjectOfType<PlayerManager>();
            playerMove = GameObject.FindObjectOfType<PlayerMove>();
            ropeAnchor = GameObject.FindObjectOfType<RopeAnchor>();
            routingFlag = GameObject.FindObjectOfType<RoutingFlag>();
            timeAttack = GameObject.FindObjectOfType<TimeAttack>();

            if (climbing != null) {
                playerRb = climbing.gameObject.GetComponent<Rigidbody>();
            }

            // Access the player's camera
            GameObject cameraHolderObj = GameObject.Find("PlayerCameraHolder");
            if (cameraHolderObj == null) {
                LogDebug("No camera holder found");
                return;
            }

            // The camera has two components, X and Y
            foreach (CameraLook cameraLook in cameraHolderObj.GetComponentsInChildren<CameraLook>()) {
                string name = cameraLook.gameObject.name;

                if ("PlayerCameraHolder".Equals(name) == true) {
                    LogDebug("Found camX");
                    playerCamX = cameraLook;
                }
                else {
                    LogDebug("Found camY");
                    playerCamY = cameraLook;
                }
            }
        }

        /**
         * <summary>
         * Clears the cache.
         * </summary>
         */
        public void Clear() {
            LogDebug("clearing cache");

            climbing = null;
            fallingEvent = null;
            iceAxes = null;
            inGameMenu = null;
            inventory = null;
            leavePeakScene = null;
            peakSummited = null;
            playerManager = null;
            playerMove = null;
            ropeAnchor = null;
            routingFlag = null;
            timeAttack = null;

            playerRb = null;
            playerCamX = null;
            playerCamY = null;

            peakWind = null;
        }
    }
}
