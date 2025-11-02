using PeterO.Cbor;
using UnityEngine;

namespace FastReset.Saves {
    public class SavedBrick : BaseSaved {
        // The saved state
        public bool enabled;
        public bool kinematic;
        public Vector3 position;
        public Quaternion rotation;
        public float duration;
        public string tag;

        /**
         * <summary>
         * serializes this object to cbor.
         * </summary>
         */
        public override CBORObject ToCBOR () {
            CBORObject cbor = CBORObject.NewArray()
                .Add(byteId)
                .Add(enabled)
                .Add(kinematic)
                .Add(Ext.Vec3ToBytes(position))
                .Add(Ext.QuatToBytes(rotation))
                .Add(duration)
                .Add(tag);

            LogDebug(
                $"Serialized brick {id}: enabled={enabled}, "
                + $"kinematic={kinematic}, position={position}, "
                + $"rotation={rotation}, duration={duration}, tag={tag}"
            );
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
            kinematic = cbor[2].AsBoolean();
            position = cbor[3].AsVector3();
            rotation = cbor[4].AsQuaternion();
            duration = cbor[5].AsSingle();
            tag = cbor[6].AsString();

            LogDebug(
                $"Serialized brick {id}: enabled={enabled}, "
                + $"kinematic={kinematic}, position={position}, "
                + $"rotation={rotation}, duration={duration}, tag={tag}"
            );
        }

        /**
         * <summary>
         * Constructs an empty instance of SavedBrick.
         *
         * IMPORTANT: This should only be used when deserializing.
         * </summary>
         */
        public SavedBrick() {}

        /**
         * <summary>
         * Constructs an instance of SavedBrick
         * with a provided ID in bytes.
         *
         * IMPORTANT: This only sets the ID, nothing else
         * </summary>
         * <param name="byteId">The ID of this object in bytes</param>
         */
        public SavedBrick(byte[] byteId) : base(byteId) {}
    }
}
