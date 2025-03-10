using System;
using UnityEngine;

using Cfg = FastReset.Config.Cfg;

namespace FastReset.UI {
    public class Window {
        private Cfg config {
            get => Plugin.instance.config;
        }

        private Cache cache {
            get => Plugin.instance.cache;
        }

        private Resetter resetter {
            get => Plugin.instance.resetter;
        }

        public State state { get; } = new State();

        private const float height = 300;
        private const float width = 386;
        private const float padding = 20;
        private const float elementWidth = 100;
        private const float smallElementWidth = 50;

        private const string profileTextPadding = "============";
        private const string stateTextPadding = "=======";

        private bool allowingMovement = true;
        private bool showUI = false;

        public bool showingUI {
            get => showUI;
        }

        /**
         * <summary>
         * Toggles whether movement is allowed.
         * </summary>
         */
        private void AllowMovement(bool allow) {
            allowingMovement = allow;

            if (cache.playerManager != null) {
                cache.playerManager.AllowPlayerControl(allow);
            }

            if (cache.peakSummited != null) {
                cache.peakSummited.DisableEverythingButClimbing(!allow);
            }
        }

        /**
         * <summary>
         * Checks whether the player is currently in a menu.
         * </summary>
         */
        private bool IsInMenu() {
            if (InGameMenu.isCurrentlyNavigationMenu == true) {
                return true;
            }

            if (cache.inGameMenu == null) {
                Plugin.LogDebug("UI.Window: inGameMenu is null");
                return false;
            }

            return cache.inGameMenu.isMainMenu == true
                || cache.inGameMenu.inMenu == true;
        }

        /**
         * <summary>
         * Toggles the UI.
         * </summary>
         */
        public void Toggle() {
            showUI = !showUI;
        }

        /**
         * <summary>
         * Forcefully disables the UI.
         * </summary>
         */
        public void Disable() {
            showUI = false;
        }

        /**
         * <summary>
         * Executes each frame to update state.
         * </summary>
         */
        public void Update() {
            if (InGameMenu.isLoading == true
                || EnterPeakScene.enteringPeakScene == true
                || EnterPeakScene.enteringAlpScene == true
                || EnterRoomSegmentScene.enteringScene == true
            ) {
                return;
            }

            // Toggle movement in very specific cases
            if (showUI == true) {
                AllowMovement(false);
                InGameMenu.isCurrentlyNavigationMenu = true;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                InGameMenu.hasBeenInMenu = true;
            }
            else if (showUI == false
                && allowingMovement == false
                && IsInMenu() == false
            ) {
                AllowMovement(true);
                InGameMenu.isCurrentlyNavigationMenu = false;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        /**
         * <summary>
         * Renders options for a profile.
         * </summary>
         */
        private void RenderProfile(Profile profile) {
            //GUILayout.Label($"== {profile.name} ==");
            GUILayout.Label($"{profile.name}");

            if (GUILayout.Button("Use") == true) {
                Profile.Select(profile);
            }

            if (profile.pendingDeletion == false
                && GUILayout.Button("Delete") == true
            ) {
                profile.pendingDeletion = true;
            }

            else if (profile.pendingDeletion == true) {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button($"Yes, delete {profile.name}", GUILayout.MinWidth(elementWidth)) == true) {
                    state.DeleteProfile(profile);
                }
                if (GUILayout.Button("No, cancel", GUILayout.MinWidth(elementWidth)) == true) {
                    profile.pendingDeletion = false;
                }
                GUILayout.EndHorizontal();
            }
        }

        /**
         * <summary>
         * Renders the pane for managing profiles
         * </summary>
         */
        public void RenderProfiles() {
            GUILayout.BeginHorizontal();
            state.newProfile = GUILayout.TextField(
                state.newProfile
            );
            if (GUILayout.Button("Add Profile", GUILayout.Width(elementWidth)) == true) {
                state.AddProfile();
            }
            GUILayout.EndHorizontal();

            GUILayout.Label("== Available Profiles ==");

            for (int i = 0; i < state.profiles.Count; i++) {
                Profile profile = state.profiles[i];
                if (profile.Equals(Profile.selected)) {
                    continue;
                }

                RenderProfile(profile);

                if (i < state.profiles.Count - 1) {
                    GUILayout.Space(padding / 2);
                }

            }
        }

        /**
         * <summary>
         * Converts true/false to available/unavailable.
         * </summary>
         */
        private string IsAvailable(bool available) {
            return (available == true) ? "Available" : "Unavailable";
        }

        /**
         * <summary>
         * Renders the main pane.
         * </summary>
         */
        /*
        public void RenderMain() {
            GUILayout.Label("== Player State ==", GUILayout.ExpandWidth(true));
            GUILayout.Label($"Routing flag mode: {IsAvailable(resetter.player.HasTempState())}");
            GUILayout.Label($"Normal: {IsAvailable(resetter.player.HasConfigState())}");

            state.editPlayerState = GUILayout.Toggle(
                state.editPlayerState, "Allow editing"
            );

            GUILayout.Space(padding);
            GUILayout.Label("== Scene State ==");
            GUILayout.Label($"Routing flag mode: {IsAvailable(resetter.scene.HasTempState())}");
            GUILayout.Label($"Normal: {IsAvailable(resetter.scene.HasConfigState())}");

            state.editSceneState = GUILayout.Toggle(
                state.editSceneState, "Allow editing"
            );

            state.useInitialState = GUILayout.Toggle(
                state.useInitialState, "Use initial state"
            );
        }
        */
        public void RenderMain() {
            GUILayout.BeginHorizontal();

            // Extra padding
            GUILayout.BeginVertical(GUILayout.Width(6));
            GUILayout.Space(6);
            GUILayout.EndVertical();

            // Options
            GUILayout.BeginVertical(GUILayout.Width(156));
            GUILayout.Space(4);
            GUILayout.Label("When saving:");
            state.editPlayerState = GUILayout.Toggle(
                state.editPlayerState, "Save player state"
            );
            state.editSceneState = GUILayout.Toggle(
                state.editSceneState, "Save scene state"
            );

            GUILayout.Space(5);

            GUILayout.Label("When restoring:");
            state.useInitialState = GUILayout.Toggle(
                state.useInitialState, "Use initial state"
            );

            GUILayout.EndVertical();

            // Information
            GUILayout.BeginVertical();

            GUILayout.Label($"{stateTextPadding} Player State {stateTextPadding}");
            GUILayout.Label($"Normal: {IsAvailable(resetter.player.HasConfigState())}");
            GUILayout.Label($"Routing flag mode: {IsAvailable(resetter.player.HasTempState())}");
            GUILayout.Space(30);
            GUILayout.Label($"{stateTextPadding} Scene State {stateTextPadding}");
            GUILayout.Label($"Normal: {IsAvailable(resetter.scene.HasConfigState())}");
            GUILayout.Label($"Routing flag mode: {IsAvailable(resetter.scene.HasTempState())}");

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        /**
         * <summary>
         * Renders the UI.
         * </summary>
         */
        public void Render() {
            if (showUI == false) {
                return;
            }

            int centerX = Screen.width / 2;
            int centerY = Screen.height / 2;

            float x = centerX - width / 2;
            float y = centerY - height / 2;

            GUILayout.BeginArea(new Rect(x, y, width, height), GUI.skin.box);
            GUILayout.Label($"{profileTextPadding} Current Profile: {state.currentProfile} {profileTextPadding}");

            state.scrollPosition = GUILayout.BeginScrollView(
                state.scrollPosition,
                GUILayout.Width(width - padding), GUILayout.Height(height - padding - 64)
            );

            switch (state.pane) {
                case Pane.Main:
                    RenderMain();
                    break;
                case Pane.Profiles:
                    RenderProfiles();
                    break;
            }


            GUILayout.EndScrollView();

            if (state.pane == Pane.Main
                && GUILayout.Button("Manage Profiles") == true
            ) {
                state.pane = Pane.Profiles;
            }

            if (state.pane == Pane.Profiles
                && GUILayout.Button("Back") == true
            ) {
                state.pane = Pane.Main;
            }

            if (GUILayout.Button("Close") == true) {
                showUI = false;
            }

            GUILayout.EndArea();
        }
    }
}
