namespace FastReset.State {
    public abstract class BaseState {
        protected Cache cache { get => Plugin.instance.cache; }

        protected abstract void SaveTempState();
        protected abstract void SaveConfigState();

        protected abstract bool HasTempState();
        protected abstract bool HasConfigState();

        protected abstract void RestoreTempState();
        protected abstract void RestoreConfigState();

        // Scene loads and unloads
        public abstract void Load();
        public abstract void Unload();

        public virtual void SaveState() {
            Plugin.LogDebug("BaseState: Saving state");
            if (cache.routingFlag.currentlyUsingFlag == true) {
                SaveTempState();
                return;
            }

            SaveConfigState();
        }

        public virtual bool RestoreState() {
            Plugin.LogDebug("BaseState: Restoring state");
            if (cache.routingFlag.currentlyUsingFlag == true
                && HasTempState() == true
            ) {
                RestoreTempState();
                return true;
            }

            if (HasConfigState() == true) {
                RestoreConfigState();
                return true;
            }

            Plugin.LogDebug("BaseState: Found no state to restore");
            return false;
        }
    }
}
