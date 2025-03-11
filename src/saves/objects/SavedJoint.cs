using PeterO.Cbor;
using UnityEngine;

namespace FastReset.Saves {
    public class SavedJoint : BaseSaved {
        // The saved state
        public Quaternion rotation;

        public override CBORObject ToCBOR () {
            return CBORObject.NewMap()
                .Add("id", id)
                .Add("rot", SaveManager.QuatToBytes(rotation));
        }

        public override void FromCBOR(CBORObject cbor) {
            id = cbor["id"].AsString();
            rotation = SaveManager.BytesToQuat(cbor["rot"].EncodeToBytes());
        }

        public SavedJoint(string id) : base(id) {}
    }
}
