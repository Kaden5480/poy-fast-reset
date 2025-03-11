using PeterO.Cbor;
using UnityEngine;

namespace FastReset.Saves {
    public class SavedJoint : BaseSaved {
        // The saved state
        public Quaternion rotation = Quaternion.identity;

        public override CBORObject ToCBOR () {
            LogDebug($"Saving joint: {id} | {rotation}");

            return CBORObject.NewArray()
                .Add(byteId)
                .Add(SaveManager.QuatToBytes(rotation));
        }

        public SavedJoint() {}

        public SavedJoint(byte[] byteId) : base(byteId) {}

        public SavedJoint(CBORObject cbor) : base(cbor[0].GetByteString()) {
            rotation = SaveManager.BytesToQuat(cbor[1].GetByteString());

            LogDebug($"Loaded joint: {id} | {rotation}");
        }
    }
}
