namespace UnityEngine
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>The Audio Low Pass Filter passes low frequencies of an AudioSource or all sounds reaching an AudioListener, while removing frequencies higher than the Cutoff Frequency.</para>
    /// </summary>
    public sealed class AudioLowPassFilter : Behaviour
    {
        /// <summary>
        /// <para>Returns or sets the current custom frequency cutoff curve.</para>
        /// </summary>
        public AnimationCurve customCutoffCurve { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Lowpass cutoff frequency in hz. 10.0 to 22000.0. Default = 5000.0.</para>
        /// </summary>
        public float cutoffFrequency { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        [Obsolete("AudioLowPassFilter.lowpassResonaceQ is obsolete. Use lowpassResonanceQ instead (UnityUpgradable) -> lowpassResonanceQ", true), EditorBrowsable(EditorBrowsableState.Never)]
        public float lowpassResonaceQ
        {
            get
            {
                return this.lowpassResonanceQ;
            }
            set
            {
            }
        }

        /// <summary>
        /// <para>Determines how much the filter's self-resonance is dampened.</para>
        /// </summary>
        public float lowpassResonanceQ { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

