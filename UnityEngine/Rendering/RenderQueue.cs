namespace UnityEngine.Rendering
{
    using System;

    /// <summary>
    /// <para>Determine in which order objects are renderered.</para>
    /// </summary>
    public enum RenderQueue
    {
        /// <summary>
        /// <para>Alpha tested geometry uses this queue.</para>
        /// </summary>
        AlphaTest = 0x992,
        /// <summary>
        /// <para>This render queue is rendered before any others.</para>
        /// </summary>
        Background = 0x3e8,
        /// <summary>
        /// <para>Opaque geometry uses this queue.</para>
        /// </summary>
        Geometry = 0x7d0,
        /// <summary>
        /// <para>Last render queue that is considered "opaque".</para>
        /// </summary>
        GeometryLast = 0x9c4,
        /// <summary>
        /// <para>This render queue is meant for overlay effects.</para>
        /// </summary>
        Overlay = 0xfa0,
        /// <summary>
        /// <para>This render queue is rendered after Geometry and AlphaTest, in back-to-front order.</para>
        /// </summary>
        Transparent = 0xbb8
    }
}

