using System;
using System.Collections.Generic;

using PeterO.Cbor;
using UnityEngine;

namespace FastReset.Saves {
    public class SavedBrittleIce : BaseSaved {
        // The saved state
        public int hp = 0;
        public List<float> matStates = new List<float>();
        public List<Vector3> positions = new List<Vector3>();
        public List<Quaternion> rotations = new List<Quaternion>();

        /**
         * <summary>
         * serializes this object to cbor.
         * </summary>
         */
        public override CBORObject ToCBOR() {
            // Material states
            CBORObject mats = CBORObject.NewArray();
            foreach (float state in matStates) {
                mats.Add(state);
            }

            // Positions
            CBORObject pos = CBORObject.NewArray();
            foreach (Vector3 position in positions) {
                pos.Add(SaveManager.Vec3ToBytes(position));
            }

            // Rotations
            CBORObject rot = CBORObject.NewArray();
            foreach (Quaternion rotation in rotations) {
                rot.Add(SaveManager.QuatToBytes(rotation));
            }

            CBORObject cbor = CBORObject.NewArray()
                .Add(byteId)
                .Add(hp)
                .Add(mats)
                .Add(pos)
                .Add(rot);

            LogDebug($"Serialized brittle ice {id}: {cbor.ToJSONString()}");
            return cbor;
        }

        /**
         * <summary>
         * Deserializes state from CBOR, updating self.
         * </summary>
         */
        public override void FromCBOR(CBORObject cbor) {
            UpdateID(cbor[0].GetByteString());

            // HP
            hp = cbor[1].AsInt32Value();

            // Material states
            CBORObject mats = cbor[2];
            for (int i = 0; i < mats.Count; i++) {
                matStates.Add(mats[i].AsSingle());
            }

            // Positions
            CBORObject pos = cbor[3];
            for (int i = 0; i < pos.Count; i++) {
                positions.Add(SaveManager.BytesToVec3(pos[i].GetByteString()));
            }

            // Rotations
            CBORObject rot = cbor[4];
            for (int i = 0; i < rot.Count; i++) {
                rotations.Add(SaveManager.BytesToQuat(rot[i].GetByteString()));
            }

            LogDebug(
                $"Deserialized brittle ice {id}: hp={hp},"
                + $" mats={matStates}, positions={positions},"
                + $" rotations={rotations}"
            );
        }

        /**
         * <summary>
         * Constructs an empty instance of SavedBrittleIce.
         *
         * IMPORTANT: This should only be used when deserializing.
         * </summary>
         */
        public SavedBrittleIce() {}

        /**
         * <summary>
         * Constructs an instance of SavedBrittleIce
         * with a provided ID in bytes.
         *
         * IMPORTANT: This only sets the ID, nothing else
         * </summary>
         * <param name="byteId">The ID of this object in bytes</param>
         */
        public SavedBrittleIce(byte[] byteId) : base(byteId) {}
    }
}
