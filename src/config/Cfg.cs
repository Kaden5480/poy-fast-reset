using System;
using System.IO;

using BepInEx.Configuration;
using UnityEngine;

namespace FastReset.Config {
    /**
     * <summary>
     * A class containing the config for fast reset.
     * This does not include player/scene state as these
     * are handled separately.
     * </summary>
     */
    public class Cfg {
        // Save/reset keybinds
        public ConfigEntry<KeyCode> saveKeybind;
        public ConfigEntry<KeyCode> resetKeybind;
        public ConfigEntry<KeyCode> toggleModifier;

        // The currently selected profile
        public ConfigEntry<string> profile;

        // Whether to reset the wind on wuthering crest
        public ConfigEntry<bool> resetWind;

        /**
         * <summary>
         * Converts a Vector3 to a string for
         * storing in config files.
         * </summary>
         * <param name="vec">The Vector3 to convert</param>
         * <returns>The string representation of the Vector3</returns>
         */
        public static string Vec3ToString(Vector3 vec) {
            return $"{vec.x};{vec.y};{vec.z}";
        }

        /**
         * <summary>
         * Converts a string representation of a Vector3 to a Vector3.
         * </summary>
         * <param name="str">A string representation of a Vector3</param>
         * <returns>The Vector3</returns>
         */
        public static Vector3 StringToVec3(string str) {
            string[] parts = str.Split(';');

            return new Vector3(
                Single.Parse(parts[0]),
                Single.Parse(parts[1]),
                Single.Parse(parts[2])
            );
        }

        /**
         * <summary>
         * Converts a Quaternion to a string for
         * storing in config files.
         * </summary>
         * <param name="quat">The Quaternion to convert</param>
         * <returns>The string representation of the Quaternion</returns>
         */
        public static string QuatToString(Quaternion quat) {
            return $"{quat.x};{quat.y};{quat.z};{quat.w}";
        }

        /**
         * <summary>
         * Converts a string representation of a Quaternion to a Quaternion.
         * </summary>
         * <param name="str">A string representation of a Quaternion</param>
         * <returns>The Quaternion</returns>
         */
        public static Quaternion StringToQuat(string str) {
            string[] parts = str.Split(';');

            return new Quaternion(
                Single.Parse(parts[0]),
                Single.Parse(parts[1]),
                Single.Parse(parts[2]),
                Single.Parse(parts[3])
            );
        }
    }
}
