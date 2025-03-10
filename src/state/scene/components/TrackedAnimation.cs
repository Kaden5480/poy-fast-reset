using System.Collections.Generic;

using BepInEx.Configuration;
using UnityEngine;

using Paths = FastReset.Config.Paths;

namespace FastReset.State {
    public class TrackedAnimation : TrackedObject {
        private Animation animation = null;

        private List<float> initialTimes = new List<float>();
        private List<float> temporaryTimes = new List<float>();

        private List<ConfigEntry<float>> configTimes = new List<ConfigEntry<float>>();

        public override void RestoreInitialState() {
            Plugin.LogDebug($"TrackedAnimation: [{obj.name}] Restoring initial state");

            int i = 0;
            foreach (AnimationState state in animation) {
                if (i >= initialTimes.Count) {
                    break;
                }

                state.normalizedTime = initialTimes[i];
                i++;
            }
        }

        public override void RestoreTempState() {
            Plugin.LogDebug($"TrackedAnimation: [{obj.name}] Restoring temporary state");

            int i = 0;
            foreach (AnimationState state in animation) {
                if (i >= initialTimes.Count) {
                    break;
                }

                Plugin.LogDebug($"TrackedAnimation: [{obj.name}] setting state_{i} to {configTimes[i].Value}");
                state.normalizedTime = temporaryTimes[i];
                i++;
            }
        }

        public override void RestoreConfigState() {
            Plugin.LogDebug($"TrackedAnimation: [{obj.name}] Restoring config state");

            int i = 0;
            foreach (AnimationState state in animation) {
                if (i >= configTimes.Count) {
                    break;
                }

                Plugin.LogDebug($"TrackedAnimation: [{obj.name}] setting state_{i} to {configTimes[i].Value}");
                state.normalizedTime = configTimes[i].Value;
                i++;
            }
        }

       protected override void SaveInitialState() {
            Plugin.LogDebug($"TrackedAnimation: [{obj.name}] Saving initial state");

            animation = obj.GetComponent<Animation>();
            foreach (AnimationState state in animation) {
                initialTimes.Add(state.normalizedTime);
            }
        }

        public override void SaveTempState() {
            Plugin.LogDebug($"TrackedAnimation: [{obj.name}] Saving temporary state");

            temporaryTimes.Clear();
            foreach (AnimationState state in animation) {
                temporaryTimes.Add(state.normalizedTime);
            }
        }

        public override void SaveConfigState() {
            Plugin.LogDebug($"TrackedAnimation: [{obj.name}] Saving config state");

            if (SceneState.animationsFile == null) {
                Plugin.LogDebug("TrackedAnimation: Creating animations file");
                SceneState.animationsFile = new ConfigFile(
                    Paths.animationsPath, false
                );
            }

            BindConfig();

            int i = 0;
            foreach (AnimationState state in animation) {
                if (i >= configTimes.Count) {
                    break;
                }

                Plugin.LogDebug($"TrackedAnimation: [{obj.name}] Updating state_{i} to {state.normalizedTime}");
                configTimes[i].Value = state.normalizedTime;
                i++;
            }
        }

        protected override void BindConfig() {
            if (boundConfig == true) {
                return;
            }

            if (SceneState.animationsFile == null) {
                Plugin.LogDebug("TrackedAnimation: Animations file doesn't exist, not binding config");
                return;
            }

            Plugin.LogDebug("TrackedAnimation: Binding config");
            configTimes.Clear();
            for (int i = 0; i < initialTimes.Count; i++) {
                Plugin.LogDebug($"TrackedAnimation: Binding state_{i} as {initialTimes[i]}");
                configTimes.Add(SceneState.animationsFile.Bind(
                    id, $"state_{i}", initialTimes[i]
                ));
            }

            boundConfig = true;
        }

        public TrackedAnimation(GameObject obj) : base(obj) {}
    }
}
