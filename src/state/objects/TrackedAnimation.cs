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

        // Initial and temporary states
        // All times are normalized
        private List<float> initialTimes = new List<float>();
        private List<float> temporaryTimes = new List<float>();

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

#region Temporary

        /**
         * <summary>
         * Saves the current state temporarily (routing flag mode).
         * </summary>
         */
        public override void SaveTempState() {
            Save(temporaryTimes);
            LogDebug("Saved temporary state");
        }

        /**
         * <summary>
         * Restores the routing flag mode state.
         * </summary>
         */
        public override void RestoreTempState() {
            Restore(temporaryTimes);
            LogDebug("Restored temporary state");
        }

#endregion

#region Saved

        /**
         * <summary>
         * Saves the current state to the data store.
         * </summary>
         */
        public override void SaveState() {
            SavedAnimation savedAnimation = SaveManager.GetAnimation(id);

            // Add a new animation to the data store
            if (savedAnimation == null) {
                savedAnimation = new SavedAnimation(byteId);
                SaveManager.Add(savedAnimation);
            }

            // Update the saved state
            Save(savedAnimation.times);
            LogDebug("Updated state in data store");
        }

        /**
         * <summary>
         * Restores the state stored in the data store.
         * </summary>
         */
        public override void RestoreState() {
            SavedAnimation savedAnimation = SaveManager.GetAnimation(id);

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
