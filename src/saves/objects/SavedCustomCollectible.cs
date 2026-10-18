using PeterO.Cbor;
using UnityEngine;

namespace FastReset.Saves {
    public class SavedCustomCollectible : BaseSaved {
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

            LogDebug($"Serialized custom collectible {id}: {cbor.ToJSONString()}");
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

            LogDebug($"Deserialized custom collectible {id}: enabled={enabled}");
        }

        /**
         * <summary>
         * Constructs an empty instance of SavedCustomCollectible.
         * </summary>
         */
        public SavedCustomCollectible() {}

        /**
         * <summary>
         * Constructs an instance of SavedCustomCollectible
         * with a provided ID in bytes.
         *
         * IMPORTANT: This only sets the ID, nothing else
         * </summary>
         * <param name="byteId">The ID of this object in bytes</param>
         */
        public SavedCustomCollectible(byte[] byteId) : base(byteId) {}
    }
}
