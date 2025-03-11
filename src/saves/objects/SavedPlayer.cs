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

            return CBORObject.NewMap()
                .Add("pos", SaveManager.Vec3ToBytes(position))
                .Add("rotX", SaveManager.QuatToBytes(rotationX))
                .Add("rotY", rotationY);
        }

        public SavedPlayer() {}

        public SavedPlayer(CBORObject cbor) {
            position = SaveManager.BytesToVec3(cbor["pos"].GetByteString());
            rotationX = SaveManager.BytesToQuat(cbor["rotX"].GetByteString());
            rotationY = cbor["rotY"].AsSingle();
            LogDebug($"Loaded player: {position} | {rotationX} | {rotationY}");
        }
    }
}
