using PeterO.Cbor;

namespace FastReset.Saves {
    public abstract class BaseSaved : Loggable {
        public string id = null;

        /**
         * <summary>
         * Serializes this object to CBOR.
         * </summary>
         */
        public virtual CBORObject ToCBOR() {
            LogError("Serialize not implemented");
            return null;
        }

        /**
         * <summary>
         * Deserializes data from CBOR to
         * store in this object.
         * </summary>
         */
        public virtual void FromCBOR(CBORObject cbor) {
            LogError("Deserializing not implemented");
        }

        public BaseSaved(string id) {
            this.id = id;
        }
    }
}
