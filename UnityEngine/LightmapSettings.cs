namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>Stores lightmaps of the scene.</para>
    /// </summary>
    public sealed class LightmapSettings : UnityEngine.Object
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Reset();

        [Obsolete("bakedColorSpace is no longer valid. Use QualitySettings.desiredColorSpace.", false)]
        public static ColorSpace bakedColorSpace
        {
            get => 
                QualitySettings.desiredColorSpace;
            set
            {
            }
        }

        /// <summary>
        /// <para>Lightmap array.</para>
        /// </summary>
        public static LightmapData[] lightmaps { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Non-directional, Directional or Directional Specular lightmaps rendering mode.</para>
        /// </summary>
        public static LightmapsMode lightmapsMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        [Obsolete("Use lightmapsMode property")]
        public static LightmapsModeLegacy lightmapsModeLegacy { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Holds all data needed by the light probes.</para>
        /// </summary>
        public static LightProbes lightProbes { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

