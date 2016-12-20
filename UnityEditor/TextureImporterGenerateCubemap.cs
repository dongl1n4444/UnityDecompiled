namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>Cubemap generation mode for TextureImporter.</para>
    /// </summary>
    public enum TextureImporterGenerateCubemap
    {
        /// <summary>
        /// <para>Automatically determine type of cubemap generation from the source image.</para>
        /// </summary>
        AutoCubemap = 6,
        /// <summary>
        /// <para>Generate cubemap from cylindrical texture.</para>
        /// </summary>
        Cylindrical = 2,
        /// <summary>
        /// <para>Generate cubemap from vertical or horizontal cross texture.</para>
        /// </summary>
        FullCubemap = 5,
        [Obsolete("Obscure shperemap modes are not supported any longer (use TextureImporterGenerateCubemap.Spheremap instead).")]
        NiceSpheremap = 4,
        /// <summary>
        /// <para>Do not generate cubemap (default).</para>
        /// </summary>
        [Obsolete("This value is deprecated (use TextureImporter.textureShape instead).")]
        None = 0,
        [Obsolete("Obscure shperemap modes are not supported any longer (use TextureImporterGenerateCubemap.Spheremap instead).")]
        SimpleSpheremap = 3,
        /// <summary>
        /// <para>Generate cubemap from spheremap texture.</para>
        /// </summary>
        Spheremap = 1
    }
}

