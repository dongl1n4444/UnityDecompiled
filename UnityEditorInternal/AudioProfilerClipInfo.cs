namespace UnityEditorInternal
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct AudioProfilerClipInfo
    {
        public int assetInstanceId;
        public int assetNameOffset;
        public int loadState;
        public int internalLoadState;
        public int age;
        public int disposed;
        public int numChannelInstances;
    }
}

