namespace UnityEngine
{
    using System;

    /// <summary>
    /// <para>Determines how the audio clip is loaded in.</para>
    /// </summary>
    public enum AudioClipLoadType
    {
        DecompressOnLoad,
        CompressedInMemory,
        Streaming
    }
}

