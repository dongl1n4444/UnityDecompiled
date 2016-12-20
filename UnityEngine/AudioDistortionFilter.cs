namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>The Audio Distortion Filter distorts the sound from an AudioSource or sounds reaching the AudioListener.</para>
    /// </summary>
    public sealed class AudioDistortionFilter : Behaviour
    {
        /// <summary>
        /// <para>Distortion value. 0.0 to 1.0. Default = 0.5.</para>
        /// </summary>
        public float distortionLevel { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

