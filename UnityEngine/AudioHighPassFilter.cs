namespace UnityEngine
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>The Audio High Pass Filter passes high frequencies of an AudioSource, and cuts off signals with frequencies lower than the Cutoff Frequency.</para>
    /// </summary>
    public sealed class AudioHighPassFilter : Behaviour
    {
        /// <summary>
        /// <para>Highpass cutoff frequency in hz. 10.0 to 22000.0. Default = 5000.0.</para>
        /// </summary>
        public float cutoffFrequency { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("AudioHighPassFilter.highpassResonaceQ is obsolete. Use highpassResonanceQ instead (UnityUpgradable) -> highpassResonanceQ", true)]
        public float highpassResonaceQ
        {
            get
            {
                return this.highpassResonanceQ;
            }
            set
            {
            }
        }

        /// <summary>
        /// <para>Determines how much the filter's self-resonance isdampened.</para>
        /// </summary>
        public float highpassResonanceQ { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

