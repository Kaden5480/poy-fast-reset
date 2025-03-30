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
                .Add(Ext.Vec3ToBytes(position))
                .Add(Ext.QuatToBytes(rotationX))
                .Add(rotationY);
        }

        public SavedPlayer() {}

        public SavedPlayer(CBORObject cbor) {
            position = cbor[0].AsVector3();
            rotationX = cbor[1].AsQuaternion();
            rotationY = cbor[2].AsSingle();

            LogDebug($"Loaded player: {position} | {rotationX} | {rotationY}");
        }
    }
}
