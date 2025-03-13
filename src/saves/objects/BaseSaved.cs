using PeterO.Cbor;

namespace FastReset.Saves {
    public abstract class BaseSaved : Loggable {
        // The ID of this object in bytes
        public byte[] byteId = null;

        // The string ID of this object
        public string id = null;

        /**
         * <summary>
         * serializes this object to cbor.
         * </summary>
         */
        public virtual CBORObject ToCBOR() {
            LogError("Serializing not implemented");
            return null;
        }

        /**
         * <summary>
         * Deserializes this object from CBOR.
         * </summary>
         */
        public virtual void FromCBOR(CBORObject cbor) {
            LogError("Deserializing not implemented");
        }

        /**
         * <summary>
         * Updates this object's ID using the provided ID
         * in bytes.
         * </summary>
         * <param name="byteId">The ID of this object in bytes</param>
         */
        protected virtual void UpdateID(byte[] byteId) {
            this.byteId = byteId;
            this.id = SaveManager.BytesToString(byteId);
        }


        /**
         * <summary>
         * Constructs an empty instance of BaseSaved.
         *
         * IMPORTANT: This should only be used when deserializing.
         * </summary>
         */
        public BaseSaved() {}

        /**
         * <summary>
         * Constructs an instance of BaseSaved
         * with a provided ID in bytes.
         *
         * IMPORTANT: This only sets the ID, nothing else
         * </summary>
         * <param name="byteId">The ID of this object in bytes</param>
         */
        public BaseSaved(byte[] byteId) {
            UpdateID(byteId);
        }
    }
}
