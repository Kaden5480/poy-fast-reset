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
         * Finds the required audio clips.
         * </summary>
         */
        public void Load() {
            if (playerState != null && sceneState != null) {
                return;
            }

            foreach (AudioClip clip in Resources.FindObjectsOfTypeAll(typeof(AudioClip))) {
                if ("ding".Equals(clip.name) == true) {
                    Plugin.LogDebug($"Found scene state clip: {clip.name}");
                    sceneState = clip;
                }
                else if ("click".Equals(clip.name) == true) {
                    Plugin.LogDebug($"Found player state clip: {clip.name}");
                    playerState = clip;
                }
            }
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
