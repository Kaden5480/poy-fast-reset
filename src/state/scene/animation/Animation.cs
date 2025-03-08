using UAnim = UnityEngine.Animation;

namespace FastReset.State {
    public class Animation : BaseAnimation {
        private List<float> stateTimers;

        public Animation(UAnim animation) : base(animation) {
            stateTimers = new List<float>();
            Save();
        }

        public override void Save() {
            stateTimers.Clear();
            foreach (AnimationState state in animation) {
                stateTimers.Add(state.normalizedTime);
            }
        }

        public override void Restore() {
            int i = 0;
            foreach (AnimationState state in animation) {
                state.normalizedTime = stateTimers[i];
                i++;
            }
        }
    }
}
