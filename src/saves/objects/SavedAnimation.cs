using System.Collections.Generic;

using PeterO.Cbor;

namespace FastReset.Saves {
    public class SavedAnimation : BaseSaved {
        // The saved state
        public List<float> times = new List<float>();

        public override CBORObject ToCBOR() {
            LogDebug($"Saving animation {id}, times: {times}");

            CBORObject cborTimes = CBORObject.NewArray();
            foreach (float time in times) {
                cborTimes.Add(time);
            }

            return CBORObject.NewArray()
                .Add(byteId)
                .Add(cborTimes);
        }

        public SavedAnimation(byte[] byteId) : base(byteId) {}

        public SavedAnimation(CBORObject cbor) : base(cbor[0].GetByteString()) {
            CBORObject cborTimes = cbor[1];
            for (int i = 0; i < cborTimes.Count; i++) {
                times.Add(cborTimes[i].AsSingle());
            }

            LogDebug($"Loaded animation {id}, times: {times}");
        }
    }
}
