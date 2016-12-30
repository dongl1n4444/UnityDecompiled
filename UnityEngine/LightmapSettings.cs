namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Stores lightmaps of the scene.</para>
    /// </summary>
    public sealed class LightmapSettings : UnityEngine.Object
    {
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void Reset();

        [Obsolete("Use QualitySettings.desiredColorSpace instead.", false)]
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
        public static LightmapData[] lightmaps { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Non-directional, Directional or Directional Specular lightmaps rendering mode.</para>
        /// </summary>
        public static LightmapsMode lightmapsMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        [Obsolete("Use lightmapsMode instead.", false)]
        public static LightmapsModeLegacy lightmapsModeLegacy
        {
            get => 
                LightmapsModeLegacy.Single;
            set
            {
            }
        }

        /// <summary>
        /// <para>Holds all data needed by the light probes.</para>
        /// </summary>
        public static LightProbes lightProbes { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

