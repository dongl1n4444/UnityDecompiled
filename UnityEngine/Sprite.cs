namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;

    /// <summary>
    /// <para>Represents a Sprite object for use in 2D gameplay.</para>
    /// </summary>
    public sealed class Sprite : UnityEngine.Object
    {
        [ExcludeFromDocs]
        public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot)
        {
            Vector4 zero = Vector4.zero;
            SpriteMeshType tight = SpriteMeshType.Tight;
            uint extrude = 0;
            float pixelsPerUnit = 100f;
            return INTERNAL_CALL_Create(texture, ref rect, ref pivot, pixelsPerUnit, extrude, tight, ref zero);
        }

        [ExcludeFromDocs]
        public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit)
        {
            Vector4 zero = Vector4.zero;
            SpriteMeshType tight = SpriteMeshType.Tight;
            uint extrude = 0;
            return INTERNAL_CALL_Create(texture, ref rect, ref pivot, pixelsPerUnit, extrude, tight, ref zero);
        }

        [ExcludeFromDocs]
        public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit, uint extrude)
        {
            Vector4 zero = Vector4.zero;
            SpriteMeshType tight = SpriteMeshType.Tight;
            return INTERNAL_CALL_Create(texture, ref rect, ref pivot, pixelsPerUnit, extrude, tight, ref zero);
        }

        [ExcludeFromDocs]
        public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit, uint extrude, SpriteMeshType meshType)
        {
            Vector4 zero = Vector4.zero;
            return INTERNAL_CALL_Create(texture, ref rect, ref pivot, pixelsPerUnit, extrude, meshType, ref zero);
        }

        /// <summary>
        /// <para>Create a new Sprite object.</para>
        /// </summary>
        /// <param name="texture">Texture from which to obtain the sprite graphic.</param>
        /// <param name="rect">Rectangular section of the texture to use for the sprite.</param>
        /// <param name="pivot">Sprite's pivot point relative to its graphic rectangle.</param>
        /// <param name="pixelsPerUnit">The number of pixels in the sprite that correspond to one unit in world space.</param>
        /// <param name="extrude">Amount by which the sprite mesh should be expanded outwards.</param>
        /// <param name="meshType">Controls the type of mesh generated for the sprite.</param>
        /// <param name="border">The border sizes of the sprite (X=left, Y=bottom, Z=right, W=top).</param>
        public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, [DefaultValue("100.0f")] float pixelsPerUnit, [DefaultValue("0")] uint extrude, [DefaultValue("SpriteMeshType.Tight")] SpriteMeshType meshType, [DefaultValue("Vector4.zero")] Vector4 border) => 
            INTERNAL_CALL_Create(texture, ref rect, ref pivot, pixelsPerUnit, extrude, meshType, ref border);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Sprite INTERNAL_CALL_Create(Texture2D texture, ref Rect rect, ref Vector2 pivot, float pixelsPerUnit, uint extrude, SpriteMeshType meshType, ref Vector4 border);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_border(out Vector4 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_bounds(out Bounds value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_rect(out Rect value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_textureRect(out Rect value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Internal_GetPivot(Sprite sprite, out Vector2 output);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Internal_GetTextureRectOffset(Sprite sprite, out Vector2 output);
        /// <summary>
        /// <para>Sets up new Sprite geometry.</para>
        /// </summary>
        /// <param name="vertices">Array of vertex positions in Sprite Rect space.</param>
        /// <param name="triangles">Array of sprite mesh triangle indices.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void OverrideGeometry(Vector2[] vertices, ushort[] triangles);

        /// <summary>
        /// <para>Returns the texture that contains the alpha channel from the source texture. Unity generates this texture under the hood for sprites that have alpha in the source, and need to be compressed using techniques like ETC1.
        /// 
        /// Returns NULL if there is no associated alpha texture for the source sprite. This is the case if the sprite has not been setup to use ETC1 compression.</para>
        /// </summary>
        public Texture2D associatedAlphaSplitTexture { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Returns the border sizes of the sprite.</para>
        /// </summary>
        public Vector4 border
        {
            get
            {
                Vector4 vector;
                this.INTERNAL_get_border(out vector);
                return vector;
            }
        }

        /// <summary>
        /// <para>Bounds of the Sprite, specified by its center and extents in world space units.</para>
        /// </summary>
        public Bounds bounds
        {
            get
            {
                Bounds bounds;
                this.INTERNAL_get_bounds(out bounds);
                return bounds;
            }
        }

        /// <summary>
        /// <para>Returns true if this Sprite is packed in an atlas.</para>
        /// </summary>
        public bool packed { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>If Sprite is packed (see Sprite.packed), returns its SpritePackingMode.</para>
        /// </summary>
        public SpritePackingMode packingMode { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>If Sprite is packed (see Sprite.packed), returns its SpritePackingRotation.</para>
        /// </summary>
        public SpritePackingRotation packingRotation { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Location of the Sprite's center point in the Rect on the original Texture, specified in pixels.</para>
        /// </summary>
        public Vector2 pivot
        {
            get
            {
                Vector2 vector;
                Internal_GetPivot(this, out vector);
                return vector;
            }
        }

        /// <summary>
        /// <para>The number of pixels in the sprite that correspond to one unit in world space. (Read Only)</para>
        /// </summary>
        public float pixelsPerUnit { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Location of the Sprite on the original Texture, specified in pixels.</para>
        /// </summary>
        public Rect rect
        {
            get
            {
                Rect rect;
                this.INTERNAL_get_rect(out rect);
                return rect;
            }
        }

        /// <summary>
        /// <para>Get the reference to the used texture. If packed this will point to the atlas, if not packed will point to the source sprite.</para>
        /// </summary>
        public Texture2D texture { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Get the rectangle this sprite uses on its texture. Raises an exception if this sprite is tightly packed in an atlas.</para>
        /// </summary>
        public Rect textureRect
        {
            get
            {
                Rect rect;
                this.INTERNAL_get_textureRect(out rect);
                return rect;
            }
        }

        /// <summary>
        /// <para>Gets the offset of the rectangle this sprite uses on its texture to the original sprite bounds. If sprite mesh type is FullRect, offset is zero.</para>
        /// </summary>
        public Vector2 textureRectOffset
        {
            get
            {
                Vector2 vector;
                Internal_GetTextureRectOffset(this, out vector);
                return vector;
            }
        }

        /// <summary>
        /// <para>Returns a copy of the array containing sprite mesh triangles.</para>
        /// </summary>
        public ushort[] triangles { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The base texture coordinates of the sprite mesh.</para>
        /// </summary>
        public Vector2[] uv { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Returns a copy of the array containing sprite mesh vertex positions.</para>
        /// </summary>
        public Vector2[] vertices { [MethodImpl(MethodImplOptions.InternalCall)] get; }
    }
}

