namespace UnityEngineInternal.Input
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct NativeInputEvent
    {
        public NativeInputEventType type;
        public int sizeInBytes;
        public int deviceId;
        public double time;
    }
}

