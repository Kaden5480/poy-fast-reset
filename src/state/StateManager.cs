using Cfg = FastReset.Config.Cfg;

namespace FastReset.State {
    public class StateManager : Loggable {
        // Shorthand for accessing the config and cache
        private Cfg config {
            get => Plugin.instance.config;
        }
        private Cache cache {
            get => Plugin.instance.cache;
        }

        // Manages/controls the current state of the scene
        private PlayerState player = new PlayerState();
        private SceneState scene = new SceneState();

        /**
         * <summary>
         * Whether the temporary state should be used.
         * </summary>
         * <param name="state">The state to check</param>
         * <returns>True if it should, false otherwise</returns>
         */
        private bool UseTemporary(BaseState state) {
            return cache.routingFlag.currentlyUsingFlag == true
                && state.HasTempState() == true;
        }

#region Saving

        /**
         * <summary>
         * Saves the initial state of the scene.
         * This must be done separately to other save types.
         * </summary>
         */
        public void SaveInitialState() {
            player.SaveInitialState();
            scene.SaveInitialState();
        }

        /**
         * <summary>
         * Saves a state for the given state type.
         * </summary>
         * <param name="state">The type of state to save for</param>
         */
        private void Save(BaseState state) {
            if (UseTemporary(state) == true) {
                state.SaveTempState();
                return;
            }

            state.SaveState();
        }

        /**
         * <summary>
         * Saves the current state of the scene.
         * This also determines the type of state to save.
         * </summary>
         */
        public void SaveState() {
            // Player state
            if (config.modifyPlayerState.Value == true) {
                Save(player);
                Plugin.LogDebug("Saved player state");
            }

            // Scene state
            if (config.modifySceneState.Value == true) {
                Save(scene);
                Plugin.LogDebug("Saved scene state");
            }
        }

#endregion

#region Restoring

        /**
         * <summary>
         * Restores a type of state for the given state.
         * </summary>
         * <param name="state">The type of state to restore a state for</param>
         */
        private void Restore(BaseState state, bool useInitialState) {
            // Initial
            if (useInitialState == true) {
                state.RestoreInitialState();
            }
            // Temporary
            else if (UseTemporary(state) == true) {
                state.RestoreTempState();
            }
            // Saved
            else if (state.HasSavedState() == true) {
                state.RestoreState();
            }
        }

        /**
         * <summary>
         * Restores the state of the scene.
         * This also determines the type of state to restore.
         * </summary>
         */
        public void RestoreState() {
            Restore(player, config.useInitialPlayerState.Value);
            LogDebug("Restored player state");

            Restore(scene, config.useInitialSceneState.Value);
            LogDebug("Restored scene state");
        }

#endregion

        /**
         * <summary>
         * Wipes the currently stored states.
         * Typically used on scene unloads.
         *
         * WARNING: This doesn't save anything
         * </summary>
         */
        public void WipeState() {
            player.WipeState();
            scene.WipeState();
            LogDebug("Wiped state");
        }
    }
}
