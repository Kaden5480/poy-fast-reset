using PeterO.Cbor;
using UnityEngine;

namespace FastReset.Saves {
    public class SavedCustomLoseToolZone : BaseSaved {
        internal bool enabled = false;

        /**
         * <summary>
         * serializes this object to cbor.
         * </summary>
         */
        public override CBORObject ToCBOR() {
            CBORObject cbor = CBORObject.NewArray()
                .Add(byteId)
                .Add(enabled);

            LogDebug($"Serialized custom lose tool zone {id}: {cbor.ToJSONString()}");
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

            LogDebug($"Deserialized custom lose tool zone {id}: enabled={enabled}");
        }

        /**
         * <summary>
         * Constructs an empty instance of SavedCustomLoseToolZone.
         * </summary>
         */
        public SavedCustomLoseToolZone() {}

        /**
         * <summary>
         * Constructs an instance of SavedCustomLoseToolZone
         * with a provided ID in bytes.
         *
         * IMPORTANT: This only sets the ID, nothing else
         * </summary>
         * <param name="byteId">The ID of this object in bytes</param>
         */
        public SavedCustomLoseToolZone(byte[] byteId) : base(byteId) {}
    }
}
