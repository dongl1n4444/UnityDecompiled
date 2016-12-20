namespace UnityEngine
{
    using System;

    /// <summary>
    /// <para>Depth texture generation mode for Camera.</para>
    /// </summary>
    [Flags]
    public enum DepthTextureMode
    {
        /// <summary>
        /// <para>Generate a depth texture.</para>
        /// </summary>
        Depth = 1,
        /// <summary>
        /// <para>Generate a depth + normals texture.</para>
        /// </summary>
        DepthNormals = 2,
        /// <summary>
        /// <para>Specifies whether motion vectors should be rendered (if possible).</para>
        /// </summary>
        MotionVectors = 4,
        /// <summary>
        /// <para>Do not generate depth texture (Default).</para>
        /// </summary>
        None = 0
    }
}

