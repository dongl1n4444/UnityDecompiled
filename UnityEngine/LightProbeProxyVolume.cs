namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>The light probe proxy volume component offers the possibility to use higher resolution lighting for large non-static objects.</para>
    /// </summary>
    public sealed class LightProbeProxyVolume : Behaviour
    {
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_boundsGlobal(out Bounds value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_originCustom(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_sizeCustom(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_originCustom(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_sizeCustom(ref Vector3 value);
        /// <summary>
        /// <para>Triggers an update of the light probe volume.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void Update();

        /// <summary>
        /// <para>The bounding box mode for generating the 3D grid of interpolated light probes.</para>
        /// </summary>
        public BoundingBoxMode boundingBoxMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The world-space bounding box in which the 3D grid of interpolated light probes is generated.</para>
        /// </summary>
        public Bounds boundsGlobal
        {
            get
            {
                Bounds bounds;
                this.INTERNAL_get_boundsGlobal(out bounds);
                return bounds;
            }
        }

        /// <summary>
        /// <para>The 3D grid resolution on the X axis. This property is used only when the Resolution Mode is set to Custom. The final resolution will be the closest power of 2.</para>
        /// </summary>
        public int gridResolutionX { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The 3D grid resolution on the Y axis. This property is used only when the Resolution Mode is set to Custom. The final resolution will be the closest power of 2.</para>
        /// </summary>
        public int gridResolutionY { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The 3D grid resolution on the Z axis. This property is used only when the Resolution Mode is set to Custom. The final resolution will be the closest power of 2.</para>
        /// </summary>
        public int gridResolutionZ { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Checks if this feature is supported by the graphics hardware or by the graphics API used. The feature requires at least Shader Model 4 including 32-bit floating-point 3D texture support with linear interpolation.</para>
        /// </summary>
        public static bool isFeatureSupported { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The local-space origin of the bounding box in which the 3D grid of interpolated light probes is generated. This is used when the Bounding Box Mode property is set to Custom.</para>
        /// </summary>
        public Vector3 originCustom
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_originCustom(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_originCustom(ref value);
            }
        }

        /// <summary>
        /// <para>Interpolated light probe density. This value is used only when the Resolution Mode is Automatic.</para>
        /// </summary>
        public float probeDensity { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The mode in which the interpolated light probe positions are generated.</para>
        /// </summary>
        public ProbePositionMode probePositionMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Sets the way the light probe volume will refresh.</para>
        /// </summary>
        public RefreshMode refreshMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The resolution mode for generating the grid of interpolated light probes.</para>
        /// </summary>
        public ResolutionMode resolutionMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The size of the bounding box in which the 3D grid of interpolated light probes is generated. This is used when the Bounding Box Mode property is set to Custom.</para>
        /// </summary>
        public Vector3 sizeCustom
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_sizeCustom(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_sizeCustom(ref value);
            }
        }

        /// <summary>
        /// <para>The bounding box mode for generating a grid of interpolated light probes.</para>
        /// </summary>
        public enum BoundingBoxMode
        {
            AutomaticLocal,
            AutomaticWorld,
            Custom
        }

        /// <summary>
        /// <para>The mode in which the interpolated light probe positions are generated.</para>
        /// </summary>
        public enum ProbePositionMode
        {
            CellCorner,
            CellCenter
        }

        /// <summary>
        /// <para>An enum describing the way a light probe volume refreshes in the Player.</para>
        /// </summary>
        public enum RefreshMode
        {
            Automatic,
            EveryFrame,
            ViaScripting
        }

        /// <summary>
        /// <para>The resolution mode for generating a grid of interpolated light probes.</para>
        /// </summary>
        public enum ResolutionMode
        {
            Automatic,
            Custom
        }
    }
}

