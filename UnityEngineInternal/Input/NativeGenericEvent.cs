namespace UnityEngineInternal.Input
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct NativeGenericEvent
    {
        public NativeInputEvent baseEvent;
        public int controlIndex;
        public int rawValue;
        public double scaledValue;
    }
}

