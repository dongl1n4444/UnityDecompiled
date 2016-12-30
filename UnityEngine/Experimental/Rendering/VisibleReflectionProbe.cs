namespace UnityEngine.Experimental.Rendering
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Holds data of a visible reflection probe.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct VisibleReflectionProbe
    {
        /// <summary>
        /// <para>Probe bounding box.</para>
        /// </summary>
        public Bounds bounds;
        /// <summary>
        /// <para>Probe transformation matrix.</para>
        /// </summary>
        public Matrix4x4 localToWorld;
        /// <summary>
        /// <para>Shader data for probe HDR texture decoding.</para>
        /// </summary>
        public Vector4 hdr;
        /// <summary>
        /// <para>Probe projection center.</para>
        /// </summary>
        public Vector3 center;
        /// <summary>
        /// <para>Probe blending distance.</para>
        /// </summary>
        public float blendDistance;
        /// <summary>
        /// <para>Probe importance.</para>
        /// </summary>
        public int importance;
        /// <summary>
        /// <para>Should probe use box projection.</para>
        /// </summary>
        public int boxProjection;
        private int instanceId;
        private int textureId;
        /// <summary>
        /// <para>Probe texture.</para>
        /// </summary>
        public Texture texture =>
            GetTextureObject(this.textureId);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern Texture GetTextureObject(int textureId);
        /// <summary>
        /// <para>Accessor to ReflectionProbe component.</para>
        /// </summary>
        public ReflectionProbe probe =>
            GetReflectionProbeObject(this.instanceId);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern ReflectionProbe GetReflectionProbeObject(int instanceId);
    }
}

