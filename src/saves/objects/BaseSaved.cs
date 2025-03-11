using PeterO.Cbor;

namespace FastReset.Saves {
    public abstract class BaseSaved : Loggable {
        public byte[] byteId = null;
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

        protected BaseSaved() {}

        public BaseSaved(byte[] byteId) {
            this.byteId = byteId;
            this.id = SaveManager.BytesToString(byteId);
        }

        public BaseSaved(CBORObject cbor) {}
    }
}
