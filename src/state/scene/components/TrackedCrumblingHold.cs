using BepInEx.Configuration;
using UnityEngine;

using Cfg = FastReset.Config.Cfg;
using Paths = FastReset.Config.Paths;

namespace FastReset.State {
    public class TrackedCrumblingHold : TrackedObject {
        private CrumblingHoldRegular holdComponent;

        private bool initialEnabled;
        private Vector3 initialScale;

        private bool temporaryEnabled;
        private Vector3 temporaryScale;

        private ConfigEntry<bool> configEnabled;
        private ConfigEntry<string> configScale;

        private void Restore(bool enabled, Vector3 scale) {
            obj.transform.localScale = scale;
            holdComponent.col1.enabled = enabled;
            holdComponent.col2.enabled = enabled;
            holdComponent.meshesHolder.SetActive(enabled);
        }

        public override void RestoreInitialState() {
            Plugin.LogDebug($"TrackedCrumblingHold: [{obj.name}] Restoring initial state");
            Restore(initialEnabled, initialScale);
        }

        public override void RestoreTempState() {
            Plugin.LogDebug($"TrackedCrumblingHold: [{obj.name}] Restoring temporary state");
            Restore(temporaryEnabled, temporaryScale);
        }

        public override void RestoreConfigState() {
            Plugin.LogDebug($"TrackedCrumblingHold: [{obj.name}] Restoring config state");
            Restore(configEnabled.Value, Cfg.StringToVec3(configScale.Value));
        }

        protected override void SaveInitialState() {
            Plugin.LogDebug($"TrackedCrumblingHold: [{obj.name}] Saving initial state");
            holdComponent = obj.GetComponent<CrumblingHoldRegular>();

            initialEnabled = holdComponent.meshesHolder.activeSelf;
            initialScale = obj.transform.localScale;
        }

        public override void SaveTempState() {
            Plugin.LogDebug($"TrackedCrumblingHold: [{obj.name}] Saving temporary state");
            temporaryEnabled = holdComponent.meshesHolder.activeSelf;
            temporaryScale = obj.transform.localScale;
        }

        public override void SaveConfigState() {
            Plugin.LogDebug($"TrackedCrumblingHold: [{obj.name}] Saving config state");

            if (SceneState.crumblingHoldsFile == null) {
                Plugin.LogDebug("TrackedCrumblingHold: Creating crumbling holds file");
                SceneState.crumblingHoldsFile = new ConfigFile(
                    Paths.crumblingHoldsPath, false
                );
            }

            BindConfig();

            configEnabled.Value = holdComponent.meshesHolder.activeSelf;
            configScale.Value = Cfg.Vec3ToString(obj.transform.localScale);
        }

        protected override void BindConfig() {
            if (boundConfig == true) {
                return;
            }

            if (SceneState.crumblingHoldsFile == null) {
                Plugin.LogDebug("TrackedCrumblingHold: Crumbling holds file doesn't exist, not binding config");
                return;
            }

            string scaleString = Cfg.Vec3ToString(obj.transform.localScale);

            Plugin.LogDebug($"TrackedCrumblingHold: Binding {obj.name} enabled as {holdComponent.meshesHolder.activeSelf}");
            Plugin.LogDebug($"TrackedCrumblingHold: Binding {obj.name} scale as {scaleString}");

            configEnabled = SceneState.crumblingHoldsFile.Bind(
                id, "enabled", holdComponent.meshesHolder.activeSelf
            );
            configScale = SceneState.crumblingHoldsFile.Bind(
                id, "scale", scaleString
            );

            boundConfig = true;
        }

        public TrackedCrumblingHold(GameObject obj) : base(obj) {}
    }
}
