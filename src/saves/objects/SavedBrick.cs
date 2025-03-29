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
                .Add(SaveManager.Vec3ToBytes(position))
                .Add(SaveManager.QuatToBytes(rotation))
                .Add(duration);

            LogDebug(
                $"Serialized brick {id}: enabled={enabled}, "
                + $"kinematic={kinematic}, position={position}, "
                + $"rotation={rotation}, duration={duration}"
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
            position = SaveManager.BytesToVec3(cbor[3].GetByteString());
            rotation = SaveManager.BytesToQuat(cbor[4].GetByteString());
            duration = cbor[5].AsSingle();

            LogDebug(
                $"Serialized brick {id}: enabled={enabled}, "
                + $"kinematic={kinematic}, position={position}, "
                + $"rotation={rotation}, duration={duration}"
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
