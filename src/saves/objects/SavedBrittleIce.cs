using PeterO.Cbor;
using UnityEngine;

namespace FastReset.Saves {
    public class SavedBrittleIce : BaseSaved {
        // The saved state
        public int hp = 0;
        public List<float> matStates = new List<float>();
        public List<Vector3> positions = new List<Vector3>();
        public List<Quaternion> rotations = new List<Quaternion>();

        public override CBORObject ToCBOR() {
            LogDebug($"Saving brittle ice {id}, hp={hp}, matStates={matStates}, "
                + $"positions={positions}, rotations={rotations}"
            );

            CBORObject mats = CBORObject.NewArray();
            foreach (float state in matStates) {
                mats.Add(state);
            }

            CBORObject pos = CBORObject.NewArray();
            foreach (Vector3 position in positions) {
                pos.Add(SaveManager.Vec3ToBytes(position));
            }

            CBORObject rot = CBORObject.NewArray();
            foreach (Quaternion rotation in rotations) {
                rot.Add(SaveManager.QuatToBytes(rotation));
            }


            return CBORObject.NewArray()
                .Add(byteId)
                .Add(mats)
                .Add(pos)
                .Add(rot);
        }

        public SavedBrittleIce(byte[] byteId) : base(byteId) {}

        public SavedBrittleIce(CBORObject cbor) : base(cbor[0].GetByteString()) {
            CBORObject mats = cbor[1];
            for (int i = 0; i < mats.Count; i++) {
                matStates.Add(mats[i].AsSingle());
            }

            CBORObject pos = cbor[2];
            for (int i = 0; i < pos.Count; i++) {
                positions.Add(SaveManager.BytesToVec3(pos[i].GetByteString()));
            }

            CBORObject rot = cbor[3];
            for (int i = 0; i < rot.Count; i++) {
                rotations.Add(SaveManager.BytesToQuat(rot[i].GetByteString()));
            }

            LogDebug($"Loaded brittle ice {id}, hp={hp}, matStates={matStates}, "
                + $"positions={positions}, rotations={rotations}"
            );
        }
    }
}
