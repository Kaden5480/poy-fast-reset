using System.Collections.Generic;
using System.IO;

using UnityEngine;

using Cfg = FastReset.Config.Cfg;
using SaveManager = FastReset.Saves.SaveManager;

namespace FastReset.UI {
    public enum Pane {
        Main,
        Profiles,
    };

    public class State : Loggable {
        // Shorthand for accessing config
        private Cfg config {
            get => Plugin.instance.config;
        }

        // The current profile
        public string currentProfile {
            get => config.profile.Value;
        }

        // Profiles available
        public List<Profile> profiles = new List<Profile>();

        // UI states
        public Vector2 scrollPosition = Vector2.zero;

        public string newProfile = "";
        public Pane pane = Pane.Main;

        /**
         * <summary>
         * Load a list of currently the selected profile and any other
         * profiles which exist in the config directory.
         * </summary>
         */
        public void LoadProfiles() {
            LogDebug("Loading profiles");
            profiles.Clear();

            // Always add selected profile
            LogDebug($"currently selected {currentProfile}");
            profiles.Add(new Profile(currentProfile, true));

            if (Directory.Exists(SaveManager.configDir) == false) {
                return;
            }

            foreach (string path in Directory.GetDirectories(SaveManager.configDir)) {
                string profile = Path.GetFileName(path);

                // Add anything apart from selected
                if (currentProfile.Equals(profile) == true) {
                    continue;
                }

                LogDebug($"found profile {profile}");
                profiles.Add(new Profile(profile));
            }
        }

        /**
         * <summary>
         * Adds a new profile.
         * </summary>
         */
        public void AddProfile() {
            string profile = newProfile.Trim();
            if (profile.Length < 1) {
                LogDebug("Tried adding empty profile");
                return;
            }

            foreach (Profile p in profiles) {
                if (profile.Equals(p.name) == true) {
                    LogDebug("Tried adding already existing profile");
                    return;
                }
            }

            LogDebug($"Adding profile: {profile}");
            newProfile = "";
            Directory.CreateDirectory(Path.Combine(SaveManager.configDir, profile));
            LoadProfiles();
        }

        /**
         * <summary>
         * Deletes a profile.
         * </summary>
         */
        public void DeleteProfile(Profile profile) {
            LogDebug($"Deleting profile {profile.name}");
            profile.Delete();
            profiles.Remove(profile);
        }
    }
}
