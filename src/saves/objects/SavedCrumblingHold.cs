using PeterO.Cbor;
using UnityEngine;

namespace FastReset.Saves {
    public class SavedCrumblingHold : BaseSaved {
        // The saved state
        public bool enabled = false;
        public Vector3 scale = Vector3.zero;

        /**
         * <summary>
         * serializes this object to cbor.
         * </summary>
         */
        public override CBORObject ToCBOR() {
            CBORObject cbor = CBORObject.NewArray()
                .Add(byteId)
                .Add(enabled)
                .Add(SaveManager.Vec3ToBytes(scale));

            LogDebug($"Serialized crumbling hold {id}: {cbor.ToJSONString()}");
            return cbor;
        }

        /**
         * <summary>
         * Deserializes state from CBOR, updating self.
         * </summary>
         */
        public override void FromCBOR(CBORObject cbor) {
            UpdateID(cbor[0].GetByteString());
            enabled = cbor[1].AsBoolean();
            scale = SaveManager.BytesToVec3(cbor[2].GetByteString());

            LogDebug($"Deserialized crumbling hold {id}: enabled={enabled}, scale={scale}");
        }

        /**
         * <summary>
         * Constructs an empty instance of SavedCrumblingHold.
         *
         * IMPORTANT: This should only be used when deserializing.
         * </summary>
         */
        public SavedCrumblingHold() {}

        /**
         * <summary>
         * Constructs an instance of SavedCrumblingHold
         * with a provided ID in bytes.
         *
         * IMPORTANT: This only sets the ID, nothing else
         * </summary>
         * <param name="byteId">The ID of this object in bytes</param>
         */
        public SavedCrumblingHold(byte[] byteId) : base(byteId) {}
    }
}
