namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>Select this to set basic parameters depending on the purpose of your texture.</para>
    /// </summary>
    public enum TextureImporterType
    {
        [Obsolete("Use Default instead. All texture types now have an Advanced foldout (UnityUpgradable) -> Default")]
        Advanced = 5,
        [Obsolete("Use NormalMap (UnityUpgradable) -> NormalMap")]
        Bump = 1,
        /// <summary>
        /// <para>This sets up your texture with the basic parameters used for the Cookies of your lights.</para>
        /// </summary>
        Cookie = 4,
        [Obsolete("Use importer.textureShape = TextureImporterShape.TextureCube")]
        Cubemap = 3,
        /// <summary>
        /// <para>Use this if your texture is going to be used as a cursor.</para>
        /// </summary>
        Cursor = 7,
        /// <summary>
        /// <para>This is the most common setting used for all the textures in general.</para>
        /// </summary>
        Default = 0,
        /// <summary>
        /// <para>Use this if your texture is going to be used on any HUD/GUI Controls.</para>
        /// </summary>
        GUI = 2,
        [Obsolete("HDRI is not supported anymore")]
        HDRI = 9,
        /// <summary>
        /// <para>This is the most common setting used for all the textures in general.</para>
        /// </summary>
        [Obsolete("Use Default (UnityUpgradable) -> Default")]
        Image = 0,
        /// <summary>
        /// <para>This sets up your texture with the parameters used by the lightmap.</para>
        /// </summary>
        Lightmap = 6,
        /// <summary>
        /// <para>Select this to turn the color channels into a format suitable for real-time normal mapping.</para>
        /// </summary>
        NormalMap = 1,
        [Obsolete("Use a texture setup as a cubemap with glossy reflection instead")]
        Reflection = 3,
        /// <summary>
        /// <para>Use this for texture containing a single channel.</para>
        /// </summary>
        SingleChannel = 10,
        /// <summary>
        /// <para>Select this if you will be using your texture for Sprite graphics.</para>
        /// </summary>
        Sprite = 8
    }
}

