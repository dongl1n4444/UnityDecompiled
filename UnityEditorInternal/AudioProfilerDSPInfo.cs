namespace UnityEditorInternal
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct AudioProfilerDSPInfo
    {
        public int id;
        public int target;
        public int targetPort;
        public int numChannels;
        public int nameOffset;
        public float weight;
        public float cpuLoad;
        public float level1;
        public float level2;
        public int numLevels;
        public int flags;
    }
}

