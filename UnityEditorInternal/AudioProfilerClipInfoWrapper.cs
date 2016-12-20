namespace UnityEditorInternal
{
    using System;

    internal class AudioProfilerClipInfoWrapper
    {
        public string assetName;
        public AudioProfilerClipInfo info;

        public AudioProfilerClipInfoWrapper(AudioProfilerClipInfo info, string assetName)
        {
            this.info = info;
            this.assetName = assetName;
        }
    }
}

