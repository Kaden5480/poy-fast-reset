using System.Collections.Generic;

using BepInEx.Configuration;
using UnityEngine;
using UAnim = UnityEngine.Animation;

namespace FastReset.State {
    public class SavedAnimation : BaseAnimation {
        private string trackedId;

        private ConfigFile configFile {
            get => SceneState.animationFile;
        }

        private List<ConfigEntry<float>> stateTimers;

        public SavedAnimation(string trackedId, UAnim animation) : base(animation) {
            this.trackedId = trackedId;
            stateTimers = new List<ConfigEntry<float>>();

            int i = 0;
            foreach (AnimationState state in animation) {
                stateTimers.Add(configFile.Bind(
                    trackedId, $"state_{i}", state.normalizedTime
                ));
                i++;
            }
        }

        public override void Save() {
            int i = 0;
            foreach (AnimationState state in animation) {
                if (i < stateTimers.Count) {
                    stateTimers[i].Value = state.normalizedTime;
                }
                else {
                    stateTimers.Add(configFile.Bind(
                        trackedId, $"state_{i}", state.normalizedTime
                    ));
                }
                i++;
            }
        }

        public override void Restore() {
            int i = 0;
            foreach (AnimationState state in animation) {
                state.normalizedTime = stateTimers[i].Value;
                i++;
            }
        }
    }
}
