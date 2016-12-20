namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// <para>Editor data used in producing a Sprite.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SpriteMetaData
    {
        /// <summary>
        /// <para>Name of the Sprite.</para>
        /// </summary>
        public string name;
        /// <summary>
        /// <para>Bounding rectangle of the sprite's graphic within the atlas image.</para>
        /// </summary>
        public Rect rect;
        /// <summary>
        /// <para>Edge-relative alignment of the sprite graphic.</para>
        /// </summary>
        public int alignment;
        /// <summary>
        /// <para>The pivot point of the Sprite, relative to its bounding rectangle.</para>
        /// </summary>
        public Vector2 pivot;
        /// <summary>
        /// <para>Edge border size for a sprite (in pixels).</para>
        /// </summary>
        public Vector4 border;
    }
}

