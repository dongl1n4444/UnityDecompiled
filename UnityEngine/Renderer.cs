namespace UnityEngine
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Rendering;

    /// <summary>
    /// <para>General functionality for all renderers.</para>
    /// </summary>
    public class Renderer : UnityEngine.Component
    {
        public void GetClosestReflectionProbes(List<ReflectionProbeBlendInfo> result)
        {
            this.GetClosestReflectionProbesInternal(result);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void GetClosestReflectionProbesInternal(object result);
        /// <summary>
        /// <para>Get per-renderer material property block.</para>
        /// </summary>
        /// <param name="dest"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void GetPropertyBlock(MaterialPropertyBlock dest);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_bounds(out Bounds value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_lightmapScaleOffset(out Vector4 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_localToWorldMatrix(out Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_realtimeLightmapScaleOffset(out Vector4 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_worldToLocalMatrix(out Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_lightmapScaleOffset(ref Vector4 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_realtimeLightmapScaleOffset(ref Vector4 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void RenderNow(int material);
        /// <summary>
        /// <para>Lets you add per-renderer material parameters without duplicating a material.</para>
        /// </summary>
        /// <param name="properties"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetPropertyBlock(MaterialPropertyBlock properties);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void SetStaticBatchInfo(int firstSubMesh, int subMeshCount);

        /// <summary>
        /// <para>The bounding volume of the renderer (Read Only).</para>
        /// </summary>
        public Bounds bounds
        {
            get
            {
                Bounds bounds;
                this.INTERNAL_get_bounds(out bounds);
                return bounds;
            }
        }

        [Obsolete("Property castShadows has been deprecated. Use shadowCastingMode instead."), EditorBrowsable(EditorBrowsableState.Never)]
        public bool castShadows { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Makes the rendered 3D object visible if enabled.</para>
        /// </summary>
        public bool enabled { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Has this renderer been statically batched with any other renderers?</para>
        /// </summary>
        public bool isPartOfStaticBatch { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Is this renderer visible in any camera? (Read Only)</para>
        /// </summary>
        public bool isVisible { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The index of the baked lightmap applied to this renderer.</para>
        /// </summary>
        public int lightmapIndex { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The UV scale &amp; offset used for a lightmap.</para>
        /// </summary>
        public Vector4 lightmapScaleOffset
        {
            get
            {
                Vector4 vector;
                this.INTERNAL_get_lightmapScaleOffset(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_lightmapScaleOffset(ref value);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Property lightmapTilingOffset has been deprecated. Use lightmapScaleOffset (UnityUpgradable) -> lightmapScaleOffset", true)]
        public Vector4 lightmapTilingOffset
        {
            get
            {
                return Vector4.zero;
            }
            set
            {
            }
        }

        [Obsolete("Use probeAnchor instead (UnityUpgradable) -> probeAnchor", true)]
        public Transform lightProbeAnchor
        {
            get
            {
                return this.probeAnchor;
            }
            set
            {
                this.probeAnchor = value;
            }
        }

        /// <summary>
        /// <para>If set, the Renderer will use the Light Probe Proxy Volume component attached to the source GameObject.</para>
        /// </summary>
        public GameObject lightProbeProxyVolumeOverride { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The light probe interpolation type.</para>
        /// </summary>
        public LightProbeUsage lightProbeUsage { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Matrix that transforms a point from local space into world space (Read Only).</para>
        /// </summary>
        public Matrix4x4 localToWorldMatrix
        {
            get
            {
                Matrix4x4 matrixx;
                this.INTERNAL_get_localToWorldMatrix(out matrixx);
                return matrixx;
            }
        }

        /// <summary>
        /// <para>Returns the first instantiated Material assigned to the renderer.</para>
        /// </summary>
        public Material material { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Returns all the instantiated materials of this object.</para>
        /// </summary>
        public Material[] materials { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Specifies the mode for motion vector rendering.</para>
        /// </summary>
        public MotionVectorGenerationMode motionVectorGenerationMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        [Obsolete("motionVectors property is deprecated. Use motionVectorGenerationMode instead.")]
        private bool motionVectors
        {
            get
            {
                return (this.motionVectorGenerationMode == MotionVectorGenerationMode.Object);
            }
            set
            {
                this.motionVectorGenerationMode = !value ? MotionVectorGenerationMode.Camera : MotionVectorGenerationMode.Object;
            }
        }

        /// <summary>
        /// <para>If set, Renderer will use this Transform's position to find the light or reflection probe.</para>
        /// </summary>
        public Transform probeAnchor { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The index of the realtime lightmap applied to this renderer.</para>
        /// </summary>
        public int realtimeLightmapIndex { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The UV scale &amp; offset used for a realtime lightmap.</para>
        /// </summary>
        public Vector4 realtimeLightmapScaleOffset
        {
            get
            {
                Vector4 vector;
                this.INTERNAL_get_realtimeLightmapScaleOffset(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_realtimeLightmapScaleOffset(ref value);
            }
        }

        /// <summary>
        /// <para>Does this object receive shadows?</para>
        /// </summary>
        public bool receiveShadows { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Should reflection probes be used for this Renderer?</para>
        /// </summary>
        public ReflectionProbeUsage reflectionProbeUsage { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Does this object cast shadows?</para>
        /// </summary>
        public ShadowCastingMode shadowCastingMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The shared material of this object.</para>
        /// </summary>
        public Material sharedMaterial { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>All the shared materials of this object.</para>
        /// </summary>
        public Material[] sharedMaterials { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Unique ID of the Renderer's sorting layer.</para>
        /// </summary>
        public int sortingLayerID { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Name of the Renderer's sorting layer.</para>
        /// </summary>
        public string sortingLayerName { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Renderer's order within a sorting layer.</para>
        /// </summary>
        public int sortingOrder { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        internal int staticBatchIndex { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        internal Transform staticBatchRootTransform { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Should light probes be used for this Renderer?</para>
        /// </summary>
        [Obsolete("useLightProbes property is deprecated. Use lightProbeUsage instead.")]
        public bool useLightProbes { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Matrix that transforms a point from world space into local space (Read Only).</para>
        /// </summary>
        public Matrix4x4 worldToLocalMatrix
        {
            get
            {
                Matrix4x4 matrixx;
                this.INTERNAL_get_worldToLocalMatrix(out matrixx);
                return matrixx;
            }
        }
    }
}

