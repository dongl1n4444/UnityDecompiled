namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Wind Zones add realism to the trees you create by making them wave their branches and leaves as if blown by the wind.</para>
    /// </summary>
    public sealed class WindZone : Component
    {
        /// <summary>
        /// <para>Defines the type of wind zone to be used (Spherical or Directional).</para>
        /// </summary>
        public WindZoneMode mode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Radius of the Spherical Wind Zone (only active if the WindZoneMode is set to Spherical).</para>
        /// </summary>
        public float radius { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The primary wind force.</para>
        /// </summary>
        public float windMain { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Defines the frequency of the wind changes.</para>
        /// </summary>
        public float windPulseFrequency { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Defines ow much the wind changes over time.</para>
        /// </summary>
        public float windPulseMagnitude { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The turbulence wind force.</para>
        /// </summary>
        public float windTurbulence { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

