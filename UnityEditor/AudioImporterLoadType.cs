namespace UnityEditor
{
    using System;
    using System.ComponentModel;

    [Obsolete("UnityEditor.AudioImporterLoadType has been deprecated. Use UnityEngine.AudioClipLoadType instead (UnityUpgradable) -> [UnityEngine] UnityEngine.AudioClipLoadType", true), EditorBrowsable(EditorBrowsableState.Never)]
    public enum AudioImporterLoadType
    {
        CompressedInMemory = -1,
        DecompressOnLoad = -1,
        [Obsolete("UnityEditor.AudioImporterLoadType.StreamFromDisc has been deprecated. Use UnityEngine.AudioClipLoadType.Streaming instead (UnityUpgradable) -> UnityEngine.AudioClipLoadType.Streaming", true)]
        StreamFromDisc = -1
    }
}

