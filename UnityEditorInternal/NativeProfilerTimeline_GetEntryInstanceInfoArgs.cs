namespace UnityEditorInternal
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct NativeProfilerTimeline_GetEntryInstanceInfoArgs
    {
        public int frameIndex;
        public int threadIndex;
        public int entryIndex;
        public int out_Id;
        public string out_Path;
        public string out_AllocationInfo;
        public string out_MetaData;
        public void Reset()
        {
            this.frameIndex = -1;
            this.threadIndex = -1;
            this.entryIndex = -1;
            this.out_Id = 0;
            this.out_Path = string.Empty;
            this.out_AllocationInfo = string.Empty;
            this.out_MetaData = string.Empty;
        }
    }
}

