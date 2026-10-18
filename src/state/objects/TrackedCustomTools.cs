using UnityEngine;

using SaveManager = FastReset.Saves.SaveManager;
using SavedCustomTools = FastReset.Saves.SavedCustomTools;

namespace FastReset.State {
    /**
     * <summary>
     * A tracked state of tools within custom levels.
     * </summary>
     */
    public class TrackedCustomTools : BaseTracked {
        private Cache cache {
            get => Plugin.instance.cache;
        }

        // Initial and temporary states
        private bool initialCrampons = false;
        private bool initialCramponsUpgrade = false;
        private bool initialIceAxes = false;
        private int initialPickups = 0;

        private bool tempCrampons = false;
        private bool tempCramponsUpgrade = false;
        private bool tempIceAxes = false;
        private int tempPickups = 0;

        public TrackedCustomTools(GameObject obj) : base(obj) {}

        /**
         * <summary>
         * A method to simplify restoring different kinds of states.
         * </summary>
         */
        private void Restore(bool crampons, bool cramponsUpgrade, bool iceAxes, int pickups) {
            cache.leToolManager.crampons = crampons;
            cache.leToolManager.cramponsUpgrade = (cramponsUpgrade) ? 1 : 0;
            cache.leToolManager.stemfoot.isAlpDam = !crampons;
            cache.leToolManager.levelEditorManager.crampons.isAlpDam = false;
            cache.leToolManager.stemfoot.CustomLevelCrampons();
            cache.leToolManager.iceAxes = iceAxes;
            if (iceAxes == false) {
                cache.leToolManager.iceAxeScript.RemoveIceAxes(true);
                cache.leToolManager.iceAxeScript.IceAxesAttackReset();
            }
            cache.leToolManager.currentQuestCollectibles = pickups;
        }

        /**
         * <summary>
         * A method to simplify saving different kinds of states.
         * </summary>
         */
        private void Save(ref bool crampons, ref bool cramponsUpgrade, ref bool iceAxes, ref int pickups) {
            crampons = cache.leToolManager.crampons;
            cramponsUpgrade = cache.leToolManager.cramponsUpgrade == 1;
            iceAxes = cache.leToolManager.iceAxes;
            pickups = cache.leToolManager.currentQuestCollectibles;
        }

#region Initial

        /**
         * <summary>
         * Saves the initial state.
         * </summary>
         */
        public override void SaveInitialState() {
            Save(ref initialCrampons, ref initialCramponsUpgrade, ref initialIceAxes, ref initialPickups);
            LogDebug("Saved initial state");
        }

        /**
         * <summary>
         * Restores the initial state.
         * </summary>
         */
        public override void RestoreInitialState() {
            Restore(initialCrampons, initialCramponsUpgrade, initialIceAxes, initialPickups);
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
            Save(ref tempCrampons, ref tempCramponsUpgrade, ref tempIceAxes, ref tempPickups);
            LogDebug("Saved temporary state");
        }

        /**
         * <summary>
         * Restores the routing flag mode state.
         * </summary>
         */
        public override void RestoreTempState() {
            Restore(tempCrampons, tempCramponsUpgrade, tempIceAxes, tempPickups);
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
            SavedCustomTools savedTools = new SavedCustomTools();
            SaveManager.Add(savedTools);

            Save(ref savedTools.crampons, ref savedTools.cramponsUpgrade, ref savedTools.iceAxes, ref savedTools.pickups);
            LogDebug("Updated state in data store");
        }

        /**
         * <summary>
         * Restores the state stored in the data store.
         * </summary>
         */
        public override void RestoreState() {
            SavedCustomTools savedTools = SaveManager.GetCustomTools();

            if (savedTools == null) {
                LogDebug("No saved state to restore");
                return;
            }

            Restore(savedTools.crampons, savedTools.cramponsUpgrade, savedTools.iceAxes, savedTools.pickups);
            LogDebug("Restored state from data store");
        }

#endregion

    }
}
