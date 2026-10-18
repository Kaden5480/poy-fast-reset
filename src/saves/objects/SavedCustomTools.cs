using PeterO.Cbor;
using UnityEngine;

namespace FastReset.Saves {
    public class SavedCustomTools : BaseSaved {
        internal bool crampons = false;
        internal bool cramponsUpgrade = false;
        internal bool iceAxes = false;
        internal int pickups = 0;

        /**
         * <summary>
         * serializes this object to cbor.
         * </summary>
         */
        public override CBORObject ToCBOR() {
            CBORObject cbor = CBORObject.NewArray()
                .Add(crampons)
                .Add(cramponsUpgrade)
                .Add(iceAxes)
                .Add(pickups);

            LogDebug($"Serialized custom tools: {cbor.ToJSONString()}");
            return cbor;
        }

        /**
         * <summary>
         * Deserializes state from CBOR, updating self.
         * </summary>
         */
        public override void FromCBOR(CBORObject cbor) {
            crampons = cbor[0].AsBoolean();
            cramponsUpgrade = cbor[1].AsBoolean();
            iceAxes = cbor[2].AsBoolean();
            pickups = cbor[3].AsInt32();

            LogDebug($"Deserialized custom tools: crampons={crampons},"
                + $" cramponsUpgrade={cramponsUpgrade}, iceAxes={iceAxes},"
                + $" pickups={pickups}"
            );
        }

        /**
         * <summary>
         * Constructs an empty instance of SavedCustomTools.
         * </summary>
         */
        public SavedCustomTools() {}
    }
}
