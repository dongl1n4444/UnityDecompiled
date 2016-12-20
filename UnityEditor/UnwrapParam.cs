namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>Unwrapping settings.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct UnwrapParam
    {
        /// <summary>
        /// <para>Maximum allowed angle distortion (0..1).</para>
        /// </summary>
        public float angleError;
        /// <summary>
        /// <para>Maximum allowed area distortion (0..1).</para>
        /// </summary>
        public float areaError;
        /// <summary>
        /// <para>This angle (in degrees) or greater between triangles will cause seam to be created.</para>
        /// </summary>
        public float hardAngle;
        /// <summary>
        /// <para>How much uv-islands will be padded.</para>
        /// </summary>
        public float packMargin;
        internal int recollectVertices;
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void SetDefaults(out UnwrapParam param);
    }
}

