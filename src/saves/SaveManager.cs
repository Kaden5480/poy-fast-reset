using System;
using System.Collections.Generic;
using System.IO;
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
        public static string configDir { get; } = Path.Combine(
            BepInEx.Paths.ConfigPath, "com.github.Kaden5480.poy-fast-reset"
        );

        private const string stateFileName = "state.dat";

        // The path to the file containing state data
        private string stateDirPath = null;
        private string stateFilePath = null;

        // The saved player state for the scene
        private SavedPlayer player = null;

        // Dictionaries mapping IDs to saved objects
        //private Dictionary<string, SavedAnimation> animations;
        //private Dictionary<string, SavedBrittleIce> brittleIces;
        //private Dictionary<string, SavedCrumblingHold> crumblingHolds;
        private Dictionary<byte[], SavedJoint> joints = new Dictionary<byte[], SavedJoint>();

        // What states exist for this scene
        public static bool hasPlayerState = false;
        public static bool hasSceneState = false;

        // An instance of SaveManager accessible statically
        private static SaveManager instance = null;

        public SaveManager() {
            instance = this;
        }

#region Adding/Getting

        /**
         * <summary>
         * Adds an object to the data store.
         * </summary>
         * <param name="obj">The BaseSaved to add</param>
         */
        public static void Add(BaseSaved obj) {
            instance.LogDebug($"Adding object: {obj.id}");

            switch (obj) {
                case SavedJoint joint:
                    instance.joints[joint.id] = joint;
                    break;
                default:
                    instance.LogError($"Trying to save unrecognised type: {obj.GetType()}");
                    throw new Exception();
            }

            hasSceneState = true;
        }

        /**
         * <summary>
         * Gets an object from the data store.
         * </summary>
         * <param name="id">The ID of the object to find</param>
         * <returns>The object if found, null otherwise</returns>
         */
        public static SavedJoint GetJoint(byte[] id) {
            instance.LogDebug($"Getting saved joint: {System.BitConverter.ToString(id)}");

            if (instance.joints.ContainsKey(id) == true) {
                return instance.joints[id];
            }

            return null;
        }

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
         * Gets the player state from the data store.
         * </summary>
         */
        public static SavedPlayer GetPlayer() {
            return instance.player;
        }

#endregion

#region Serializing/Deserializing Types

        /**
         * <summary>
         * Converts a Quaternion to a byte array.
         * </summary>
         * <param name="quat">The Quaternion to convert</param>
         * <returns>The Quaternion as bytes</returns>
         */
        public static byte[] QuatToBytes(Quaternion quat) {
            using (MemoryStream stream = new MemoryStream()) {
                using (BinaryWriter writer = new BinaryWriter(stream)) {
                    writer.Write(quat.x);
                    writer.Write(quat.y);
                    writer.Write(quat.z);
                    writer.Write(quat.w);

                    return stream.ToArray();
                }
            }
        }

        /**
         * <summary>
         * Converts bytes into a quaternion.
         * </summary>
         * <param name="bytes">The bytes to convert</param>
         * <returns>The quaternion</returns>
         */
        public static Quaternion BytesToQuat(byte[] bytes) {
            float x, y, z, w;

            using (MemoryStream stream = new MemoryStream(bytes)) {
                using (BinaryReader reader = new BinaryReader(stream)) {
                    x = reader.ReadSingle();
                    y = reader.ReadSingle();
                    z = reader.ReadSingle();
                    w = reader.ReadSingle();
                }
            }

            return new Quaternion(x, y, z, w);
        }

        /**
         * <summary>
         * Converts a Vector3 to a byte array.
         * </summary>
         * <param name="vec">The Vector3 to convert</param>
         * <returns>The Vector3 as bytes</returns>
         */
        public static byte[] Vec3ToBytes(Vector3 vec) {
            using (MemoryStream stream = new MemoryStream()) {
                using (BinaryWriter writer = new BinaryWriter(stream)) {
                    writer.Write(vec.x);
                    writer.Write(vec.y);
                    writer.Write(vec.z);

                    return stream.ToArray();
                }
            }
        }

        /**
         * <summary>
         * Converts bytes into a Vector3.
         * </summary>
         * <param name="bytes">The bytes to convert</param>
         * <returns>The Vector3</returns>
         */
        public static Vector3 BytesToVec3(byte[] bytes) {
            float x, y, z;

            using (MemoryStream stream = new MemoryStream(bytes)) {
                using (BinaryReader reader = new BinaryReader(stream)) {
                    x = reader.ReadSingle();
                    y = reader.ReadSingle();
                    z = reader.ReadSingle();

                }
            }

            return new Vector3(x, y, z);
        }

#endregion

#region Saving/Loading

        /**
         * <summary>
         * Saves the currently stored objects to the data store.
         * </summary>
         */
        public void Save() {
            // If there is no state path, do nothing
            if (stateDirPath == null || stateFilePath == null) {
                return;
            }

            // If there is no state to store, do nothing
            if (hasPlayerState == false && hasSceneState == false) {
                return;
            }

            // Construct a CBOR object holding everything
            CBORObject root = CBORObject.NewMap();

            // Maps for each type
            if (joints.Count > 0) {
                CBORObject jointArray = CBORObject.NewArray();

                foreach (SavedJoint joint in joints.Values) {
                    jointArray = jointArray.Add(joint.ToCBOR());
                }

                root = root.Add("joints", jointArray);
            }

            // Add the player if there is a state
            if (player != null) {
                root = root.Add("player", player.ToCBOR());
            }

            // Save the object to a file
            if (Directory.Exists(stateDirPath) == false) {
                Directory.CreateDirectory(stateDirPath);
            }

            LogDebug($"Saving: {root.ToJSONString()}");

            using (FileStream stream = new FileStream(stateFilePath, FileMode.Create)) {
                root.WriteTo(stream);
            }
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

            hasPlayerState = false;
            hasSceneState = false;
            player = null;
            joints.Clear();

            // Load data
            string profile = Plugin.instance.config.profile.Value;
            string sceneName = Plugin.instance.cache.scene.name;

            // Where the data should be loaded from
            stateDirPath = Path.Combine(
                configDir, profile, sceneName
            );
            stateFilePath = Path.Combine(
                stateDirPath, stateFileName
            );

            // Check if the state file exists
            if (File.Exists(stateFilePath) == false) {
                LogDebug("No data found for current profile and scene");
                return;
            }

            LogDebug("Found data for current scene");

            // Try loading the data
            CBORObject root = CBORObject.DecodeFromBytes(
                File.ReadAllBytes(stateFilePath)
            );

            // Load each type
            if (root.ContainsKey("player") == true) {
                player = new SavedPlayer(root["player"]);
                hasPlayerState = true;
            }
            if (root.ContainsKey("joints") == true) {
                CBORObject jointArray = root["joints"];
                for (int i = 0; i < jointArray.Count; i++) {
                    SavedJoint joint = new SavedJoint(jointArray[i]);
                    joints.Add(joint.id, joint);
                }
                hasSceneState = true;
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

    }
}
