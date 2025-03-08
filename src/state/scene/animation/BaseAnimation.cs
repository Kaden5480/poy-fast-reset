namespace FastReset.State {
    public abstract class BaseAnimation {
        protected Animation animation;

        public BaseAnimation(Animation animation) {
            this.animation = animation;
        }

        public abstract void Save();

        public abstract void Restore();
    }
}
