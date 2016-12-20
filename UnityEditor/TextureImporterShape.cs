namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>Select the kind of shape of your texture.</para>
    /// </summary>
    [Flags]
    public enum TextureImporterShape
    {
        /// <summary>
        /// <para>Texture is 2D.</para>
        /// </summary>
        Texture2D = 1,
        /// <summary>
        /// <para>Texture is a Cubemap.</para>
        /// </summary>
        TextureCube = 2
    }
}

