namespace UnityEngine.Experimental.Rendering
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Holds data of a visible light.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct VisibleLight
    {
        /// <summary>
        /// <para>Light type.</para>
        /// </summary>
        public LightType lightType;
        /// <summary>
        /// <para>Light color multiplied by intensity.</para>
        /// </summary>
        public Color finalColor;
        /// <summary>
        /// <para>Light's influence rectangle on screen.</para>
        /// </summary>
        public Rect screenRect;
        /// <summary>
        /// <para>Light transformation matrix.</para>
        /// </summary>
        public Matrix4x4 localToWorld;
        /// <summary>
        /// <para>Light range.</para>
        /// </summary>
        public float range;
        /// <summary>
        /// <para>Spot light angle.</para>
        /// </summary>
        public float spotAngle;
        private int instanceId;
        /// <summary>
        /// <para>Light flags, see VisibleLightFlags.</para>
        /// </summary>
        public VisibleLightFlags flags;
        /// <summary>
        /// <para>Accessor to Light component.</para>
        /// </summary>
        public Light light =>
            GetLightObject(this.instanceId);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern Light GetLightObject(int instanceId);
    }
}

