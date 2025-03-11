using PeterO.Cbor;
using UnityEngine;

namespace FastReset.Saves {
    public class SavedCrumblingHold : BaseSaved {
        // The saved state
        public bool enabled = false;
        public Vector3 scale = Vector3.zero;

        public override CBORObject ToCBOR() {
            LogDebug($"Saving crumbling hold {id}, enabled={enabled}, scale={scale}");

            return CBORObject.NewArray()
                .Add(byteId)
                .Add(enabled)
                .Add(SaveManager.Vec3ToBytes(scale));
        }

        public SavedCrumblingHold(byte[] byteId) : base(byteId) {}

        public SavedCrumblingHold(CBORObject cbor) : base(cbor[0].GetByteString()) {
            enabled = cbor[1].AsBoolean();
            scale = SaveManager.BytesToVec3(cbor[2].GetByteString());

            LogDebug($"Loaded crumbling hold {id}, enabled={enabled}, scale={scale}");
        }
    }
}
