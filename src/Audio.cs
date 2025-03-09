using System.Collections;

using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace FastReset {
    public class Audio {
        // The source to play the clips with
        private AudioSource source = null;

        // Audio clips for scene/player state saves/restores
        private AudioClip sceneState = null;
        private AudioClip playerState = null;

        /**
         * <summary>
         * Constructs an instance of Audio.
         * </summary>
         */
        public Audio() {
            // Create the audio game object
            Plugin.LogDebug("Audio: Creating game object and audio source");
            GameObject sourceObj = new GameObject("Fast Reset Audio");
            GameObject.DontDestroyOnLoad(sourceObj);

            // Assign an AudioSource to it
            source = sourceObj.AddComponent<AudioSource>();
        }

        /**
         * <summary>
         * Loads audio clips from the game for later use.
         * </summary>
         */
        public IEnumerator Load() {
            // Load the great gales cabin
            Plugin.LogDebug("Audio: Loading audio clips from cabin scene");
            const int buildIndex = 1;

            AsyncOperation load = SceneManager.LoadSceneAsync(buildIndex, LoadSceneMode.Additive);
            while (load.isDone == false) {
                yield return null;
            }

            foreach (AudioSource source in GameObject.FindObjectsOfType<AudioSource>()) {
                if ("DirtDingSound".Equals(source.gameObject.name) == true) {
                    sceneState = source.clip;
                }
                else if ("ClickSound".Equals(source.gameObject.name) == true) {
                    playerState = source.clip;
                }
            }

            AsyncOperation unload = SceneManager.UnloadSceneAsync(buildIndex);
            while (unload.isDone == false) {
                yield return null;
            }

            Plugin.LogDebug("Audio: Finished loading audio clips from cabin scene");

            Plugin.LogDebug("Audio: Reloading scene");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        }

        /**
         * <summary>
         * Plays the scene state save sound effect.
         * </summary>
         */
        public void PlayScene() {
            if (sceneState == null) {
                Plugin.LogDebug("Audio: scene state clip is null");
                return;
            }

            Plugin.LogDebug("Audio: playing scene state clip");
            source.clip = sceneState;
            source.volume = 0.33f;
            source.Play();
        }

        /**
         * <summary>
         * Plays the player state save/restore sound effect.
         * </summary>
         */
        public void PlayPlayer() {
            if (playerState == null) {
                Plugin.LogDebug("Audio: player state clip is null");
                return;
            }

            Plugin.LogDebug("Audio: playing player state clip");
            source.clip = playerState;
            source.volume = 0.25f;
            source.Play();
        }
    }
}
