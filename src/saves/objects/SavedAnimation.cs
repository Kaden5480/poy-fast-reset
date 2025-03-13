using System.Collections.Generic;

using PeterO.Cbor;

namespace FastReset.Saves {
    public class SavedAnimation : BaseSaved {
        // The saved state
        public List<float> times = new List<float>();

        /**
         * <summary>
         * serializes this object to cbor.
         * </summary>
         */
        public override CBORObject ToCBOR() {
            CBORObject cborTimes = CBORObject.NewArray();
            foreach (float time in times) {
                cborTimes.Add(time);
            }

            CBORObject cbor = CBORObject.NewArray()
                .Add(byteId)
                .Add(cborTimes);

            LogDebug($"Serialized animation {id}: {cbor.ToJSONString()}");
            return cbor;
        }

        /**
         * <summary>
         * Deserializes state from CBOR, updating self.
         * </summary>
         */
        public override void FromCBOR(CBORObject cbor) {
            UpdateID(cbor[0].GetByteString());

            CBORObject cborTimes = cbor[1];
            for (int i = 0; i < cborTimes.Count; i++) {
                times.Add(cborTimes[i].AsSingle());
            }

            LogDebug($"Deserialized animation {id}: times={times}");
        }

        /**
         * <summary>
         * Constructs an empty instance of SavedAnimation.
         *
         * IMPORTANT: This should only be used when deserializing.
         * </summary>
         */
        public SavedAnimation() {}

        /**
         * <summary>
         * Constructs an instance of SavedAnimation
         * with a provided ID in bytes.
         *
         * IMPORTANT: This only sets the ID, nothing else
         * </summary>
         * <param name="byteId">The ID of this object in bytes</param>
         */
        public SavedAnimation(byte[] byteId) : base(byteId) {}
    }
}
