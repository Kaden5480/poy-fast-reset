using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

using PeterO.Cbor;
using UnityEngine;
using UnityEngine.SceneManagement;

using Cfg = FastReset.Config.Cfg;
using Cache = FastReset.Cache;

namespace FastReset.Saves {
    /**
     * <summary>
     * A class which provides ease of use
     * for interacting with fast reset's saved data.
     * </summary>
     */
    public class SaveManager : Loggable {
        // The directory where the save data will be stored
        public static string configDirName = "com.github.Kaden5480.poy-fast-reset";
        public static string configDir { get; } = Path.Combine(
            BepInEx.Paths.ConfigPath, configDirName
        );

        // If a state takes up more than this many
        // bytes, use compression
        private const int byteCompressionLimit = 256;

        // The path to the file containing state data
        private string stateDirPath = null;
        private string stateFilePath = null;

        // The saved player state for the scene
        private SavedPlayer player = null;

        // Dictionaries mapping IDs to saved objects
        private Dictionary<string, SavedAnimation> animations = new Dictionary<string, SavedAnimation>();
        private Dictionary<string, SavedBrick> bricks = new Dictionary<string, SavedBrick>();
        private Dictionary<string, SavedBrittleIce> brittleIces = new Dictionary<string, SavedBrittleIce>();
        private Dictionary<string, SavedCrumblingHold> crumblingHolds = new Dictionary<string, SavedCrumblingHold>();
        private Dictionary<string, SavedJoint> joints = new Dictionary<string, SavedJoint>();

        // What states exist for this scene
        public static bool hasPlayerState = false;
        public static bool hasSceneState = false;

        // An instance of SaveManager accessible statically
        private static SaveManager instance = null;

        public SaveManager() {
            instance = this;
        }

#region Parsing Paths

        /**
         * <summary>
         * Analyzes a path to determine whether it's a workshop path or a local path.
         * Used for determining the type of custom level currently loaded.
         * </summary>
         * <param name="path">The path to check</param>
         * <returns>True if it's a workshop path, false otherwise</returns>
         */
        public bool IsWorkshopLevel(string path) {
            path = path.Replace("\\", "/");
            return path.Contains("Steam/steamapps/workshop/content");
        }

#endregion

#region Adding

        /**
         * <summary>
         * Adds the player state to the data store.
         * </summary>
         */
        public static void AddPlayer(SavedPlayer player) {
            instance.player = player;
            hasPlayerState = true;
        }

        /**
         * <summary>
         * Adds an object to the data store.
         * </summary>
         * <param name="obj">The BaseSaved to add</param>
         */
        public static void Add(BaseSaved obj) {
            instance.LogDebug($"Adding {obj.GetType()}: {obj.id}");

            switch (obj) {
                case SavedAnimation animation:
                    instance.animations[animation.id] = animation;
                    break;
                case SavedBrick brick:
                    instance.bricks[brick.id] = brick;
                    break;
                case SavedBrittleIce brittleIce:
                    instance.brittleIces[brittleIce.id] = brittleIce;
                    break;
                case SavedCrumblingHold crumblingHold:
                    instance.crumblingHolds[crumblingHold.id] = crumblingHold;
                    break;
                case SavedJoint joint:
                    instance.joints[joint.id] = joint;
                    break;
                default:
                    instance.LogError($"Trying to save unrecognised type: {obj.GetType()}");
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
        public static SavedPlayer GetPlayer() {
            return instance.player;
        }

        /**
         * <summary>
         * Gets objects from the data store.
         * </summary>
         * <param name="id">The ID of the object to find</param>
         * <returns>The object if found, null otherwise</returns>
         */
        public static SavedAnimation GetAnimation(string id) {
            if (instance.animations.ContainsKey(id) == true) {
                return instance.animations[id];
            }

            return null;
        }

        public static SavedBrick GetBrick(string id) {
            if (instance.bricks.ContainsKey(id) == true) {
                return instance.bricks[id];
            }

            return null;
        }

        public static SavedBrittleIce GetBrittleIce(string id) {
            if (instance.brittleIces.ContainsKey(id) == true) {
                return instance.brittleIces[id];
            }

            return null;
        }

        public static SavedCrumblingHold GetCrumblingHold(string id) {
            if (instance.crumblingHolds.ContainsKey(id) == true) {
                return instance.crumblingHolds[id];
            }

            return null;
        }

        public static SavedJoint GetJoint(string id) {
            if (instance.joints.ContainsKey(id) == true) {
                return instance.joints[id];
            }

            return null;
        }

#endregion

#region Serializing/Deserializing Types

        /**
         * <summary>
         * Converts a byte array into a string representation.
         * </summary>
         * <returns>The bytes as a hex string</returns>
         */
        public static string BytesToString(byte[] bytes) {
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
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
         * Saves the currently stored objects to the data store.
         * </summary>
         */
        public void Save() {
            // If there is no state path, do nothing
            if (stateFilePath == null) {
                LogDebug("No state file path, not saving");
                return;
            }

            // If there is no state to store, do nothing
            if (hasPlayerState == false && hasSceneState == false) {
                LogDebug("No player or scene state, not saving");
                return;
            }

            // Construct a CBOR object holding everything
            CBORObject root = CBORObject.NewMap();

            // Write sections to root CBOR object
            WriteSection<SavedAnimation>(root, "animations", animations);
            WriteSection<SavedBrick>(root, "bricks", bricks);
            WriteSection<SavedBrittleIce>(root, "brittleIces", brittleIces);
            WriteSection<SavedCrumblingHold>(root, "crumblingHolds", crumblingHolds);
            WriteSection<SavedJoint>(root, "joints", joints);

            // Add the player if there is a state
            if (player != null) {
                root.Add("player", player.ToCBOR());
            }

            // Save the object to a file
            if (Directory.Exists(stateDirPath) == false) {
                Directory.CreateDirectory(stateDirPath);
            }

            byte[] bytes = root.EncodeToBytes();

            // Store large states with compression
            LogDebug($"Saving: {root.ToJSONString()}");
            File.WriteAllBytes(stateFilePath, Compress(bytes));
        }

        /**
         * <summary>
         * Loads objects from the data store.
         *
         * This must be called whenever the scene/profile changes.
         *
         * It will reload the scene and profile paths, along with the
         * saved state for the scene and profile.
         * </summary>
         */
        public void Load() {
            // Wipe any stored data
            stateDirPath = null;
            stateFilePath = null;

            // Wiping these states is VERY important
            WipePlayer();
            WipeScene();

            // Load data
            Scene scene = Plugin.instance.cache.scene;
            string profile = Plugin.instance.config.profile.Value;


            // If custom level, use different paths
            stateDirPath = Path.Combine(
                configDir, profile
            );

            if (scene.buildIndex == 69) {
                string path = CustomLevelManager.control.desiredFolderPath;
                string compendiumName = CustomLevelManager.control.compendiumName;
                string peakName = CustomLevelManager.control.peakName;

                // Determine if workshop or local
                if (IsWorkshopLevel(path)) {
                    // Base path will be the workshop ID
                    stateDirPath = Path.Combine(
                        stateDirPath, "custom-levels", "workshop",
                        Path.GetFileName(path), compendiumName
                    );
                    LogDebug("Workshop custom level being loaded");
                }
                else {
                    stateDirPath = Path.Combine(
                        stateDirPath, "custom-levels", "local",
                        compendiumName
                    );
                    LogDebug("Local custom level being loaded");
                }

                stateFilePath = Path.Combine(
                    stateDirPath, $"{peakName}.dat"
                );
            }
            else {
                stateFilePath = Path.Combine(
                    stateDirPath, $"{scene.name}.dat"
                );
            }

            LogDebug($"Looking for data in {stateFilePath}");

            // Check if the state file exists
            if (File.Exists(stateFilePath) == false) {
                LogDebug("No data found for current profile and scene");
                return;
            }

            // Try loading the data
            CBORObject root = CBORObject.DecodeFromBytes(
                Decompress(File.ReadAllBytes(stateFilePath))
            );

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

        /**
         * <summary>
         * Reloads the state data.
         * Typically used for profile changes.
         * </summary>
         */
        public static void Reload() {
            instance.LogDebug("Reloading data");
            instance.Save();
            instance.Load();
        }

#endregion

#region Compression

        /**
         * <summary>
         * Compresses bytes using gzip.
         * </summary>
         * <param name="bytes">The bytes to compress</param>
         * <returns>The compressed bytes</returns>
         */
        private byte[] Compress(byte[] bytes) {
            if (bytes.Length < byteCompressionLimit) {
                LogDebug($"Not compressing data of size: {bytes.Length}");
                return bytes;
            }

            using (MemoryStream stream = new MemoryStream()) {
            using (GZipStream gzip = new GZipStream(stream, CompressionMode.Compress)) {
                gzip.Write(bytes, 0, bytes.Length);
                gzip.Close();

                return stream.ToArray();
            }}
        }

        /**
         * <summary>
         * Decompresses bytes using gzip.
         * </summary>
         * <param name="bytes">the bytes to decompress</param>
         * <returns>the decompressed bytes</returns>
         */
        private byte[] Decompress(byte[] bytes) {
            try {
                using (MemoryStream compressed = new MemoryStream(bytes)) {
                using (GZipStream gzip = new GZipStream(compressed, CompressionMode.Decompress)) {
                using (MemoryStream decompressed = new MemoryStream()) {
                    gzip.CopyTo(decompressed);
                    return decompressed.ToArray();
                }}}
            }
            catch (IOException) {
                LogDebug("Failed decompressing, data may not be compressed");
            }

            return bytes;
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
    public static void WipePlayer() {
        instance.player = null;

        // Indicate no player data
        hasPlayerState = false;

        instance.LogDebug("Wiped player data");
    }

    /**
     * <summary>
     * Wipes all player data.
     *
     * IMPORTANT: This doesn't save anything, it just
     * clears the data store.
     * </summary>
     */
    public static void WipeScene() {
        instance.animations.Clear();
        instance.bricks.Clear();
        instance.brittleIces.Clear();
        instance.crumblingHolds.Clear();
        instance.joints.Clear();

        // Indicate no scene data
        hasSceneState = false;

        instance.LogDebug("Wiped scene data");
    }

#endregion

    }
}
