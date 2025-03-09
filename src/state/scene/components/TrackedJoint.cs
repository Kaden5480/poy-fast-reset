using BepInEx.Configuration;
using UnityEngine;

using Cfg = FastReset.Config.Cfg;
using Paths = FastReset.Config.Paths;

namespace FastReset.State {
    public class TrackedJoint : TrackedObject {
        private Rigidbody jointRb = null;

        private Quaternion initialRotation;
        private Quaternion temporaryRotation;
        private ConfigEntry<string> configRotation;

        private void ResetJoint(Quaternion rotation) {
            jointRb.angularVelocity = Vector3.zero;
            jointRb.velocity = Vector3.zero;

            obj.transform.rotation = rotation;
        }

        public override void RestoreInitialState() {
            Plugin.LogDebug($"TrackedJoint: [{obj.name}] Restoring initial state");
            ResetJoint(initialRotation);
        }

        public override void RestoreTempState() {
            Plugin.LogDebug($"TrackedJoint: [{obj.name}] Restoring temporary state");
            ResetJoint(temporaryRotation);
        }

        public override void RestoreConfigState() {
            Plugin.LogDebug($"TrackedJoint: [{obj.name}] Restoring config state");
            ResetJoint(Cfg.StringToQuat(configRotation.Value));
        }

        protected override void SaveInitialState() {
            Plugin.LogDebug($"TrackedJoint: [{obj.name}] Saving initial state");

            jointRb = obj.GetComponent<Rigidbody>();
            initialRotation = obj.transform.rotation;
        }

        public override void SaveTempState() {
            Plugin.LogDebug($"TrackedJoint: [{obj.name}] Saving temporary state");
            temporaryRotation = obj.transform.rotation;
        }

        public override void SaveConfigState() {
            Plugin.LogDebug($"TrackedJoint: [{obj.name}] Saving config state");

            if (SceneState.jointsFile == null) {
                Plugin.LogDebug("TrackedJoint: Creating joints file");
                SceneState.jointsFile = new ConfigFile(
                    Paths.jointsPath, false
                );
            }

            BindConfig();

            configRotation.Value = Cfg.QuatToString(obj.transform.rotation);
        }

        protected override void BindConfig() {
            if (boundConfig == true) {
                return;
            }

            if (SceneState.jointsFile == null) {
                Plugin.LogDebug("TrackedJoint: Joints file doesn't exist, not binding config");
                return;
            }

            Plugin.LogDebug("TrackedJoint: Binding config");

            string quatString = Cfg.QuatToString(initialRotation);
            Plugin.LogDebug($"TrackedJoint: Binding {obj.name} rotation as {quatString}");
            configRotation = SceneState.jointsFile.Bind(
                id, "rotation", quatString
            );

            boundConfig = true;
        }

        public TrackedJoint(GameObject obj) : base(obj) {}
    }
}
