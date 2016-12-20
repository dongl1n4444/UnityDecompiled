namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>Reverb Zones are used when you want to create location based ambient effects in the scene.</para>
    /// </summary>
    public sealed class AudioReverbZone : Behaviour
    {
        /// <summary>
        /// <para>High-frequency to mid-frequency decay time ratio.</para>
        /// </summary>
        public float decayHFRatio { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Reverberation decay time at mid frequencies.</para>
        /// </summary>
        public float decayTime { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Value that controls the modal density in the late reverberation decay.</para>
        /// </summary>
        public float density { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Value that controls the echo density in the late reverberation decay.</para>
        /// </summary>
        public float diffusion { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Reference high frequency (hz).</para>
        /// </summary>
        public float HFReference { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Reference low frequency (hz).</para>
        /// </summary>
        public float LFReference { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The distance from the centerpoint that the reverb will not have any effect. Default = 15.0.</para>
        /// </summary>
        public float maxDistance { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The distance from the centerpoint that the reverb will have full effect at. Default = 10.0.</para>
        /// </summary>
        public float minDistance { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Early reflections level relative to room effect.</para>
        /// </summary>
        public int reflections { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Initial reflection delay time.</para>
        /// </summary>
        public float reflectionsDelay { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Late reverberation level relative to room effect.</para>
        /// </summary>
        public int reverb { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Late reverberation delay time relative to initial reflection.</para>
        /// </summary>
        public float reverbDelay { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Set/Get reverb preset properties.</para>
        /// </summary>
        public AudioReverbPreset reverbPreset { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Room effect level (at mid frequencies).</para>
        /// </summary>
        public int room { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Relative room effect level at high frequencies.</para>
        /// </summary>
        public int roomHF { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Relative room effect level at low frequencies.</para>
        /// </summary>
        public int roomLF { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Like rolloffscale in global settings, but for reverb room size effect.</para>
        /// </summary>
        public float roomRolloffFactor { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

