using System.IO;

using PeterO.Cbor;
using UnityEngine;

namespace FastReset.Saves {
    public static class Ext {
        /**
         * <summary>
         * Converts a Vector3 to a byte array.
         * </summary>
         * <param name="vec">The Vector3 to convert</param>
         * <returns>The Vector3 as bytes</returns>
         */
        public static byte[] Vec3ToBytes(Vector3 vec) {
            using (MemoryStream stream = new MemoryStream()) {
            using (BinaryWriter writer = new BinaryWriter(stream)) {
                writer.Write(vec.x);
                writer.Write(vec.y);
                writer.Write(vec.z);

                return stream.ToArray();
            }}
        }

        /**
         * <summary>
         * Converts bytes into a Vector3.
         * </summary>
         * <param name="bytes">The bytes to convert</param>
         * <returns>The Vector3</returns>
         */
        public static Vector3 BytesToVec3(byte[] bytes) {
            float x, y, z;

            using (MemoryStream stream = new MemoryStream(bytes)) {
            using (BinaryReader reader = new BinaryReader(stream)) {
                x = reader.ReadSingle();
                y = reader.ReadSingle();
                z = reader.ReadSingle();

            }}

            return new Vector3(x, y, z);
        }

        /**
         * <summary>
         * Converts a Quaternion to a byte array.
         * </summary>
         * <param name="quat">The Quaternion to convert</param>
         * <returns>The Quaternion as bytes</returns>
         */
        public static byte[] QuatToBytes(Quaternion quat) {
            using (MemoryStream stream = new MemoryStream()) {
            using (BinaryWriter writer = new BinaryWriter(stream)) {
                writer.Write(quat.x);
                writer.Write(quat.y);
                writer.Write(quat.z);
                writer.Write(quat.w);

                return stream.ToArray();
            }}
        }

        /**
         * <summary>
         * Converts bytes into a quaternion.
         * </summary>
         * <param name="bytes">The bytes to convert</param>
         * <returns>The quaternion</returns>
         */
        public static Quaternion BytesToQuat(byte[] bytes) {
            float x, y, z, w;

            using (MemoryStream stream = new MemoryStream(bytes)) {
            using (BinaryReader reader = new BinaryReader(stream)) {
                x = reader.ReadSingle();
                y = reader.ReadSingle();
                z = reader.ReadSingle();
                w = reader.ReadSingle();
            }}

            return new Quaternion(x, y, z, w);
        }

        /**
         * <summary>
         * Reads a Vector3 from a CBORObject.
         * </summary>
         * <returns>The Vector3</returns>
         */
        public static Vector3 AsVector3(this CBORObject cbor) {
            return BytesToVec3(cbor.GetByteString());
        }

        /**
         * <summary>
         * Reads a Quaternion from a CBORObject.
         * </summary>
         * <returns>The Quaternion</returns>
         */
        public static Quaternion AsQuaternion(this CBORObject cbor) {
            return BytesToQuat(cbor.GetByteString());
        }
    }
}
