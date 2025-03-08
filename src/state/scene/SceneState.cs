using System.Collections.Generic;

using BepInEx.Configuration;
using UnityEngine;

using Cfg = FastReset.Config.Cfg;
using Paths = FastReset.Config.Paths;

namespace FastReset.State {
    public class SceneState {
        // Shorthand for accessing cache and config
        private static Cache cache {
            get => Plugin.instance.cache;
        }
        public static Cfg config {
            get => Plugin.instance.config;
        }

        // File where the animation states are stored
        public static ConfigFile animationFile = null;

        // Initial, temporary, and saved states of objects
        private List<TrackedObject> initial = new List<TrackedObject>();
        private List<TrackedObject> temporary = null;
        private Dictionary<string, TrackedObject> saved = null;

        /**
         * <summary>
         * Saves the initial state.
         * </summary>
         */
        private void SaveInitialState() {
            initial.Clear();
            foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>()) {
                TrackedObject tracked = new TrackedObject(obj);
                tracked.SaveState();
                initial.Add(tracked);
            }
        }

        /**
         * <summary>
         * Restores the initial state.
         * </summary>
         */
        private void RestoreInitialState() {
            foreach (TrackedObject obj in initial) {
                obj.RestoreState();
            }
        }

        /**
         * <summary>
         * Saves the scene state temporarily.
         * </summary>
         */
        private void SaveTempState() {
            if (temporary == null) {
                temporary = new List<TrackedObject>();
            }

            temporary.Clear();
            foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>()) {
                TrackedObject tracked = new TrackedObject(obj);
                tracked.SaveState();
                temporary.Add(tracked);
            }
        }

        /**
         * <summary>
         * Restores the temporarily saved scene state.
         *
         * This will default to the saved state (if it exists)
         * if a temporary state wasn't saved.
         * </summary>
         */
        private void RestoreTempState() {
            if (temporary == null) {
                return;
            }

            foreach (TrackedObject obj in temporary) {
                obj.RestoreState();
            }
        }

        /**
         * <summary>
         * Saves the current scene state.
         *
         * If the player is in routing flag mode, the state is
         * only saved temporarily.
         *
         * Otherwise, the state is saved to a file permanently.
         * </summary>
         */
        public void SaveState() {
            if (cache.routingFlag.currentlyUsingFlag == true) {
                SaveTempState();
                return;
            }
        }

        /**
         * <summary>
         * Restores the scene's state.
         *
         * If the player is in routing flag mode, the temporary
         * state is used. If the temporary state is unset, the saved
         * state is used.
         *
         * If the player isn't in routing flag mode, only the saved
         * state is used.
         * </summary>
         */
        public void RestoreState() {
            if (cache.routingFlag.currentlyUsingFlag == true) {
                RestoreTempState();
                return;
            }
        }

        /**
         * <summary>
         * Prepares configs for the current scene.
         * </summary>
         */
        public void OnSceneLoaded() {
            // Save the initial scene state
            SaveInitialState();

            animationFile = new ConfigFile(
                Paths.animationsPath, false
            );
        }

        /**
         * <summary>
         * Saves configs for the current scene and
         * prepares for the next scene load.
         * </summary>
         */
        public void OnSceneUnloaded() {
            animationFile.Save();

            animationFile = null;

            initial.Clear();
            temporary = null;
            saved = null;
        }

    }
}
