namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Rendering;

    /// <summary>
    /// <para>The Render Settings contain values for a range of visual elements in your scene, like fog and ambient light.</para>
    /// </summary>
    public sealed class RenderSettings : UnityEngine.Object
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern UnityEngine.Object GetRenderSettings();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_get_ambientEquatorColor(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_get_ambientGroundColor(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_get_ambientLight(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_get_ambientProbe(out SphericalHarmonicsL2 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_get_ambientSkyColor(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_get_fogColor(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_set_ambientEquatorColor(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_set_ambientGroundColor(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_set_ambientLight(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_set_ambientProbe(ref SphericalHarmonicsL2 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_set_ambientSkyColor(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_set_fogColor(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Reset();

        /// <summary>
        /// <para>Ambient lighting coming from the sides.</para>
        /// </summary>
        public static Color ambientEquatorColor
        {
            get
            {
                Color color;
                INTERNAL_get_ambientEquatorColor(out color);
                return color;
            }
            set
            {
                INTERNAL_set_ambientEquatorColor(ref value);
            }
        }

        /// <summary>
        /// <para>Ambient lighting coming from below.</para>
        /// </summary>
        public static Color ambientGroundColor
        {
            get
            {
                Color color;
                INTERNAL_get_ambientGroundColor(out color);
                return color;
            }
            set
            {
                INTERNAL_set_ambientGroundColor(ref value);
            }
        }

        /// <summary>
        /// <para>How much the light from the Ambient Source affects the scene.</para>
        /// </summary>
        public static float ambientIntensity { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Flat ambient lighting color.</para>
        /// </summary>
        public static Color ambientLight
        {
            get
            {
                Color color;
                INTERNAL_get_ambientLight(out color);
                return color;
            }
            set
            {
                INTERNAL_set_ambientLight(ref value);
            }
        }

        /// <summary>
        /// <para>Ambient lighting mode.</para>
        /// </summary>
        public static AmbientMode ambientMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Custom or skybox ambient lighting data.</para>
        /// </summary>
        public static SphericalHarmonicsL2 ambientProbe
        {
            get
            {
                SphericalHarmonicsL2 sl;
                INTERNAL_get_ambientProbe(out sl);
                return sl;
            }
            set
            {
                INTERNAL_set_ambientProbe(ref value);
            }
        }

        [Obsolete("Use RenderSettings.ambientIntensity instead (UnityUpgradable) -> ambientIntensity", false)]
        public static float ambientSkyboxAmount
        {
            get
            {
                return ambientIntensity;
            }
            set
            {
                ambientIntensity = value;
            }
        }

        /// <summary>
        /// <para>Ambient lighting coming from above.</para>
        /// </summary>
        public static Color ambientSkyColor
        {
            get
            {
                Color color;
                INTERNAL_get_ambientSkyColor(out color);
                return color;
            }
            set
            {
                INTERNAL_set_ambientSkyColor(ref value);
            }
        }

        /// <summary>
        /// <para>Custom specular reflection cubemap.</para>
        /// </summary>
        public static Cubemap customReflection { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Default reflection mode.</para>
        /// </summary>
        public static DefaultReflectionMode defaultReflectionMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Cubemap resolution for default reflection.</para>
        /// </summary>
        public static int defaultReflectionResolution { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The fade speed of all flares in the scene.</para>
        /// </summary>
        public static float flareFadeSpeed { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The intensity of all flares in the scene.</para>
        /// </summary>
        public static float flareStrength { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Is fog enabled?</para>
        /// </summary>
        public static bool fog { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The color of the fog.</para>
        /// </summary>
        public static Color fogColor
        {
            get
            {
                Color color;
                INTERNAL_get_fogColor(out color);
                return color;
            }
            set
            {
                INTERNAL_set_fogColor(ref value);
            }
        }

        /// <summary>
        /// <para>The density of the exponential fog.</para>
        /// </summary>
        public static float fogDensity { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The ending distance of linear fog.</para>
        /// </summary>
        public static float fogEndDistance { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Fog mode to use.</para>
        /// </summary>
        public static FogMode fogMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The starting distance of linear fog.</para>
        /// </summary>
        public static float fogStartDistance { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Size of the Light halos.</para>
        /// </summary>
        public static float haloStrength { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The number of times a reflection includes other reflections.</para>
        /// </summary>
        public static int reflectionBounces { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>How much the skybox / custom cubemap reflection affects the scene.</para>
        /// </summary>
        public static float reflectionIntensity { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The global skybox to use.</para>
        /// </summary>
        public static Material skybox { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The light used by the procedural skybox.</para>
        /// </summary>
        public static Light sun { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

