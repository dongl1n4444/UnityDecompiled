namespace UnityEditorInternal
{
    using System;

    internal class AudioProfilerGroupInfoWrapper
    {
        public bool addToRoot;
        public string assetName;
        public AudioProfilerGroupInfo info;
        public string objectName;

        public AudioProfilerGroupInfoWrapper(AudioProfilerGroupInfo info, string assetName, string objectName, bool addToRoot)
        {
            this.info = info;
            this.assetName = assetName;
            this.objectName = objectName;
            this.addToRoot = addToRoot;
        }
    }
}

