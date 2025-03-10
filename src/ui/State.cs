using System.Collections.Generic;
using System.IO;

using UnityEngine;

using Cfg = FastReset.Config.Cfg;
using Paths = FastReset.Config.Paths;

namespace FastReset.UI {
    public enum Pane {
        Main,
        Profiles,
    };

    public class State {
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
        public bool editPlayerState = true;
        public bool editSceneState = true;
        public string newProfile = "";
        public Pane pane = Pane.Main;

        /**
         * <summary>
         * Load a list of currently the selected profile and any other
         * profiles which exist in the config directory.
         * </summary>
         */
        public void LoadProfiles() {
            Plugin.LogDebug("UI.State: Loading profiles");
            profiles.Clear();

            // Always add selected profile
            Plugin.LogDebug($"UI.State: currently selected {currentProfile}");
            profiles.Add(new Profile(currentProfile, true));

            foreach (string path in Directory.GetDirectories(Paths.configDir)) {
                string profile = Path.GetFileName(path);

                // Add anything apart from selected
                if (currentProfile.Equals(profile) == true) {
                    continue;
                }

                Plugin.LogDebug($"UI.State: found profile {profile}");
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
                Plugin.LogDebug("UI.State: Tried adding empty profile");
                return;
            }

            foreach (Profile p in profiles) {
                if (profile.Equals(p.name) == true) {
                Plugin.LogDebug("UI.State: Tried adding already existing profile");
                    return;
                }
            }

            Plugin.LogDebug($"Adding profile: {profile}");
            newProfile = "";
            Directory.CreateDirectory(Path.Combine(Paths.configDir, profile));
            LoadProfiles();
        }

        /**
         * <summary>
         * Deletes a profile.
         * </summary>
         */
        public void DeleteProfile(Profile profile) {
            Plugin.LogDebug($"UI.State: Deleting profile {profile.name}");
            profile.Delete();
            profiles.Remove(profile);
        }
    }
}
