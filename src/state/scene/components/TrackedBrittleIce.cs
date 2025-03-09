using System;
using System.Collections.Generic;

using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

using Cfg = FastReset.Config.Cfg;
using Paths = FastReset.Config.Paths;
using PositionFix = FastReset.Patches.PositionFix;

namespace FastReset.State {
    public class TrackedBrittleIce : TrackedObject {
        // Shorthand for accessing the cache
        private Cache cache {
            get => Plugin.instance.cache;
        }

        private BrittleIce brittleIce;

        private Renderer[] iceCracksRenderers {
            get => (Renderer[]) AccessTools
                .Field(typeof(BrittleIce), "iceCracksRenderers")
                .GetValue(brittleIce);
        }

        // HP of the ice
        private int initialHp;
        private int temporaryHp;
        private ConfigEntry<int> configHp;

        // Material states
        private string initialMaterial;
        private string temporaryMaterial;
        private ConfigEntry<string> configMaterial;

        // Rigidbody positions
        private List<Vector3> initialPositions = new List<Vector3>();
        private List<Vector3> temporaryPositions = new List<Vector3>();
        private List<ConfigEntry<string>> configPositions = new List<ConfigEntry<string>>();

        // Rigidbody rotations
        private List<Quaternion> initialRotations = new List<Quaternion>();
        private List<Quaternion> temporaryRotations = new List<Quaternion>();
        private List<ConfigEntry<string>> configRotations = new List<ConfigEntry<string>>();

        private string MaterialToString(Material material) {
            float state0 = material.GetFloat("_IceCracksStrength");
            float state1 = material.GetFloat("_IceStrength1");
            float state2 = material.GetFloat("_IceStrength2");
            float state3 = material.GetFloat("_IceCracksScale");
            return $"{state0};{state1};{state2};{state3}";
        }

        private float[] StringToMatStates(string stateString) {
            string[] stringStates = stateString.Split(';');
            return new float[] {
                Single.Parse(stringStates[0]),
                Single.Parse(stringStates[1]),
                Single.Parse(stringStates[2]),
                Single.Parse(stringStates[3]),
            };
        }

        private void UpdateMaterialStates(string stateString) {
            float[] states = StringToMatStates(stateString);

            foreach (Renderer renderer in iceCracksRenderers) {
                renderer.material.SetFloat("_IceCracksStrength", states[0]);
                renderer.material.SetFloat("_IceStrength1", states[1]);
                renderer.material.SetFloat("_IceStrength2", states[2]);
                renderer.material.SetFloat("_IceCracksScale", states[3]);
            }
        }

        private void Restore(int hp, string stateString) {
            brittleIce.iceHP = hp;
            UpdateMaterialStates(stateString);

            // Restore kinematic and active state if HP is high enough
            // Otherwise, hide it
            bool isActive = hp > 0;

            // Disable all coroutines
            brittleIce.StopAllCoroutines();

            foreach (Rigidbody rb in brittleIce.myRBs) {
                rb.isKinematic = isActive;
                rb.gameObject.SetActive(isActive);

                foreach (Collider collider in rb.gameObject.GetComponents<Collider>()) {
                    collider.enabled = isActive;
                }

                brittleIce.enabled = isActive;
            }
        }

        public override void RestoreInitialState() {
            Plugin.LogDebug($"TrackedBrittleIce: [{obj.name}] Restoring initial state");
            Restore(initialHp, initialMaterial);

            for (int i = 0; i < brittleIce.myRBs.Length; i++) {
                Transform transform = brittleIce.myRBs[i].transform;
                transform.position = PositionFix.OffsetToReal(initialPositions[i]);
                transform.rotation = initialRotations[i];
            }
        }

        public override void RestoreTempState() {
            Plugin.LogDebug($"TrackedBrittleIce: [{obj.name}] Restoring temporary state");
            Restore(temporaryHp, temporaryMaterial);

            for (int i = 0; i < brittleIce.myRBs.Length; i++) {
                Transform transform = brittleIce.myRBs[i].transform;
                transform.position = PositionFix.OffsetToReal(temporaryPositions[i]);
                transform.rotation = temporaryRotations[i];
            }
        }

        public override void RestoreConfigState() {
            Plugin.LogDebug($"TrackedBrittleIce: [{obj.name}] Restoring config state");
            Restore(configHp.Value, configMaterial.Value);

            for (int i = 0; i < brittleIce.myRBs.Length; i++) {
                Transform transform = brittleIce.myRBs[i].transform;
                transform.position = PositionFix.OffsetToReal(Cfg.StringToVec3(configPositions[i].Value));
                transform.rotation = Cfg.StringToQuat(configRotations[i].Value);
            }
        }

        protected override void SaveInitialState() {
            Plugin.LogDebug($"TrackedBrittleIce: [{obj.name}] Saving initial state");
            brittleIce = obj.GetComponent<BrittleIce>();

            initialHp = brittleIce.iceHP;

            if (iceCracksRenderers == null) {
                Plugin.LogDebug($"TrackedBrittleIce: [{obj.name}] renderers are null");
            }

            initialMaterial = MaterialToString(
                iceCracksRenderers[0].material
            );

            foreach (Rigidbody rb in brittleIce.myRBs) {
                initialPositions.Add(PositionFix.RealToOffset(rb.transform.position));
                initialRotations.Add(rb.transform.rotation);
            }
        }

        public override void SaveTempState() {
            Plugin.LogDebug($"TrackedBrittleIce: [{obj.name}] Saving temporary state");

            temporaryHp = brittleIce.iceHP;
            temporaryMaterial = MaterialToString(
                iceCracksRenderers[0].material
            );

            foreach (Rigidbody rb in brittleIce.myRBs) {
                temporaryPositions.Add(PositionFix.RealToOffset(rb.transform.position));
                temporaryRotations.Add(rb.transform.rotation);
            }
        }

        public override void SaveConfigState() {
            Plugin.LogDebug($"TrackedBrittleIce: [{obj.name}] Saving config state");

            if (SceneState.brittleIceFile == null) {
                Plugin.LogDebug("TrackedBrittleIce: Creating brittle ice file");
                SceneState.brittleIceFile = new ConfigFile(
                    Paths.brittleIcePath, false
                );
            }

            BindConfig();

            Plugin.LogDebug($"TrackedBrittleIce: [{obj.name}] saving hp as {brittleIce.iceHP}");
            configHp.Value = brittleIce.iceHP;

            string materialString = MaterialToString(
                iceCracksRenderers[0].material
            );
            Plugin.LogDebug($"TrackedBrittleIce: [{obj.name}] saving material as {materialString}");
            configMaterial.Value = materialString;

            for (int i = 0; i < brittleIce.myRBs.Length; i++) {
                Transform transform = brittleIce.myRBs[i].transform;

                string positionString = Cfg.Vec3ToString(PositionFix.RealToOffset(transform.position));
                string rotationString = Cfg.QuatToString(transform.rotation);

                Plugin.LogDebug($"TrackedBrittleIce: [{obj.name}] saving position_{i} as {positionString}");
                configPositions[i].Value = positionString;

                Plugin.LogDebug($"TrackedBrittleIce: [{obj.name}] saving rotation_{i} as {rotationString}");
                configRotations[i].Value = rotationString;
            }
        }

        protected override void BindConfig() {
            if (boundConfig == true) {
                return;
            }

            if (SceneState.brittleIceFile == null) {
                Plugin.LogDebug("TrackedBrittleIce: Brittle ice file doesn't exist, not binding config");
                return;
            }

            Plugin.LogDebug("TrackedBrittleIce: Binding config");

            Plugin.LogDebug($"TrackedBrittleIce: [{obj.name}] Binding hp as {initialHp}");
            configHp = SceneState.brittleIceFile.Bind(
                id, "hp", initialHp
            );

            Plugin.LogDebug($"TrackedBrittleIce: [{obj.name}] Binding material as {initialMaterial}");
            configMaterial = SceneState.brittleIceFile.Bind(
                id, "material", initialMaterial
            );

            for (int i = 0; i < initialPositions.Count; i++) {
                string positionString = Cfg.Vec3ToString(initialPositions[i]);
                string rotationString = Cfg.QuatToString(initialRotations[i]);

                Plugin.LogDebug($"TrackedBrittleIce: [{obj.name}] Binding position_{i} as {positionString}");
                configPositions.Add(SceneState.brittleIceFile.Bind(
                    id, $"position_{i}", positionString
                ));

                Plugin.LogDebug($"TrackedBrittleIce: [{obj.name}] Binding rotation_{i} as {positionString}");
                configRotations.Add(SceneState.brittleIceFile.Bind(
                    id, $"rotation_{i}", rotationString
                ));
            }

            boundConfig = true;
        }

        public TrackedBrittleIce(GameObject obj) : base(obj) {}
    }
}
