using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

namespace FastReset {
    public class SceneObjects {
        public AudioSource menuClick;

        public LeavePeakScene leavePeakScene;
        public RoutingFlag routingFlag;
        public TimeAttack timeAttack;

        public IceAxe iceAxes;
        public Climbing climbing;
        public Rigidbody playerRB;
        public PlayerMove playerMove;
        public RopeAnchor ropeAnchor;
        public FallingEvent fallingEvent;
        public Transform playerTransform;

        public Transform playerCameraHolder;
        public CameraLook camY;

        public bool isSolemnTempest;
        public DistanceActivator distanceActivator;

        public List<JointData> joints;

        /**
         * <summary>
         * Constructs an instance of SceneObjects, initializing fields.
         * </summary>
         */
        public SceneObjects() {
            Reset();
        }

        /**
         * <summary>
         * Gets a private field of type `FT` from an object with type `T`.
         * </summary>
         * <param name="obj">The object to get the value of the private field from</param>
         * <param name="fieldName">The field to get the value of</param>
         */
        private FT GetPrivateField<T, FT>(T obj, string fieldName) {
            return (FT) typeof(T).GetField(fieldName,
                BindingFlags.NonPublic | BindingFlags.Instance
            ).GetValue(obj);
        }

        /**
         * <summary>
         * Resets the state of references to objects.
         * </summary>
         */
        public void Reset() {
            menuClick = null;

            leavePeakScene = null;
            routingFlag = null;
            timeAttack = null;

            iceAxes = null;
            climbing = null;
            playerRB = null;
            playerMove = null;
            ropeAnchor = null;
            fallingEvent = null;
            playerTransform = null;

            playerCameraHolder = null;
            camY = null;

            isSolemnTempest = false;
            distanceActivator = null;
            joints = new List<JointData>();
        }

        /**
         * <summary>
         * Loads references to objects in the current scene.
         * </summary>
         */
        public void LoadObjects() {
            Reset();

            InGameMenu menu = GameObject.FindObjectOfType<InGameMenu>();

            leavePeakScene = GameObject.FindObjectOfType<LeavePeakScene>();
            routingFlag = GameObject.FindObjectOfType<RoutingFlag>();
            timeAttack = GameObject.FindObjectOfType<TimeAttack>();

            if (routingFlag == null) {
                return;
            }

            if (menu != null) {
                menuClick = menu.menuClick;
            }

            iceAxes = GetPrivateField<RoutingFlag, IceAxe>(routingFlag, "iceAxes");
            climbing = GetPrivateField<RoutingFlag, Climbing>(routingFlag, "climbing");
            playerRB = GetPrivateField<RoutingFlag, Rigidbody>(routingFlag, "playerRB");
            playerMove = GetPrivateField<RoutingFlag, PlayerMove>(routingFlag, "playermove");
            ropeAnchor = GetPrivateField<RoutingFlag, RopeAnchor>(routingFlag, "ropeanchor");
            fallingEvent = GameObject.FindObjectOfType<FallingEvent>();
            playerTransform = GetPrivateField<RoutingFlag, Transform>(routingFlag, "playerTransform");

            GameObject playerCameraHolderObj = GameObject.Find("PlayerCameraHolder");
            if (playerCameraHolderObj != null) {
                playerCameraHolder = playerCameraHolderObj.transform;
            }

            GameObject camYObj = GameObject.Find("CamY");
            if (camYObj != null) {
                camY = camYObj.GetComponent<CameraLook>();
            }

            isSolemnTempest = routingFlag.isSolemnTempest;
            distanceActivator = routingFlag.distanceActivatorST;

            foreach (ConfigurableJoint joint in GameObject.FindObjectsOfType<ConfigurableJoint>()) {
                GameObject jointObj = joint.gameObject;

                if (jointObj.name.StartsWith("TrainingBeam") == false
                    && jointObj.name.StartsWith("wheelJoint") == false
                ) {
                    continue;
                }

                joints.Add(new JointData(jointObj));
            }
        }

        /**
         * <summary>
         * Checks whether teleporting is enabled on the current scene.
         * </summary>
         * <return>True if teleporting is enabled, false otherwise</return>
         */
        public bool CanTeleport() {
            // Only allowed in normal mode
            if (GameManager.control.permaDeathEnabled || GameManager.control.freesoloEnabled) {
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
                && playerTransform != null
                && timeAttack != null;
        }
    }
}
