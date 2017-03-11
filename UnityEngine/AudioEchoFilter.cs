namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>The Audio Echo Filter repeats a sound after a given Delay, attenuating the repetitions based on the Decay Ratio.</para>
    /// </summary>
    [RequireComponent(typeof(AudioBehaviour))]
    public sealed class AudioEchoFilter : Behaviour
    {
        /// <summary>
        /// <para>Echo decay per delay. 0 to 1. 1.0 = No decay, 0.0 = total decay (i.e. simple 1 line delay). Default = 0.5.</para>
        /// </summary>
        public float decayRatio { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Echo delay in ms. 10 to 5000. Default = 500.</para>
        /// </summary>
        public float delay { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Volume of original signal to pass to output. 0.0 to 1.0. Default = 1.0.</para>
        /// </summary>
        public float dryMix { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Volume of echo signal to pass to output. 0.0 to 1.0. Default = 1.0.</para>
        /// </summary>
        public float wetMix { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

