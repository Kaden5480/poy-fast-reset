using PeterO.Cbor;
using UnityEngine;

namespace FastReset.Saves {
    public class SavedPlayer : Loggable {
        // The saved state
        public Vector3 position = Vector3.zero;
        public Quaternion rotationX = Quaternion.identity;
        public float rotationY = 0f;

        public CBORObject ToCBOR () {
            LogDebug($"Saving player: {position} | {rotationX} | {rotationY}");

            return CBORObject.NewArray()
                .Add(SaveManager.Vec3ToBytes(position))
                .Add(SaveManager.QuatToBytes(rotationX))
                .Add(rotationY);
        }

        public SavedPlayer() {}

        public SavedPlayer(CBORObject cbor) {
            position = SaveManager.BytesToVec3(cbor[0].GetByteString());
            rotationX = SaveManager.BytesToQuat(cbor[1].GetByteString());
            rotationY = cbor[2].AsSingle();

            LogDebug($"Loaded player: {position} | {rotationX} | {rotationY}");
        }
    }
}
