using UnityEngine;

using UAnim = UnityEngine.Animation;

namespace FastReset.State {
    public abstract class BaseAnimation {
        protected UAnim animation;

        public BaseAnimation(UAnim animation) {
            this.animation = animation;
        }

        public abstract void Save();

        public abstract void Restore();
    }
}
