// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.GraphicsTools.Editor
{
    /// <summary>
    /// Defines types of textures which can be written to disk.
    /// </summary>
    public static class TextureFile
    {
        /// <summary>
        /// Enumeration of texture file formats.
        /// </summary>
        public enum Format
        {
            TGA = 0,
            PNG = 1,
            JPG = 2
        }

        /// <summary>
        /// Returns the extension string. For example ".jpg" 
        /// </summary>
        public static string GetExtension(Format type)
        {
            return extensions[(int)type];
        }

        /// <summary>
        /// Turns a Unity texture into a byte buffer based on the image format.
        /// </summary>
        public static byte[] Encode(Texture2D texture, Format extension)
        {
            if (texture == null)
            {
                return null;
            }

            switch (extension)
            {
                default:
                case TextureFile.Format.TGA:
                    {
                        return texture.EncodeToTGA();
                    }
                case TextureFile.Format.PNG:
                    {
                        return texture.EncodeToPNG();
                    }
                case TextureFile.Format.JPG:
                    {
                        return texture.EncodeToJPG();
                    }
            }
        }

        private static readonly string[] extensions = Array.ConvertAll(Enum.GetNames(typeof(Format)), s => '.' + s.ToLower());
    }
}
