namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Class for handling 3D Textures, Use this to create.</para>
    /// </summary>
    public sealed class Texture3D : Texture
    {
        /// <summary>
        /// <para>Create a new empty 3D Texture.</para>
        /// </summary>
        /// <param name="width">Width of texture in pixels.</param>
        /// <param name="height">Height of texture in pixels.</param>
        /// <param name="depth">Depth of texture in pixels.</param>
        /// <param name="format">Texture data format.</param>
        /// <param name="mipmap">Should the texture have mipmaps?</param>
        public Texture3D(int width, int height, int depth, TextureFormat format, bool mipmap)
        {
            Internal_Create(this, width, height, depth, format, mipmap);
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
        /// <para>Actually apply all previous SetPixels changes.</para>
        /// </summary>
        /// <param name="updateMipmaps">When set to true, mipmap levels are recalculated.</param>
        /// <param name="makeNoLongerReadable">When set to true, system memory copy of a texture is released.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void Apply([DefaultValue("true")] bool updateMipmaps, [DefaultValue("false")] bool makeNoLongerReadable);
        [ExcludeFromDocs]
        public Color[] GetPixels()
        {
            int miplevel = 0;
            return this.GetPixels(miplevel);
        }

        /// <summary>
        /// <para>Returns an array of pixel colors representing one mip level of the 3D texture.</para>
        /// </summary>
        /// <param name="miplevel"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern Color[] GetPixels([DefaultValue("0")] int miplevel);
        [ExcludeFromDocs]
        public Color32[] GetPixels32()
        {
            int miplevel = 0;
            return this.GetPixels32(miplevel);
        }

        /// <summary>
        /// <para>Returns an array of pixel colors representing one mip level of the 3D texture.</para>
        /// </summary>
        /// <param name="miplevel"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern Color32[] GetPixels32([DefaultValue("0")] int miplevel);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_Create([Writable] Texture3D mono, int width, int height, int depth, TextureFormat format, bool mipmap);
        [ExcludeFromDocs]
        public void SetPixels(Color[] colors)
        {
            int miplevel = 0;
            this.SetPixels(colors, miplevel);
        }

        /// <summary>
        /// <para>Sets pixel colors of a 3D texture.</para>
        /// </summary>
        /// <param name="colors">The colors to set the pixels to.</param>
        /// <param name="miplevel">The mipmap level to be affected by the new colors.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void SetPixels(Color[] colors, [DefaultValue("0")] int miplevel);
        [ExcludeFromDocs]
        public void SetPixels32(Color32[] colors)
        {
            int miplevel = 0;
            this.SetPixels32(colors, miplevel);
        }

        /// <summary>
        /// <para>Sets pixel colors of a 3D texture.</para>
        /// </summary>
        /// <param name="colors">The colors to set the pixels to.</param>
        /// <param name="miplevel">The mipmap level to be affected by the new colors.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void SetPixels32(Color32[] colors, [DefaultValue("0")] int miplevel);

        /// <summary>
        /// <para>The depth of the texture (Read Only).</para>
        /// </summary>
        public int depth { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The format of the pixel data in the texture (Read Only).</para>
        /// </summary>
        public TextureFormat format { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
    }
}

