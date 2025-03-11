using System.IO;

using Cfg = FastReset.Config.Cfg;
using SaveManager = FastReset.Saves.SaveManager;

namespace FastReset.UI {
    public class Profile : Loggable {
        private static Profile _selected;
        public static Profile selected {
            get => _selected;
        }

        public bool pendingDeletion;
        public string name;

        private static Cfg config {
            get => Plugin.instance.config;
        }

        private string profileDir {
            get => Path.Combine(
                SaveManager.configDir, name
            );
        }

        public Profile(string name, bool selected = false) {
            this.name = name;

            pendingDeletion = false;

            if (selected == true) {
                Profile._selected = this;
            }

        }

        public static void Select(Profile profile) {
            _selected = profile;
            profile.pendingDeletion = false;
            config.profile.Value = profile.name;

            SaveManager.Reload();
        }

        public void Delete() {
            LogDebug($"Deleting {profileDir}");
            if (Directory.Exists(profileDir) == true) {
                Directory.Delete(profileDir, true);
            }
        }
    }
}
