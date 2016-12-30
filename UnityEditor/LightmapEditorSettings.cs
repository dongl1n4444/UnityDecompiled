namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Various settings for the bake.</para>
    /// </summary>
    public sealed class LightmapEditorSettings
    {
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void AnalyzeLighting(out LightingStats enabled, out LightingStats active, out LightingStats inactive);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool GetGeometryHash(Renderer renderer, out Hash128 geometryHash);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool GetInputSystemHash(Renderer renderer, out Hash128 inputSystemHash);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool GetInstanceHash(Renderer renderer, out Hash128 instanceHash);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool GetInstanceResolution(Renderer renderer, out int width, out int height);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern Object GetLightmapSettings();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool GetSystemResolution(Renderer renderer, out int width, out int height);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool GetTerrainSystemResolution(Terrain terrain, out int width, out int height, out int numChunksInX, out int numChunksInY);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool HasClampedResolution(Renderer renderer);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool HasZeroAreaMesh(Renderer renderer);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool IsLightmappedOrDynamicLightmappedForRendering(Renderer renderer);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void Reset();

        [Obsolete("LightmapEditorSettings.aoAmount has been deprecated.", false)]
        public static float aoAmount
        {
            get => 
                0f;
            set
            {
            }
        }

        [Obsolete("LightmapEditorSettings.aoContrast has been deprecated.", false)]
        public static float aoContrast
        {
            get => 
                0f;
            set
            {
            }
        }

        /// <summary>
        /// <para>Ambient occlusion (AO) for direct lighting.</para>
        /// </summary>
        public static float aoExponentDirect { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Ambient occlusion (AO) for indirect lighting.</para>
        /// </summary>
        public static float aoExponentIndirect { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Beyond this distance a ray is considered to be unoccluded.</para>
        /// </summary>
        public static float aoMaxDistance { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        public static float bakeResolution { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        [Obsolete("LightmapEditorSettings.bounceBoost has been deprecated.", false)]
        public static float bounceBoost
        {
            get => 
                0f;
            set
            {
            }
        }

        [Obsolete("LightmapEditorSettings.bounceIntensity has been deprecated.", false)]
        public static float bounceIntensity
        {
            get => 
                0f;
            set
            {
            }
        }

        [Obsolete("LightmapEditorSettings.bounces has been deprecated.", false)]
        public static int bounces
        {
            get => 
                0;
            set
            {
            }
        }

        /// <summary>
        /// <para>Enable baked ambient occlusion (AO).</para>
        /// </summary>
        public static bool enableAmbientOcclusion { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        [Obsolete("LightmapEditorSettings.finalGatherContrastThreshold has been deprecated.", false)]
        public static float finalGatherContrastThreshold
        {
            get => 
                0f;
            set
            {
            }
        }

        [Obsolete("LightmapEditorSettings.finalGatherGradientThreshold has been deprecated.", false)]
        public static float finalGatherGradientThreshold
        {
            get => 
                0f;
            set
            {
            }
        }

        [Obsolete("LightmapEditorSettings.finalGatherInterpolationPoints has been deprecated.", false)]
        public static int finalGatherInterpolationPoints
        {
            get => 
                0;
            set
            {
            }
        }

        [Obsolete("LightmapEditorSettings.finalGatherRays has been deprecated.", false)]
        public static int finalGatherRays
        {
            get => 
                0;
            set
            {
            }
        }

        [Obsolete("LightmapEditorSettings.lastUsedResolution has been deprecated.", false)]
        public static float lastUsedResolution
        {
            get => 
                0f;
            set
            {
            }
        }

        [Obsolete("LightmapEditorSettings.lockAtlas has been deprecated.", false)]
        public static bool lockAtlas
        {
            get => 
                false;
            set
            {
            }
        }

        /// <summary>
        /// <para>The maximum height of an individual lightmap texture.</para>
        /// </summary>
        public static int maxAtlasHeight { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The maximum width of an individual lightmap texture.</para>
        /// </summary>
        public static int maxAtlasWidth { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Texel separation between shapes.</para>
        /// </summary>
        public static int padding { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        [Obsolete("LightmapEditorSettings.quality has been deprecated.", false)]
        public static LightmapBakeQuality quality
        {
            get => 
                LightmapBakeQuality.High;
            set
            {
            }
        }

        /// <summary>
        /// <para>Lightmap resolution in texels per world unit. Defines the resolution of Realtime GI if enabled. If Baked GI is enabled, this defines the resolution used for indirect lighting. Higher resolution may take a long time to bake.</para>
        /// </summary>
        public static float realtimeResolution { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Determines how Unity will compress baked reflection cubemap.</para>
        /// </summary>
        public static ReflectionCubemapCompression reflectionCubemapCompression { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        [Obsolete("resolution is now called realtimeResolution (UnityUpgradable) -> realtimeResolution", false)]
        public static float resolution
        {
            get => 
                realtimeResolution;
            set
            {
                realtimeResolution = value;
            }
        }

        [Obsolete("LightmapEditorSettings.skyLightColor has been deprecated.", false)]
        public static Color skyLightColor
        {
            get => 
                Color.black;
            set
            {
            }
        }

        [Obsolete("LightmapEditorSettings.skyLightIntensity has been deprecated.", false)]
        public static float skyLightIntensity
        {
            get => 
                0f;
            set
            {
            }
        }

        /// <summary>
        /// <para>Whether to use texture compression on the generated lightmaps.</para>
        /// </summary>
        public static bool textureCompression { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

