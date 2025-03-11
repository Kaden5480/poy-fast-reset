using Cfg = FastReset.Config.Cfg;
using SaveManager = FastReset.Saves.SaveManager;

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

        // An instance of StateManager accessible statically
        private static StateManager instance = null;

        private SaveManager saveManager {
            get => Plugin.instance.saveManager;
        }

        public StateManager() {
            instance = this;
        }

#region State Information

        /**
         * <summary>
         * What kinds of states are available.
         * </summary>
         */
        public static bool hasPlayerTemp { get => instance.player.HasTempState(); }
        public static bool hasPlayerSaved { get => instance.player.HasSavedState(); }
        public static bool hasSceneTemp { get => instance.scene.HasTempState(); }
        public static bool hasSceneSaved { get => instance.scene.HasSavedState(); }

#endregion

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
            if (cache.routingFlag.currentlyUsingFlag == true) {
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
            // Temporary
            if (cache.routingFlag.currentlyUsingFlag == true) {
                player.SaveTempState();
                scene.SaveTempState();
                LogDebug("Saved temporary state");
                return;
            }

            // Saved
            player.SaveState();
            scene.SaveState();
            LogDebug("Saved state to data store");

            saveManager.Save();
        }

#endregion

#region Restoring

        /**
         * <summary>
         * Whether the temporary state should be used.
         * Specifically used for restoring states.
         * </summary>
         * <param name="state">The state to check</param>
         * <returns>True if it should, false otherwise</returns>
         */
        private bool UseTemporary(BaseState state) {
            return cache.routingFlag.currentlyUsingFlag == true
                && state.HasTempState() == true;
        }

        /**
         * <summary>
         * Restores a type of state for the given state.
         * </summary>
         * <param name="state">The type of state to restore a state for</param>
         */
        private bool Restore(BaseState state, bool useInitialState) {
            // Initial
            if (useInitialState == true) {
                state.RestoreInitialState();
                return true;
            }
            // Temporary
            else if (UseTemporary(state) == true) {
                state.RestoreTempState();
                return true;
            }
            // Saved
            else if (state.HasSavedState() == true) {
                state.RestoreState();
                return true;
            }

            return false;
        }

        /**
         * <summary>
         * Restores the state of the scene.
         * This also determines the type of state to restore.
         * </summary>
         */
        public void RestoreState() {
            if (Restore(player, config.useInitialPlayerState.Value) == true) {
                LogDebug("Restored player state");
            }

            if (Restore(scene, config.useInitialSceneState.Value) == true) {
                LogDebug("Restored scene state");
            }
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
