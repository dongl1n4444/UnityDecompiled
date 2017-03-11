namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Use IHVImageFormatImporter to modify Texture2D import settings for Textures in IHV (Independent Hardware Vendor) formats such as .DDS and .PVR from Editor scripts.</para>
    /// </summary>
    public sealed class IHVImageFormatImporter : AssetImporter
    {
        /// <summary>
        /// <para>Filtering mode of the texture.</para>
        /// </summary>
        public UnityEngine.FilterMode filterMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Is texture data readable from scripts.</para>
        /// </summary>
        public bool isReadable { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Texture coordinate wrapping mode.</para>
        /// </summary>
        public TextureWrapMode wrapMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Texture U coordinate wrapping mode.</para>
        /// </summary>
        public TextureWrapMode wrapModeU { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Texture V coordinate wrapping mode.</para>
        /// </summary>
        public TextureWrapMode wrapModeV { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Texture W coordinate wrapping mode for Texture3D.</para>
        /// </summary>
        public TextureWrapMode wrapModeW { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

