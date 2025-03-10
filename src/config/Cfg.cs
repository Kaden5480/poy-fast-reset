using System;
using System.IO;

using BepInEx.Configuration;
using UnityEngine;

namespace FastReset.Config {
    public class Cfg {
        // Save/reset keybinds
        public ConfigEntry<KeyCode> saveKeybind;
        public ConfigEntry<KeyCode> resetKeybind;
        public ConfigEntry<KeyCode> toggleModifier;

        // The currently selected profile
        public ConfigEntry<string> profile;

        // Whether to reset the wind on wuthering crest
        public ConfigEntry<bool> resetWind;

        public static string Vec3ToString(Vector3 vec) {
            return $"{vec.x};{vec.y};{vec.z}";
        }

        public static Vector3 StringToVec3(string str) {
            string[] parts = str.Split(';');

            return new Vector3(
                Single.Parse(parts[0]),
                Single.Parse(parts[1]),
                Single.Parse(parts[2])
            );
        }

        public static string QuatToString(Quaternion quat) {
            return $"{quat.x};{quat.y};{quat.z};{quat.w}";
        }

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
