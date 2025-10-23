using System;
using System.Collections.Generic;

using PeterO.Cbor;

namespace FastReset.Saves {
    /**
     * <summary>
     * A class which stores save data for a specific save slot.
     * </summary>
     */
    public class SaveData : Loggable {
        // The saved player state for the scene
        private SavedPlayer player = null;

        // Dictionaries mapping IDs to saved objects
        private Dictionary<string, SavedAnimation> animations = new Dictionary<string, SavedAnimation>();
        private Dictionary<string, SavedBrick> bricks = new Dictionary<string, SavedBrick>();
        private Dictionary<string, SavedBrittleIce> brittleIces = new Dictionary<string, SavedBrittleIce>();
        private Dictionary<string, SavedCrumblingHold> crumblingHolds = new Dictionary<string, SavedCrumblingHold>();
        private Dictionary<string, SavedJoint> joints = new Dictionary<string, SavedJoint>();

        // What states exist for this scene
        public bool hasPlayerState { get; private set; } = false;
        public bool hasSceneState  { get; private set; } = false;

#region Adding

        /**
         * <summary>
         * Adds the player state to the data store.
         * </summary>
         */
        public void AddPlayer(SavedPlayer player) {
            this.player = player;
            hasPlayerState = true;
        }

        /**
         * <summary>
         * Adds an object to the data store.
         * </summary>
         * <param name="obj">The BaseSaved to add</param>
         */
        public void Add(BaseSaved obj) {
            LogDebug($"Adding {obj.GetType()}: {obj.id}");

            switch (obj) {
                case SavedAnimation animation:
                    animations[animation.id] = animation;
                    break;
                case SavedBrick brick:
                    bricks[brick.id] = brick;
                    break;
                case SavedBrittleIce brittleIce:
                    brittleIces[brittleIce.id] = brittleIce;
                    break;
                case SavedCrumblingHold crumblingHold:
                    crumblingHolds[crumblingHold.id] = crumblingHold;
                    break;
                case SavedJoint joint:
                    joints[joint.id] = joint;
                    break;
                default:
                    LogError($"Trying to save unrecognised type: {obj.GetType()}");
                    throw new Exception();
            }

            hasSceneState = true;
        }

#endregion

#region Getting

        /**
         * <summary>
         * Gets the player state from the data store.
         * </summary>
         */
        public SavedPlayer GetPlayer() {
            return player;
        }

        /**
         * <summary>
         * Gets objects from the data store.
         * </summary>
         * <param name="id">The ID of the object to find</param>
         * <returns>The object if found, null otherwise</returns>
         */
        public SavedAnimation GetAnimation(string id) {
            if (animations.ContainsKey(id) == true) {
                return animations[id];
            }

            return null;
        }

        public SavedBrick GetBrick(string id) {
            if (bricks.ContainsKey(id) == true) {
                return bricks[id];
            }

            return null;
        }

        public SavedBrittleIce GetBrittleIce(string id) {
            if (brittleIces.ContainsKey(id) == true) {
                return brittleIces[id];
            }

            return null;
        }

        public SavedCrumblingHold GetCrumblingHold(string id) {
            if (crumblingHolds.ContainsKey(id) == true) {
                return crumblingHolds[id];
            }

            return null;
        }

        public SavedJoint GetJoint(string id) {
            if (joints.ContainsKey(id) == true) {
                return joints[id];
            }

            return null;
        }

#endregion

#region Saving/Loading

        /**
         * <summary>
         * Writes a section of the data store to a CBORObject
         * if it has any data.
         * </summary>
         * <param name="root">The CBORObject to add to</param>
         * <param name="key">The key to store the data with</param>
         * <param name="section">The section to write</param>
         */
        public void WriteSection<T>(
            CBORObject root,
            string key,
            Dictionary<string, T> section
        ) where T : BaseSaved {
            if (section.Count < 1) {
                return;
            }

            CBORObject array = CBORObject.NewArray();
            foreach (T item in section.Values) {
                array.Add(item.ToCBOR());
            }

            root.Add(key, array);
        }

        /**
         * <summary>
         * Reads a section from a CBORObject and stores
         * it in the provided section of the data store.
         * </summary>
         * <param name="root">The CBORObject to read a section from</param>
         * <param name="key">The name of the section to read</param>
         * <param name="section">The section to store into</param>
         */
        public void ReadSection<T>(
            CBORObject root,
            string key,
            Dictionary<string, T> section
        ) where T : BaseSaved, new() {
            try {
                if (root.ContainsKey(key) == false) {
                    LogDebug($"Skipped reading section: {key}, no data found");
                    return;
                }

                LogDebug($"Reading section: {key}");

                CBORObject array = root[key];

                for (int i = 0; i < array.Count; i++) {
                    T item = new T();
                    item.FromCBOR(array[i]);
                    section.Add(item.id, item);
                    LogDebug($"Read object: {item.GetType()}({item.id})");
                }

                // Indicate a scene state exists
                hasSceneState = true;
            }
            catch (Exception e) {
                LogDebug($"Exception occurred while reading section \"{key}\": {e}");
            }
        }

        /**
         * <summary>
         * Serializes the currently stored objects into a CBORObject.
         * </summary>
         * <returns>The CBORObject</returns>
         */
        public CBORObject ToCBOR() {
            CBORObject root = CBORObject.NewMap();

            // Add the player if there is a state
            if (player != null) {
                root.Add("player", player.ToCBOR());
            }

            // Write sections to root CBOR object
            WriteSection<SavedAnimation>(root, "animations", animations);
            WriteSection<SavedBrick>(root, "bricks", bricks);
            WriteSection<SavedBrittleIce>(root, "brittleIces", brittleIces);
            WriteSection<SavedCrumblingHold>(root, "crumblingHolds", crumblingHolds);
            WriteSection<SavedJoint>(root, "joints", joints);

            return root;
        }

        /**
         * <summary>
         * Deserializes a CBORObject into the save data.
         * </summary>
         * <param name="root">The CBORObject to deserialize</param>
         */
        public void FromCBOR(CBORObject root) {
            // Load player data
            try {
                if (root.ContainsKey("player") == true) {
                    player = new SavedPlayer(root["player"]);
                    hasPlayerState = true;
                }
            }
            catch (Exception e) {
                WipePlayer();
                LogDebug($"Exception occurred while loading player data: {e}");
            }

            // Load scene data
            ReadSection<SavedAnimation>(root, "animations", animations);
            ReadSection<SavedBrick>(root, "bricks", bricks);
            ReadSection<SavedBrittleIce>(root, "brittleIces", brittleIces);
            ReadSection<SavedCrumblingHold>(root, "crumblingHolds", crumblingHolds);
            ReadSection<SavedJoint>(root, "joints", joints);

            LogDebug($"Loaded data: {root.ToJSONString()}");
        }

#endregion


#region Wiping Data

    /**
     * <summary>
     * Wipes all scene data.
     *
     * IMPORTANT: This doesn't save anything, it just
     * clears the data store.
     * </summary>
     */
    public void WipePlayer() {
        player = null;

        // Indicate no player data
        hasPlayerState = false;

        LogDebug("Wiped player data");
    }

    /**
     * <summary>
     * Wipes all player data.
     *
     * IMPORTANT: This doesn't save anything, it just
     * clears the data store.
     * </summary>
     */
    public void WipeScene() {
        animations.Clear();
        bricks.Clear();
        brittleIces.Clear();
        crumblingHolds.Clear();
        joints.Clear();

        // Indicate no scene data
        hasSceneState = false;

        LogDebug("Wiped scene data");
    }

#endregion

    }
}
