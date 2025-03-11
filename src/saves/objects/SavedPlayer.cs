using PeterO.Cbor;
using UnityEngine;

namespace FastReset.Saves {
    public class SavedPlayer {
        // The saved state
        public Vector3 position;
        public Quaternion rotationX;
        public float rotationY;

        public CBORObject ToCBOR () {
            return CBORObject.NewMap()
                .Add("pos", SaveManager.Vec3ToBytes(position))
                .Add("rotX", SaveManager.QuatToBytes(rotationX))
                .Add("rotY", rotationY);
        }

        public void FromCBOR(CBORObject cbor) {
            position = SaveManager.BytesToVec3(cbor["pos"].EncodeToBytes());
            rotationX = SaveManager.BytesToQuat(cbor["rotX"].EncodeToBytes());
            rotationY = cbor["rotY"].AsSingle();
        }
    }
}
