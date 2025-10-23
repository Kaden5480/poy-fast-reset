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
        // An instance of SaveManager accessible statically
        private static SaveManager instance = null;

        // If a state takes up more than this many
        // bytes, use compression
        private const int byteCompressionLimit = 256;

        // The directory where the save data will be stored
        public static string configDir { get; } = Path.Combine(
            BepInEx.Paths.ConfigPath, "com.github.Kaden5480.poy-fast-reset"
        );

        // The path to the file containing state data
        private string stateDirPath = null;
        private string stateFilePath = null;

        // The different save data for this scene
        private bool useNormalState = true;
        private int routingStateIndex = 0;

        private SaveData normalState = null;
        private List<SaveData> routingStates = new List<SaveData>();

        public SaveManager() {
            instance = this;
        }

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

#region Managing Current State

        /**
         * <summary>
         * Gets the save data for the current state.
         * </summary>
         * <param name="create">Whether to create the save data if it doesn't exist</param>
         */
        public static SaveData GetSave(bool create = false) {
            SaveData save = null;

            if (instance.useNormalState == true) {
                save = instance.normalState;
            }
            else if (instance.routingStateIndex >= 0
                && instance.routingStateIndex < instance.routingStates.Count
            ) {
                save = instance.routingStates[instance.routingStateIndex];
            }

            // If save data exists, return it
            if (save != null) {
                return save;
            }

            // If not creating, just return anyway
            if (create == false) {
                return save;
            }

            // Otherwise, make the save
            save = new SaveData();

            if (instance.useNormalState == true) {
                instance.normalState = save;
            }
            else {
                instance.routingStates.Add(save);
                instance.routingStateIndex = instance.routingStates.Count - 1;
            }

            return save;
        }

#endregion

#region Checking States

        /**
         * <summary>
         * Checks whether player state exists for the current state.
         * </summary>
         * <returns>True if it does, false otherwise</returns>
         */
        public static bool HasPlayerState() {
            SaveData state = GetSave();
            if (state == null) {
                return false;
            }

            return state.hasPlayerState;
        }

        /**
         * <summary>
         * Checks whether scene state exists for the current state.
         * </summary>
         * <returns>True if it does, false otherwise</returns>
         */
        public static bool HasSceneState() {
            SaveData state = GetSave();
            if (state == null) {
                return false;
            }

            return state.hasSceneState;
        }

#endregion

#region Switching States

        /**
         * <summary>
         * Switches to using the normal state.
         * </summary>
         */
        public static void UseNormal() {
            instance.useNormalState = true;
            instance.LogDebug("Using normal state");
        }

        /**
         * <summary>
         * Switches to using a specific routing flage state.
         * </summary>
         */
        public static void UseRouting(int index) {
            instance.useNormalState = false;
            instance.routingStateIndex = index;
            instance.LogDebug($"Using routing flag state {index}");
        }

#endregion

#region Saving/Loading

        /**
         * <summary>
         * Saves the currently stored save data to the data store.
         * </summary>
         */
        public void Save() {
            // If there is no state path, do nothing
            if (stateFilePath == null) {
                LogDebug("No state file path, not saving");
                return;
            }

            // If there is no state to store, do nothing
            if (normalState == null && routingStates.Count < 1) {
                LogDebug("No save data, not saving");
                return;
            }

            // Construct a CBOR object holding everything
            CBORObject root = CBORObject.NewMap();

            // Save the object to a file
            if (Directory.Exists(stateDirPath) == false) {
                Directory.CreateDirectory(stateDirPath);
            }

            CBORObject allStates = CBORObject.NewMap();

            // Serialize the data
            if (normalState != null) {
                allStates.Add("normal", normalState.ToCBOR());
                LogDebug("Storing normal state");
            }

            if (routingStates.Count > 0) {
                CBORObject states = CBORObject.NewArray();

                foreach (SaveData data in routingStates) {
                    states.Add(data.ToCBOR());
                }

                allStates.Add("routing", states);
                LogDebug("Storing routing states");
            }

            // Add all save data to the root object
            root.Add("states", allStates);

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
            normalState = null;
            routingStates.Clear();

            // Load data
            string profile = Plugin.instance.config.profile.Value;
            string sceneName = Plugin.instance.cache.scene.name;

            // Where the data should be loaded from
            stateDirPath = Path.Combine(
                configDir, profile
            );
            stateFilePath = Path.Combine(
                stateDirPath, $"{sceneName}.dat"
            );

            // Check if the state file exists
            if (File.Exists(stateFilePath) == false) {
                LogDebug("No data found for current profile and scene");
                return;
            }

            // Try loading the data
            CBORObject root = CBORObject.DecodeFromBytes(
                Decompress(File.ReadAllBytes(stateFilePath))
            );

            // Check for different states
            if (root.ContainsKey("states") == false) {
                LogDebug("No save states found, skipping");
                return;
            }

            CBORObject states = root["states"];

            if (states.ContainsKey("normal") == true) {
                normalState = new SaveData();
                normalState.FromCBOR(states["normal"]);
                LogDebug("Loaded normal state");
            }

            if (states.ContainsKey("routing") == true) {
                for (int i = 0; i < states["routing"].Count; i++) {
                    SaveData data = new SaveData();
                    data.FromCBOR(states["routing"][i]);
                    routingStates.Add(data);

                }

                LogDebug("Loaded routing states");
            }
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

    }
}
