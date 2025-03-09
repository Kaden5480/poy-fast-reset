using System.Collections.Generic;
using System.IO;

using BepInEx.Configuration;
using UnityEngine;

using Paths = FastReset.Config.Paths;

namespace FastReset.State {
    public class SceneState : BaseState {
        private bool setTemporary = false;
        private bool setConfig = false;

        private List<TrackedObject> objs = new List<TrackedObject>();

        public static ConfigFile animationsFile;

        private void SaveInitialState() {
            Plugin.LogDebug("SceneState: Saving initial state");
            foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>()) {
                Animation animation = obj.GetComponent<Animation>();
                if (animation != null
                    && "Peak_3_OldMill".Equals(cache.scene.name) == true
                    && "mill_wings".Equals(obj.name) == true
                ) {
                    objs.Add(new TrackedAnimation(obj));
                }
            }
        }

        protected override void SaveTempState() {
            Plugin.LogDebug("SceneState: Saving temporary state");
            foreach (TrackedObject obj in objs) {
                obj.SaveTempState();
            }

            setTemporary = true;
        }

        protected override void SaveConfigState() {
            Plugin.LogDebug("SceneState: Saving config state");
            foreach (TrackedObject obj in objs) {
                obj.SaveConfigState();
            }

            setConfig = true;
        }

        protected override bool HasTempState() {
            return setTemporary;
        }

        protected override bool HasConfigState() {
            return setConfig;
        }

        private void RestoreInitialState() {
            Plugin.LogDebug("SceneState: Restoring initial state");
            foreach (TrackedObject obj in objs) {
                obj.RestoreInitialState();
            }
        }

        protected override void RestoreTempState() {
            Plugin.LogDebug("SceneState: Restoring temporary state");
            foreach (TrackedObject obj in objs) {
                obj.RestoreTempState();
            }
        }

        protected override void RestoreConfigState() {
            Plugin.LogDebug("SceneState: Restoring config state");
            foreach (TrackedObject obj in objs) {
                obj.RestoreConfigState();
            }
        }

        // Scene loads and unloads
        public override void Load() {
            if (File.Exists(Paths.animationsPath) == true) {
                Plugin.LogDebug("SceneState: Loading animations config");
                animationsFile = new ConfigFile(
                    Paths.animationsPath, false
                );
                setConfig = true;
            }

            SaveInitialState();
        }

        public override void Unload() {
            Plugin.LogDebug("SceneState: Unloading scene states");

            setTemporary = false;
            setConfig = false;

            if (animationsFile != null) {
                Plugin.LogDebug("SceneState: Saving animations file");
                animationsFile.Save();
            }

            animationsFile = null;

            objs.Clear();
        }

        // Allow saving/restoring initial states
        public override void SaveState() {
            base.SaveState();
        }

        public override bool RestoreState() {
            return base.RestoreState();
        }
    }
}
