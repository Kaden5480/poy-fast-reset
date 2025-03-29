using System.Collections.Generic;

using BepInEx.Configuration;
using UnityEngine;

using SaveManager = FastReset.Saves.SaveManager;

namespace FastReset.State {
    public class SceneState : Loggable, BaseState {
        // Indicates whether a temporary state has been stored
        private bool hasTempState = false;

        // The objects which are being tracked
        private List<BaseTracked> objs = new List<BaseTracked>();

#region Initial

        /**
         * <summary>
         * Saves the initial state of the current scene.
         * </summary>
         */
        public void SaveInitialState() {
            int animationCount = 0;
            int brickCount = 0;
            int brittleIceCount = 0;
            int crumblingHoldCount = 0;
            int jointCount = 0;

            foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>()) {
                Animation animation = obj.GetComponent<Animation>();
                BrickHold brickHold = obj.GetComponent<BrickHold>();
                BrittleIce brittleIce = obj.GetComponent<BrittleIce>();
                CrumblingHoldRegular crumblingHold = obj.GetComponent<CrumblingHoldRegular>();
                ConfigurableJoint joint = obj.GetComponent<ConfigurableJoint>();

                // Currently only track the animation of the mill on Old Mill
                if (animation != null
                    && "Peak_3_OldMill".Equals(Plugin.instance.cache.scene.name) == true
                    && "mill_wings".Equals(obj.name) == true
                ) {
                    objs.Add(new TrackedAnimation(obj));
                    animationCount++;
                }

                if (brickHold != null
                    && brickHold.popoutInstantly == false
                ) {
                    objs.Add(new TrackedBrick(obj));
                    brickCount++;
                }

                if (brittleIce != null
                    && brittleIce.largeStalagmite == true
                ) {
                    objs.Add(new TrackedBrittleIce(obj));
                    brittleIceCount++;
                }

                if (crumblingHold != null) {
                    objs.Add(new TrackedCrumblingHold(obj));
                    crumblingHoldCount++;
                }

                if (joint != null && (
                    obj.name.StartsWith("TrainingBeam") == true
                    || obj.name.StartsWith("wheelJoint") == true
                )) {
                    objs.Add(new TrackedJoint(obj));
                    jointCount++;
                }
            }

            LogDebug("Saved initial state");
            LogDebug(
                $"Tracking: {objs.Count} objects\n"
                + $"Animations:      {animationCount}\n"
                + $"Bricks:          {brickCount}\n"
                + $"Brittle ice:     {brittleIceCount}\n"
                + $"Crumbling holds: {crumblingHoldCount}\n"
                + $"Joints:          {jointCount}"
            );
        }

        /**
         * <summary>
         * Restores the initial state for the current scene.
         * </summary>
         */
        public void RestoreInitialState() {
            foreach (BaseTracked obj in objs) {
                obj.RestoreInitialState();
            }

            LogDebug("Restored initial state");
        }

#endregion

#region Temporary

        /**
         * <summary>
         * Checks whether a temporary state is available
         * for the current scene.
         * </summary>
         */
        public bool HasTempState() {
            return hasTempState;
        }

        /**
         * <summary>
         * Saves the state of the scene temporarily.
         * </summary>
         */
        public void SaveTempState() {
            foreach (BaseTracked obj in objs) {
                obj.SaveTempState();
            }

            hasTempState = true;
            LogDebug("Saved temporary state");
        }

        /**
         * <summary>
         * Restores the temporary state
         * for the current scene.
         * </summary>
         */
        public void RestoreTempState() {
            foreach (BaseTracked obj in objs) {
                obj.RestoreTempState();
            }

            LogDebug("Restored temporary state");
        }

#endregion

#region Saved

        /**
         * <summary>
         * Checks whether a saved state is available for
         * the current scene.
         * </summary>
         */
        public bool HasSavedState() {
            return SaveManager.hasSceneState;
        }

        /**
         * <summary>
         * Saves the current scene state to the data store.
         * </summary>
         */
        public void SaveState() {
            // Wipe scene data first
            SaveManager.WipeScene();

            foreach (BaseTracked obj in objs) {
                obj.SaveState();
            }

            LogDebug("Saved state to data store");
        }

        /**
         * <summary>
         * Restores the saved state for the current scene.
         * </summary>
         */
        public void RestoreState() {
            foreach (BaseTracked obj in objs) {
                obj.RestoreState();
            }

            LogDebug("Restored state from data store");
        }

#endregion

#region Cleaning Up

        /**
         * <summary>
         * Wipes any stored states, typically used
         * on scene unloads.
         * </summary>
         */
        public void WipeState() {
            hasTempState = false;
            objs.Clear();

            LogDebug("Wiped state");
        }

#endregion

    }
}
