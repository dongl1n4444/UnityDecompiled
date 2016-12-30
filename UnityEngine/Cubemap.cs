namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Class for handling cube maps, Use this to create or modify existing.</para>
    /// </summary>
    public sealed class Cubemap : Texture
    {
        /// <summary>
        /// <para>Create a new empty cubemap texture.</para>
        /// </summary>
        /// <param name="size">Width/height of a cube face in pixels.</param>
        /// <param name="format">Pixel data format to be used for the Cubemap.</param>
        /// <param name="mipmap">Should mipmaps be created?</param>
        public Cubemap(int size, TextureFormat format, bool mipmap)
        {
            Internal_Create(this, size, format, mipmap);
        }

        [ExcludeFromDocs]
        public void Apply()
        {
            bool makeNoLongerReadable = false;
            bool updateMipmaps = true;
            this.Apply(updateMipmaps, makeNoLongerReadable);
        }

        [ExcludeFromDocs]
        public void Apply(bool updateMipmaps)
        {
            bool makeNoLongerReadable = false;
            this.Apply(updateMipmaps, makeNoLongerReadable);
        }

        /// <summary>
        /// <para>Actually apply all previous SetPixel and SetPixels changes.</para>
        /// </summary>
        /// <param name="updateMipmaps">When set to true, mipmap levels are recalculated.</param>
        /// <param name="makeNoLongerReadable">When set to true, system memory copy of a texture is released.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void Apply([DefaultValue("true")] bool updateMipmaps, [DefaultValue("false")] bool makeNoLongerReadable);
        /// <summary>
        /// <para>Returns pixel color at coordinates (face, x, y).</para>
        /// </summary>
        /// <param name="face"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Color GetPixel(CubemapFace face, int x, int y)
        {
            Color color;
            INTERNAL_CALL_GetPixel(this, face, x, y, out color);
            return color;
        }

        [ExcludeFromDocs]
        public Color[] GetPixels(CubemapFace face)
        {
            int miplevel = 0;
            return this.GetPixels(face, miplevel);
        }

        /// <summary>
        /// <para>Returns pixel colors of a cubemap face.</para>
        /// </summary>
        /// <param name="face">The face from which pixel data is taken.</param>
        /// <param name="miplevel">Mipmap level for the chosen face.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern Color[] GetPixels(CubemapFace face, [DefaultValue("0")] int miplevel);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetPixel(Cubemap self, CubemapFace face, int x, int y, out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetPixel(Cubemap self, CubemapFace face, int x, int y, ref Color color);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_Create([Writable] Cubemap mono, int size, TextureFormat format, bool mipmap);
        /// <summary>
        /// <para>Sets pixel color at coordinates (face, x, y).</para>
        /// </summary>
        /// <param name="face"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        public void SetPixel(CubemapFace face, int x, int y, Color color)
        {
            INTERNAL_CALL_SetPixel(this, face, x, y, ref color);
        }

        [ExcludeFromDocs]
        public void SetPixels(Color[] colors, CubemapFace face)
        {
            int miplevel = 0;
            this.SetPixels(colors, face, miplevel);
        }

        /// <summary>
        /// <para>Sets pixel colors of a cubemap face.</para>
        /// </summary>
        /// <param name="colors">Pixel data for the Cubemap face.</param>
        /// <param name="face">The face to which the new data should be applied.</param>
        /// <param name="miplevel">The mipmap level for the face.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void SetPixels(Color[] colors, CubemapFace face, [DefaultValue("0")] int miplevel);
        [ExcludeFromDocs]
        public void SmoothEdges()
        {
            int smoothRegionWidthInPixels = 1;
            this.SmoothEdges(smoothRegionWidthInPixels);
        }

        /// <summary>
        /// <para>Performs smoothing of near edge regions.</para>
        /// </summary>
        /// <param name="smoothRegionWidthInPixels">Pixel distance at edges over which to apply smoothing.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void SmoothEdges([DefaultValue("1")] int smoothRegionWidthInPixels);

        /// <summary>
        /// <para>The format of the pixel data in the texture (Read Only).</para>
        /// </summary>
        public TextureFormat format { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>How many mipmap levels are in this texture (Read Only).</para>
        /// </summary>
        public int mipmapCount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
    }
}

