namespace UnityEngine
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>The Audio High Pass Filter passes high frequencies of an AudioSource and.</para>
    /// </summary>
    [RequireComponent(typeof(AudioBehaviour))]
    public sealed class AudioHighPassFilter : Behaviour
    {
        /// <summary>
        /// <para>Highpass cutoff frequency in hz. 10.0 to 22000.0. Default = 5000.0.</para>
        /// </summary>
        public float cutoffFrequency { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        [Obsolete("AudioHighPassFilter.highpassResonaceQ is obsolete. Use highpassResonanceQ instead (UnityUpgradable) -> highpassResonanceQ", true), EditorBrowsable(EditorBrowsableState.Never)]
        public float highpassResonaceQ
        {
            get => 
                this.highpassResonanceQ;
            set
            {
            }
        }

        /// <summary>
        /// <para>Determines how much the filter's self-resonance isdampened.</para>
        /// </summary>
        public float highpassResonanceQ { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

