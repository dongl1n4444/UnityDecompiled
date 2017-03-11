namespace UnityEngine
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Rendering;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>General functionality for all renderers.</para>
    /// </summary>
    [RequireComponent(typeof(Transform))]
    public class Renderer : UnityEngine.Component
    {
        public void GetClosestReflectionProbes(List<ReflectionProbeBlendInfo> result)
        {
            this.GetClosestReflectionProbesInternal(result);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void GetClosestReflectionProbesInternal(object result);
        /// <summary>
        /// <para>Get per-renderer material property block.</para>
        /// </summary>
        /// <param name="dest"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void GetPropertyBlock(MaterialPropertyBlock dest);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_bounds(out Bounds value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_lightmapScaleOffset(out Vector4 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_localToWorldMatrix(out Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_realtimeLightmapScaleOffset(out Vector4 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_worldToLocalMatrix(out Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_lightmapScaleOffset(ref Vector4 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_realtimeLightmapScaleOffset(ref Vector4 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void RenderNow(int material);
        /// <summary>
        /// <para>Lets you add per-renderer material parameters without duplicating a material.</para>
        /// </summary>
        /// <param name="properties"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void SetPropertyBlock(MaterialPropertyBlock properties);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
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

        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Use shadowCastingMode instead.", false)]
        public bool castShadows
        {
            get => 
                (this.shadowCastingMode != ShadowCastingMode.Off);
            set
            {
                this.shadowCastingMode = !value ? ShadowCastingMode.Off : ShadowCastingMode.On;
            }
        }

        /// <summary>
        /// <para>Makes the rendered 3D object visible if enabled.</para>
        /// </summary>
        public bool enabled { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Has this renderer been statically batched with any other renderers?</para>
        /// </summary>
        public bool isPartOfStaticBatch { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Is this renderer visible in any camera? (Read Only)</para>
        /// </summary>
        public bool isVisible { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The index of the baked lightmap applied to this renderer.</para>
        /// </summary>
        public int lightmapIndex { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

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
            get => 
                Vector4.zero;
            set
            {
            }
        }

        [Obsolete("Use probeAnchor instead (UnityUpgradable) -> probeAnchor", true)]
        public Transform lightProbeAnchor
        {
            get => 
                this.probeAnchor;
            set
            {
                this.probeAnchor = value;
            }
        }

        /// <summary>
        /// <para>If set, the Renderer will use the Light Probe Proxy Volume component attached to the source game object.</para>
        /// </summary>
        public GameObject lightProbeProxyVolumeOverride { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The light probe interpolation type.</para>
        /// </summary>
        public LightProbeUsage lightProbeUsage { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

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
        public Material material { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Returns all the instantiated materials of this object.</para>
        /// </summary>
        public Material[] materials { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Specifies the mode for motion vector rendering.</para>
        /// </summary>
        public MotionVectorGenerationMode motionVectorGenerationMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Specifies whether this renderer has a per-object motion vector pass.</para>
        /// </summary>
        [Obsolete("Use motionVectorGenerationMode instead.", false)]
        public bool motionVectors
        {
            get => 
                (this.motionVectorGenerationMode == MotionVectorGenerationMode.Object);
            set
            {
                this.motionVectorGenerationMode = !value ? MotionVectorGenerationMode.Camera : MotionVectorGenerationMode.Object;
            }
        }

        /// <summary>
        /// <para>If set, Renderer will use this Transform's position to find the light or reflection probe.</para>
        /// </summary>
        public Transform probeAnchor { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The index of the realtime lightmap applied to this renderer.</para>
        /// </summary>
        public int realtimeLightmapIndex { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

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
        public bool receiveShadows { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Should reflection probes be used for this Renderer?</para>
        /// </summary>
        public ReflectionProbeUsage reflectionProbeUsage { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Does this object cast shadows?</para>
        /// </summary>
        public ShadowCastingMode shadowCastingMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The shared material of this object.</para>
        /// </summary>
        public Material sharedMaterial { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>All the shared materials of this object.</para>
        /// </summary>
        public Material[] sharedMaterials { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        internal int sortingGroupID { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        internal int sortingGroupOrder { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Unique ID of the Renderer's sorting layer.</para>
        /// </summary>
        public int sortingLayerID { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Name of the Renderer's sorting layer.</para>
        /// </summary>
        public string sortingLayerName { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Renderer's order within a sorting layer.</para>
        /// </summary>
        public int sortingOrder { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        internal int staticBatchIndex { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        internal Transform staticBatchRootTransform { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Should light probes be used for this Renderer?</para>
        /// </summary>
        [Obsolete("Use lightProbeUsage instead.", false)]
        public bool useLightProbes
        {
            get => 
                (this.lightProbeUsage != LightProbeUsage.Off);
            set
            {
                this.lightProbeUsage = !value ? LightProbeUsage.Off : LightProbeUsage.BlendProbes;
            }
        }

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

