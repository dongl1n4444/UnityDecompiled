namespace UnityEngine.Audio
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    /// <summary>
    /// <para>Object representing a group in the mixer.</para>
    /// </summary>
    public class AudioMixerGroup : UnityEngine.Object
    {
        internal AudioMixerGroup()
        {
        }

        public AudioMixer audioMixer { [MethodImpl(MethodImplOptions.InternalCall)] get; }
    }
}

