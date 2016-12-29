﻿namespace UnityEngine
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>The Audio Reverb Filter takes an Audio Clip and distorts it to create a custom reverb effect.</para>
    /// </summary>
    public sealed class AudioReverbFilter : Behaviour
    {
        /// <summary>
        /// <para>Decay HF Ratio : High-frequency to low-frequency decay time ratio. Ranges from 0.1 to 2.0. Default is 0.5.</para>
        /// </summary>
        public float decayHFRatio { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Reverberation decay time at low-frequencies in seconds. Ranges from 0.1 to 20.0. Default is 1.0.</para>
        /// </summary>
        public float decayTime { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Reverberation density (modal density) in percent. Ranges from 0.0 to 100.0. Default is 100.0.</para>
        /// </summary>
        public float density { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Reverberation diffusion (echo density) in percent. Ranges from 0.0 to 100.0. Default is 100.0.</para>
        /// </summary>
        public float diffusion { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Mix level of dry signal in output in mB. Ranges from -10000.0 to 0.0. Default is 0.</para>
        /// </summary>
        public float dryLevel { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Reference high frequency in Hz. Ranges from 20.0 to 20000.0. Default is 5000.0.</para>
        /// </summary>
        public float hfReference { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Reference low-frequency in Hz. Ranges from 20.0 to 1000.0. Default is 250.0.</para>
        /// </summary>
        public float lfReference { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        [Obsolete("AudioReverbFilter.lFReference is obsolete. Use lfReference instead (UnityUpgradable) -> lfReference", true), EditorBrowsable(EditorBrowsableState.Never)]
        public float lFReference
        {
            get => 
                this.lfReference;
            set
            {
            }
        }

        /// <summary>
        /// <para>Late reverberation level relative to room effect in mB. Ranges from -10000.0 to 2000.0. Default is 0.0.</para>
        /// </summary>
        public float reflectionsDelay { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Early reflections level relative to room effect in mB. Ranges from -10000.0 to 1000.0. Default is -10000.0.</para>
        /// </summary>
        public float reflectionsLevel { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Late reverberation delay time relative to first reflection in seconds. Ranges from 0.0 to 0.1. Default is 0.04.</para>
        /// </summary>
        public float reverbDelay { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Late reverberation level relative to room effect in mB. Ranges from -10000.0 to 2000.0. Default is 0.0.</para>
        /// </summary>
        public float reverbLevel { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Set/Get reverb preset properties.</para>
        /// </summary>
        public AudioReverbPreset reverbPreset { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Room effect level at low frequencies in mB. Ranges from -10000.0 to 0.0. Default is 0.0.</para>
        /// </summary>
        public float room { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Room effect high-frequency level re. low frequency level in mB. Ranges from -10000.0 to 0.0. Default is 0.0.</para>
        /// </summary>
        public float roomHF { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Room effect low-frequency level in mB. Ranges from -10000.0 to 0.0. Default is 0.0.</para>
        /// </summary>
        public float roomLF { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Rolloff factor for room effect. Ranges from 0.0 to 10.0. Default is 10.0.</para>
        /// </summary>
        public float roomRolloff { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

