namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Class for ProceduralTexture handling.</para>
    /// </summary>
    public sealed class ProceduralTexture : Texture
    {
        /// <summary>
        /// <para>Grab pixel values from a ProceduralTexture.
        /// </para>
        /// </summary>
        /// <param name="x">X-coord of the top-left corner of the rectangle to grab.</param>
        /// <param name="y">Y-coord of the top-left corner of the rectangle to grab.</param>
        /// <param name="blockWidth">Width of rectangle to grab.</param>
        /// <param name="blockHeight">Height of the rectangle to grab.
        /// Get the pixel values from a rectangular area of a ProceduralTexture into an array.
        /// The block is specified by its x,y offset in the texture and by its width and height. The block is "flattened" into the array by scanning the pixel values across rows one by one.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern Color32[] GetPixels32(int x, int y, int blockWidth, int blockHeight);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern ProceduralMaterial GetProceduralMaterial();
        /// <summary>
        /// <para>The output type of this ProceduralTexture.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern ProceduralOutputType GetProceduralOutputType();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern bool HasBeenGenerated();

        /// <summary>
        /// <para>The format of the pixel data in the texture (Read Only).</para>
        /// </summary>
        public TextureFormat format { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Check whether the ProceduralMaterial that generates this ProceduralTexture is set to an output format with an alpha channel.</para>
        /// </summary>
        public bool hasAlpha { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
    }
}

