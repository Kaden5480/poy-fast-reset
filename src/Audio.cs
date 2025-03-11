using System.Collections;

using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace FastReset {
    public class Audio : Loggable {
        // The source to play the clips with
        private AudioSource source = null;

        // Audio clips for scene/player state saves/restores
        private AudioClip saveState = null;
        private AudioClip restoreState = null;
        private AudioClip failureState = null;

        /**
         * <summary>
         * Constructs an instance of Audio.
         * </summary>
         */
        public Audio() {
            // Create the audio game object
            LogDebug("Creating game object and audio source");
            GameObject sourceObj = new GameObject("Fast Reset Audio");
            GameObject.DontDestroyOnLoad(sourceObj);

            // Assign an AudioSource to it
            source = sourceObj.AddComponent<AudioSource>();

            // Bypass any effects
            source.bypassEffects = true;
            source.bypassListenerEffects = true;
            source.bypassReverbZones = true;
        }

        /**
         * <summary>
         * Finds the required audio clips.
         * </summary>
         */
        public void Load() {
            // If the clips have already been found, don't search
            if (restoreState != null && saveState != null && failureState != null) {
                return;
            }

            // Try finding the audio clips
            foreach (AudioClip clip in Resources.FindObjectsOfTypeAll(typeof(AudioClip))) {
                if ("ding".Equals(clip.name) == true) {
                    LogDebug($"Found save state clip: {clip.name}");
                    saveState = clip;
                }
                else if ("click".Equals(clip.name) == true) {
                    LogDebug($"Found restore state clip: {clip.name}");
                    restoreState = clip;
                }
                else if ("pickup2".Equals(clip.name) == true) {
                    LogDebug($"Found failure state clip: {clip.name}");
                    failureState = clip;
                }
            }

            // Also take the sfx mixer group
            foreach (AudioSource src in GameObject.FindObjectsOfType<AudioSource>()) {
                if (src.outputAudioMixerGroup == null) {
                    continue;
                }

                if ("SFX".Equals(src.outputAudioMixerGroup.name) == true) {
                    LogDebug($"Found SFX audio mixer group");
                    source.outputAudioMixerGroup = src.outputAudioMixerGroup;
                    break;
                }
            }
        }

        /**
         * <summary>
         * Plays the save state sound effect.
         * </summary>
         */
        public void PlaySave() {
            if (saveState == null) {
                LogDebug("Save state clip is null");
                return;
            }

            LogDebug("Playing save state clip");
            source.clip = saveState;
            source.volume = 0.33f;
            source.Play();
        }

        /**
         * <summary>
         * Plays the restore state sound effect.
         * </summary>
         */
        public void PlayRestore() {
            if (restoreState == null) {
                LogDebug("Restore state clip is null");
                return;
            }

            LogDebug("Playing restore state clip");
            source.clip = restoreState;
            source.volume = 0.25f;
            source.Play();
        }

        /**
         * <summary>
         * Plays the failure state sound effect.
         * </summary>
         */
        public void PlayFailure() {
            if (failureState == null) {
                LogDebug("Failure state clip is null");
                return;
            }

            LogDebug("Playing failure state clip");
            source.clip = failureState;
            source.volume = 0.4f;
            source.Play();
        }
    }
}
