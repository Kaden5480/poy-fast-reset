using System;
using System.IO;
using System.Reflection;

using UnityEngine;
using UnityEngine.UI;

namespace FastReset.UI {
    public static class ImageLoader {
        /**
         * <summary>
         * Loads a file with the specified filename
         * into a byte array.
         *
         * The files are loaded from res/, so passing
         * a name of "image.png" will load res/image.png.
         * </summary>
         * <param name="name">The name of the file to load</param>
         * <returns>The file's bytes</returns>
         */
        public static byte[] LoadBytes(string name) {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string assemblyName = assembly.GetName().Name;

            using (Stream stream = assembly.GetManifestResourceStream(
                $"{assemblyName}.res.{name}"
            )) {
            using (MemoryStream mem = new MemoryStream()) {
                stream.CopyTo(mem);
                return mem.ToArray();
            }}
        }

        /**
         * <summary>
         * Loads an image with the specified filename
         * into a Texture2D.
         * </summary>
         * <returns>The Texture2D of the image</returns>
         */
        public static Texture2D LoadTexture(string name) {
            Texture2D texture = new Texture2D(1, 1);

            byte[] bytes = LoadBytes(name);
            ImageConversion.LoadImage(texture, bytes);

            return texture;
        }
    }
}
