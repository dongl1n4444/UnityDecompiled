namespace UnityEngine
{
    using System;

    /// <summary>
    /// <para>Value describing the current load state of the audio data associated with an AudioClip.</para>
    /// </summary>
    public enum AudioDataLoadState
    {
        Unloaded,
        Loading,
        Loaded,
        Failed
    }
}

