namespace FastReset.State {
    public interface BaseState {

#region Initial

        /**
         * <summary>
         * Saves the initial state of the current scene.
         * </summary>
         */
        void SaveInitialState();

        /**
         * <summary>
         * Restores the initial state for the current scene.
         * </summary>
         */
        void RestoreInitialState();

#endregion

#region Saved

        /**
         * <summary>
         * Checks whether a saved state is available for
         * the current scene.
         * </summary>
         */
        bool HasSavedState();

        /**
         * <summary>
         * Saves the current scene state to the data store.
         * </summary>
         */
        void SaveState();

        /**
         * <summary>
         * Restores the saved state for the current scene.
         * </summary>
         */
        void RestoreState();

#endregion

#region Cleaning Up

        /**
         * <summary>
         * Wipes any stored states, typically used
         * on scene unloads.
         *
         * WARNING: This doesn't save anything
         * </summary>
         */
        void WipeState();

#endregion

    }
}
