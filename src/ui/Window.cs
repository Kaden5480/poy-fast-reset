using System;
using System.Text;
using UnityEngine;

using Cfg = FastReset.Config.Cfg;
using StateManager = FastReset.State.StateManager;

namespace FastReset.UI {
    public class Window : Loggable {
        private Cfg config {
            get => Plugin.instance.config;
        }

        private Cache cache {
            get => Plugin.instance.cache;
        }

        private Resetter resetter {
            get => Plugin.instance.resetter;
        }

        private State state { get; } = new State();

        private const float height = 370;
        private const float width = 386;
        private const float padding = 20;
        private const float elementWidth = 100;
        private const float smallElementWidth = 50;

        private string profileTextPaddingLeft = "";
        private string profileTextPaddingRight = "";
        private const string availablePadding = "===============";
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
            if (cache.inGameMenu == null) {
                LogDebug("inGameMenu is null");
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
                InGameMenu.hasBeenInMenu = false;
            }
        }

        /**
         * <summary>
         * Renders options for a profile.
         * </summary>
         */
        private void RenderProfile(Profile profile) {
            GUILayout.Label($"{profile.name}");

            if (GUILayout.Button("Use") == true) {
                Profile.Select(profile);
                Recalculate();
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
            GUILayout.Space(13);
            GUILayout.FlexibleSpace();

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            state.newProfile = GUILayout.TextField(
                state.newProfile
            );
            if (GUILayout.Button("Add Profile", GUILayout.Width(elementWidth)) == true) {
                state.AddProfile();
                Recalculate();
            }
            GUILayout.EndHorizontal();

            GUILayout.Label($"{availablePadding} Available Profiles {availablePadding}");

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

            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
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
            config.modifyPlayerState.Value = GUILayout.Toggle(
                config.modifyPlayerState.Value, "Save player state"
            );
            config.modifySceneState.Value = GUILayout.Toggle(
                config.modifySceneState.Value, "Save scene state"
            );

            GUILayout.Space(5);

            GUILayout.Label("When restoring:");
            config.restorePlayerState.Value = GUILayout.Toggle(
                config.restorePlayerState.Value, "Restore player state"
            );
            config.restoreSceneState.Value = GUILayout.Toggle(
                config.restoreSceneState.Value, "Restore scene state"
            );
            config.useInitialPlayerState.Value = GUILayout.Toggle(
                config.useInitialPlayerState.Value, "Use initial player state"
            );
            config.useInitialSceneState.Value = GUILayout.Toggle(
                config.useInitialSceneState.Value, "Use initial scene state"
            );
            config.resetWind.Value = GUILayout.Toggle(
                config.resetWind.Value, "Reset wind"
            );

            GUILayout.Label("Fast coffee:");
            config.reapplyCoffee.Value = GUILayout.Toggle(
                config.reapplyCoffee.Value, "Reapply coffee"
            );
            config.slurpSound.Value = GUILayout.Toggle(
                config.slurpSound.Value, "Slurp sound"
            );

            GUILayout.EndVertical();

            // Information
            GUILayout.BeginVertical();

            GUILayout.Label($"{stateTextPadding} Player State {stateTextPadding}");
            GUILayout.Label($"Normal: {IsAvailable(StateManager.hasPlayerSaved)}");
            GUILayout.Label($"Routing flag mode: {IsAvailable(StateManager.hasPlayerTemp)}");
            GUILayout.Space(height / 10);
            GUILayout.Label($"{stateTextPadding} Scene State {stateTextPadding}");
            GUILayout.Label($"Normal: {IsAvailable(StateManager.hasSceneSaved)}");
            GUILayout.Label($"Routing flag mode: {IsAvailable(StateManager.hasSceneTemp)}");

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


            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label($"{profileTextPaddingLeft} Current Profile: {state.currentProfile} {profileTextPaddingRight}");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

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
                Disable();
            }

            GUILayout.EndArea();
        }

        /**
         * <summary>
         * Load a list of currently the selected profile and any other
         * profiles which exist in the config directory.
         * </summary>
         */
        public void LoadProfiles() {
            state.LoadProfiles();
            Recalculate();
        }

        /**
         * <summary>
         * Recalculates the profile text padding.
         * </summary>
         */
        public void Recalculate() {
            int totalLength = 53;

            int textLength = 19 + state.currentProfile.Length;
            int paddingCount = (totalLength - textLength) / 2;

            StringBuilder leftBuilder = new StringBuilder();
            leftBuilder.Append('=', paddingCount);

            StringBuilder rightBuilder = new StringBuilder();
            rightBuilder.Append('=', paddingCount);

            if (((totalLength - textLength) & 1) != 0) {
                rightBuilder.Append('=');
            }

            profileTextPaddingLeft = leftBuilder.ToString();
            profileTextPaddingRight = rightBuilder.ToString();

        }
    }
}
