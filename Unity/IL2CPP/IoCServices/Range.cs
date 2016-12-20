namespace Unity.IL2CPP.IoCServices
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct Range
    {
        public int start;
        public int length;
        public Range(int start_, int length_)
        {
            this.start = start_;
            this.length = length_;
        }
    }
}

