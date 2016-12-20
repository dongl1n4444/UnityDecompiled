namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>Texture importer lets you modify Texture2D import settings for DDS textures from editor scripts.</para>
    /// </summary>
    public sealed class DDSImporter : AssetImporter
    {
        /// <summary>
        /// <para>Is texture data readable from scripts.</para>
        /// </summary>
        public bool isReadable { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

