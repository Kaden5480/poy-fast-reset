using PeterO.Cbor;
using UnityEngine;

namespace FastReset.Saves {
    public class SavedPlayer : Loggable {
        // The saved state
        public Vector3 position = Vector3.zero;
        public Quaternion rotationX = Quaternion.identity;
        public float rotationY = 0f;
        public bool grounded = false;

        public CBORObject ToCBOR () {
            LogDebug($"Saving player: {position} | {rotationX} | {rotationY} | {grounded}");

            return CBORObject.NewArray()
                .Add(Ext.Vec3ToBytes(position))
                .Add(Ext.QuatToBytes(rotationX))
                .Add(rotationY)
                .Add(grounded);
        }

        public SavedPlayer() {}

        public SavedPlayer(CBORObject cbor) {
            position = cbor[0].AsVector3();
            rotationX = cbor[1].AsQuaternion();
            rotationY = cbor[2].AsSingle();
            grounded = cbor[3].AsBoolean();

            LogDebug($"Loaded player: {position} | {rotationX} | {rotationY} | {grounded}");
        }
    }
}
