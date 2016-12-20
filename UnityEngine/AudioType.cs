namespace UnityEngine
{
    using System;

    /// <summary>
    /// <para>Type of the imported(native) data.</para>
    /// </summary>
    public enum AudioType
    {
        /// <summary>
        /// <para>Acc - not supported.</para>
        /// </summary>
        ACC = 1,
        /// <summary>
        /// <para>Aiff.</para>
        /// </summary>
        AIFF = 2,
        /// <summary>
        /// <para>iPhone hardware decoder, supports AAC, ALAC and MP3. Extracodecdata is a pointer to an FMOD_AUDIOQUEUE_EXTRACODECDATA structure.</para>
        /// </summary>
        AUDIOQUEUE = 0x18,
        /// <summary>
        /// <para>Impulse tracker.</para>
        /// </summary>
        IT = 10,
        /// <summary>
        /// <para>Protracker / Fasttracker MOD.</para>
        /// </summary>
        MOD = 12,
        /// <summary>
        /// <para>MP2/MP3 MPEG.</para>
        /// </summary>
        MPEG = 13,
        /// <summary>
        /// <para>Ogg vorbis.</para>
        /// </summary>
        OGGVORBIS = 14,
        /// <summary>
        /// <para>ScreamTracker 3.</para>
        /// </summary>
        S3M = 0x11,
        /// <summary>
        /// <para>3rd party / unknown plugin format.</para>
        /// </summary>
        UNKNOWN = 0,
        /// <summary>
        /// <para>VAG.</para>
        /// </summary>
        VAG = 0x17,
        /// <summary>
        /// <para>Microsoft WAV.</para>
        /// </summary>
        WAV = 20,
        /// <summary>
        /// <para>FastTracker 2 XM.</para>
        /// </summary>
        XM = 0x15,
        /// <summary>
        /// <para>Xbox360 XMA.</para>
        /// </summary>
        XMA = 0x16
    }
}

