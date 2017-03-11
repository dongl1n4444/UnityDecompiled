namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Script interface for.</para>
    /// </summary>
    public sealed class QualitySettings : UnityEngine.Object
    {
        [ExcludeFromDocs]
        public static void DecreaseLevel()
        {
            bool applyExpensiveChanges = false;
            DecreaseLevel(applyExpensiveChanges);
        }

        /// <summary>
        /// <para>Decrease the current quality level.</para>
        /// </summary>
        /// <param name="applyExpensiveChanges">Should expensive changes be applied (Anti-aliasing etc).</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void DecreaseLevel([DefaultValue("false")] bool applyExpensiveChanges);
        /// <summary>
        /// <para>Returns the current graphics quality level.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern int GetQualityLevel();
        [ExcludeFromDocs]
        public static void IncreaseLevel()
        {
            bool applyExpensiveChanges = false;
            IncreaseLevel(applyExpensiveChanges);
        }

        /// <summary>
        /// <para>Increase the current quality level.</para>
        /// </summary>
        /// <param name="applyExpensiveChanges">Should expensive changes be applied (Anti-aliasing etc).</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void IncreaseLevel([DefaultValue("false")] bool applyExpensiveChanges);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_get_shadowCascade4Split(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_set_shadowCascade4Split(ref Vector3 value);
        [ExcludeFromDocs]
        public static void SetQualityLevel(int index)
        {
            bool applyExpensiveChanges = true;
            SetQualityLevel(index, applyExpensiveChanges);
        }

        /// <summary>
        /// <para>Sets a new graphics quality level.</para>
        /// </summary>
        /// <param name="index">Quality index to set.</param>
        /// <param name="applyExpensiveChanges">Should expensive changes be applied (Anti-aliasing etc).</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void SetQualityLevel(int index, [DefaultValue("true")] bool applyExpensiveChanges);

        /// <summary>
        /// <para>Active color space (Read Only).</para>
        /// </summary>
        public static ColorSpace activeColorSpace { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Global anisotropic filtering mode.</para>
        /// </summary>
        public static AnisotropicFiltering anisotropicFiltering { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Set The AA Filtering option.</para>
        /// </summary>
        public static int antiAliasing { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Async texture upload provides timesliced async texture upload on the render thread with tight control over memory and timeslicing. There are no allocations except for the ones which driver has to do. To read data and upload texture data a ringbuffer whose size can be controlled is re-used.
        /// 
        /// Use asyncUploadBufferSize to set the buffer size for asynchronous texture uploads. The size is in megabytes. Minimum value is 2 and maximum is 512. Although the buffer will resize automatically to fit the largest texture currently loading, it is recommended to set the value approximately to the size of biggest texture used in the scene to avoid re-sizing of the buffer which can incur performance cost.</para>
        /// </summary>
        public static int asyncUploadBufferSize { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Async texture upload provides timesliced async texture upload on the render thread with tight control over memory and timeslicing. There are no allocations except for the ones which driver has to do. To read data and upload texture data a ringbuffer whose size can be controlled is re-used.
        /// 
        /// Use asyncUploadTimeSlice to set the time-slice in milliseconds for asynchronous texture uploads per 
        /// frame. Minimum value is 1 and maximum is 33.</para>
        /// </summary>
        public static int asyncUploadTimeSlice { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>If enabled, billboards will face towards camera position rather than camera orientation.</para>
        /// </summary>
        public static bool billboardsFaceCameraPosition { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Blend weights.</para>
        /// </summary>
        public static BlendWeights blendWeights { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        [Obsolete("Use GetQualityLevel and SetQualityLevel", false)]
        public static QualityLevel currentLevel
        {
            get => 
                ((QualityLevel) GetQualityLevel());
            set
            {
                SetQualityLevel((int) value, true);
            }
        }

        /// <summary>
        /// <para>Desired color space (Read Only).</para>
        /// </summary>
        public static ColorSpace desiredColorSpace { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Global multiplier for the LOD's switching distance.</para>
        /// </summary>
        public static float lodBias { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>A texture size limit applied to all textures.</para>
        /// </summary>
        public static int masterTextureLimit { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>A maximum LOD level. All LOD groups.</para>
        /// </summary>
        public static int maximumLODLevel { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Maximum number of frames queued up by graphics driver.</para>
        /// </summary>
        public static int maxQueuedFrames { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The indexed list of available Quality Settings.</para>
        /// </summary>
        public static string[] names { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Budget for how many ray casts can be performed per frame for approximate collision testing.</para>
        /// </summary>
        public static int particleRaycastBudget { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The maximum number of pixel lights that should affect any object.</para>
        /// </summary>
        public static int pixelLightCount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Enables realtime reflection probes.</para>
        /// </summary>
        public static bool realtimeReflectionProbes { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The normalized cascade distribution for a 2 cascade setup. The value defines the position of the cascade with respect to Zero.</para>
        /// </summary>
        public static float shadowCascade2Split { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The normalized cascade start position for a 4 cascade setup. Each member of the vector defines the normalized position of the coresponding cascade with respect to Zero.</para>
        /// </summary>
        public static Vector3 shadowCascade4Split
        {
            get
            {
                Vector3 vector;
                INTERNAL_get_shadowCascade4Split(out vector);
                return vector;
            }
            set
            {
                INTERNAL_set_shadowCascade4Split(ref value);
            }
        }

        /// <summary>
        /// <para>Number of cascades to use for directional light shadows.</para>
        /// </summary>
        public static int shadowCascades { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Shadow drawing distance.</para>
        /// </summary>
        public static float shadowDistance { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Offset shadow frustum near plane.</para>
        /// </summary>
        public static float shadowNearPlaneOffset { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Directional light shadow projection.</para>
        /// </summary>
        public static ShadowProjection shadowProjection { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The default resolution of the shadow maps.</para>
        /// </summary>
        public static ShadowResolution shadowResolution { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Realtime Shadows type to be used.</para>
        /// </summary>
        public static ShadowQuality shadows { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Should soft blending be used for particles?</para>
        /// </summary>
        public static bool softParticles { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Use a two-pass shader for the vegetation in the terrain engine.</para>
        /// </summary>
        public static bool softVegetation { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The VSync Count.</para>
        /// </summary>
        public static int vSyncCount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

