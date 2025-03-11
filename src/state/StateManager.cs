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
         * <returns>Whether a state was saved</returns>
         */
        private bool Save(BaseState state, bool canModify) {
            if (canModify == false) {
                return false;
            }

            if (cache.routingFlag.currentlyUsingFlag == true) {
                state.SaveTempState();
                return true;
            }

            state.SaveState();
            return true;
        }

        /**
         * <summary>
         * Saves the current state of the scene.
         * This also determines the type of state to save.
         * </summary>
         * <returns>Whether a state was saved</returns>
         */
        public bool SaveState() {
            bool saved = false;

            if (Save(player, config.modifyPlayerState.Value) == true) {
                saved = true;
            }
            if (Save(scene, config.modifySceneState.Value) == true) {
                saved = true;
            }

            // Only save if not in routing flag mode
            if (cache.routingFlag.currentlyUsingFlag == false) {
                saveManager.Save();
            }

            return saved;
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
         * <returns>Whether a state was restored</returns>
         */
        public bool RestoreState() {
            bool restored = false;

            bool restorePlayerState = config.restorePlayerState.Value;
            bool restoreSceneState = config.restoreSceneState.Value;

            // Enforce both in routing flag mode
            if (restorePlayerState == false
                && restoreSceneState == true
                && cache.routingFlag.currentlyUsingFlag == false
            ) {
                LogError("Cannot restore only scene state in normal mode");
                return false;
            }

            if (restorePlayerState == true
                && Restore(player, config.useInitialPlayerState.Value) == true
            ) {
                LogDebug("Restored player state");
                restored = true;
            }

            if (restoreSceneState == true
                && Restore(scene, config.useInitialSceneState.Value) == true
            ) {
                LogDebug("Restored scene state");
                restored = true;
            }

            return restored;
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
