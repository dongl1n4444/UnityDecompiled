namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>This enum is used to build a bitmask for controlling per-channel vertex compression.</para>
    /// </summary>
    [Flags]
    public enum VertexChannelCompressionFlags
    {
        /// <summary>
        /// <para>Vertex color.</para>
        /// </summary>
        kColor = 4,
        /// <summary>
        /// <para>Vertex normal.</para>
        /// </summary>
        kNormal = 2,
        /// <summary>
        /// <para>Position.</para>
        /// </summary>
        kPosition = 1,
        /// <summary>
        /// <para>Tangent.</para>
        /// </summary>
        kTangent = 0x80,
        /// <summary>
        /// <para>Texture coordinate channel 0. Usually used for Albedo texture.</para>
        /// </summary>
        kUV0 = 8,
        /// <summary>
        /// <para>Texture coordinate channel 1. Usually used for baked lightmap.</para>
        /// </summary>
        kUV1 = 0x10,
        /// <summary>
        /// <para>Texture coordinate channel 2. Usually used for realtime GI.</para>
        /// </summary>
        kUV2 = 0x20,
        /// <summary>
        /// <para>Texture coordinate channel 3.</para>
        /// </summary>
        kUV3 = 0x40
    }
}

