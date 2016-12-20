namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Rendering;

    /// <summary>
    /// <para>Various settings for the bake.</para>
    /// </summary>
    public sealed class LightmapEditorSettings
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool GetGeometryHash(Renderer renderer, out Hash128 geometryHash);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool GetInputSystemHash(Renderer renderer, out Hash128 inputSystemHash);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool GetInstanceHash(Renderer renderer, out Hash128 instanceHash);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool GetInstanceResolution(Renderer renderer, out int width, out int height);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern Object GetLightmapSettings();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool GetSystemResolution(Renderer renderer, out int width, out int height);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool GetTerrainSystemResolution(Terrain terrain, out int width, out int height, out int numChunksInX, out int numChunksInY);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool HasClampedResolution(Renderer renderer);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool HasZeroAreaMesh(Renderer renderer);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool IsLightmappedOrDynamicLightmappedForRendering(Renderer renderer);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Reset();

        [Obsolete("LightmapEditorSettings.aoAmount has been deprecated.", false)]
        public static float aoAmount
        {
            get
            {
                return 0f;
            }
            set
            {
            }
        }

        [Obsolete("LightmapEditorSettings.aoContrast has been deprecated.", false)]
        public static float aoContrast
        {
            get
            {
                return 0f;
            }
            set
            {
            }
        }

        /// <summary>
        /// <para>Beyond this distance a ray is considered to be unoccluded.</para>
        /// </summary>
        public static float aoMaxDistance { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        public static float bakeResolution { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        [Obsolete("LightmapEditorSettings.bounceBoost has been deprecated.", false)]
        public static float bounceBoost
        {
            get
            {
                return 0f;
            }
            set
            {
            }
        }

        [Obsolete("LightmapEditorSettings.bounceIntensity has been deprecated.", false)]
        public static float bounceIntensity
        {
            get
            {
                return 0f;
            }
            set
            {
            }
        }

        [Obsolete("LightmapEditorSettings.bounces has been deprecated.", false)]
        public static int bounces
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        [Obsolete("LightmapEditorSettings.finalGatherContrastThreshold has been deprecated.", false)]
        public static float finalGatherContrastThreshold
        {
            get
            {
                return 0f;
            }
            set
            {
            }
        }

        [Obsolete("LightmapEditorSettings.finalGatherGradientThreshold has been deprecated.", false)]
        public static float finalGatherGradientThreshold
        {
            get
            {
                return 0f;
            }
            set
            {
            }
        }

        [Obsolete("LightmapEditorSettings.finalGatherInterpolationPoints has been deprecated.", false)]
        public static int finalGatherInterpolationPoints
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        [Obsolete("LightmapEditorSettings.finalGatherRays has been deprecated.", false)]
        public static int finalGatherRays
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        [Obsolete("LightmapEditorSettings.lastUsedResolution has been deprecated.", false)]
        public static float lastUsedResolution
        {
            get
            {
                return 0f;
            }
            set
            {
            }
        }

        [Obsolete("LightmapEditorSettings.lockAtlas has been deprecated.", false)]
        public static bool lockAtlas
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        /// <summary>
        /// <para>The maximum height of an individual lightmap texture.</para>
        /// </summary>
        public static int maxAtlasHeight { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The maximum width of an individual lightmap texture.</para>
        /// </summary>
        public static int maxAtlasWidth { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Texel separation between shapes.</para>
        /// </summary>
        public static int padding { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        [Obsolete("LightmapEditorSettings.quality has been deprecated.", false)]
        public static LightmapBakeQuality quality
        {
            get
            {
                return LightmapBakeQuality.High;
            }
            set
            {
            }
        }

        /// <summary>
        /// <para>Lightmap resolution in texels per world unit. Defines the resolution of Realtime GI if enabled. If Baked GI is enabled, this defines the resolution used for indirect lighting. Higher resolution may take a long time to bake.</para>
        /// </summary>
        public static float realtimeResolution { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Determines how Unity will compress baked reflection cubemap.</para>
        /// </summary>
        public static ReflectionCubemapCompression reflectionCubemapCompression { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        [Obsolete("resolution is now called realtimeResolution (UnityUpgradable) -> realtimeResolution", false)]
        public static float resolution
        {
            get
            {
                return realtimeResolution;
            }
            set
            {
                realtimeResolution = value;
            }
        }

        [Obsolete("LightmapEditorSettings.skyLightColor has been deprecated.", false)]
        public static Color skyLightColor
        {
            get
            {
                return Color.black;
            }
            set
            {
            }
        }

        [Obsolete("LightmapEditorSettings.skyLightIntensity has been deprecated.", false)]
        public static float skyLightIntensity
        {
            get
            {
                return 0f;
            }
            set
            {
            }
        }

        /// <summary>
        /// <para>Whether to use texture compression on the generated lightmaps.</para>
        /// </summary>
        public static bool textureCompression { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

