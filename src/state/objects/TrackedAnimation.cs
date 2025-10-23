using System.Collections.Generic;

using UnityEngine;

using SaveManager = FastReset.Saves.SaveManager;
using SavedAnimation = FastReset.Saves.SavedAnimation;

namespace FastReset.State {
    /**
     * <summary>
     * An animation which is tracked so it can
     * be saved/restored.
     * </summary>
     */
    public class TrackedAnimation : BaseTracked {
        private Animation animation = null;

        // Initial state
        // All times are normalized
        private List<float> initialTimes = new List<float>();

        public TrackedAnimation(GameObject obj) : base(obj) {}

        /**
         * <summary>
         * A method to simplify restoring different kinds of states.
         * </summary>
         * <param name="times">The state times to restore</param>
         */
        private void Restore(List<float> times) {
            int i = 0;
            foreach (AnimationState state in animation) {
                state.normalizedTime = times[i];
                i++;
            }
        }

        /**
         * <summary>
         * A method to simplify saving different kinds of states.
         * </summary>
         * <param name="times">The list to store states into</param>
         */
        private void Save(List<float> times) {
            times.Clear();
            foreach (AnimationState state in animation) {
                times.Add(state.normalizedTime);
            }
        }

#region Initial

        /**
         * <summary>
         * Saves the initial state.
         * </summary>
         */
        public override void SaveInitialState() {
            animation = obj.GetComponent<Animation>();

            Save(initialTimes);
            LogDebug("Saved initial state");
        }

        /**
         * <summary>
         * Restores the initial state.
         * </summary>
         */
        public override void RestoreInitialState() {
            Restore(initialTimes);
            LogDebug("Restored initial state");
        }

#endregion

#region Saved

        /**
         * <summary>
         * Saves the current state to the data store.
         * </summary>
         * <param name="save">The save data to save to</param>
         */
        public override void SaveState(SaveData save) {
            // Add a new animation to the data store
            SavedAnimation savedAnimation = new SavedAnimation(byteId);
            save.Add(savedAnimation);

            // Update the saved state
            Save(savedAnimation.times);
            LogDebug("Updated state in data store");
        }

        /**
         * <summary>
         * Restores the state stored in the data store.
         * </summary>
         * <param name="save">The save data to restore from</param>
         */
        public override void RestoreState(SaveData save) {
            SavedAnimation savedAnimation = save.GetAnimation(id);

            if (savedAnimation == null) {
                LogDebug("No saved state to restore");
                return;
            }

            Restore(savedAnimation.times);
            LogDebug("Restored state from data store");
        }

#endregion

    }
}
