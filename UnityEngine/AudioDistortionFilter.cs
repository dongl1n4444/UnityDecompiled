namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>The Audio Distortion Filter distorts the sound from an AudioSource or.</para>
    /// </summary>
    [RequireComponent(typeof(AudioBehaviour))]
    public sealed class AudioDistortionFilter : Behaviour
    {
        /// <summary>
        /// <para>Distortion value. 0.0 to 1.0. Default = 0.5.</para>
        /// </summary>
        public float distortionLevel { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

