using PeterO.Cbor;
using UnityEngine;

namespace FastReset.Saves {
    public class SavedJoint : BaseSaved {
        // The saved state
        public Vector3 position = Vector3.zero;
        public Quaternion rotation = Quaternion.identity;

        /**
         * <summary>
         * serializes this object to cbor.
         * </summary>
         */
        public override CBORObject ToCBOR () {
            CBORObject cbor = CBORObject.NewArray()
                .Add(byteId)
                .Add(Ext.Vec3ToBytes(position))
                .Add(Ext.QuatToBytes(rotation));

            LogDebug($"Serialized joint {id}: {cbor.ToJSONString()}");
            return cbor;
        }

        /**
         * <summary>
         * Deserializes state from CBOR, updating self.
         * </summary>
         */
        public override void FromCBOR(CBORObject cbor) {
            UpdateID(cbor[0].GetByteString());
            position = cbor[1].AsVector3();
            rotation = cbor[2].AsQuaternion();

            LogDebug($"Deserialized joint {id}: rotation={rotation}");
        }

        /**
         * <summary>
         * Constructs an empty instance of SavedJoint.
         *
         * IMPORTANT: This should only be used when deserializing.
         * </summary>
         */
        public SavedJoint() {}

        /**
         * <summary>
         * Constructs an instance of SavedJoint
         * with a provided ID in bytes.
         *
         * IMPORTANT: This only sets the ID, nothing else
         * </summary>
         * <param name="byteId">The ID of this object in bytes</param>
         */
        public SavedJoint(byte[] byteId) : base(byteId) {}
    }
}
