namespace UnityEngineInternal.Input
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct NativeTextEvent
    {
        public NativeInputEvent baseEvent;
        public int utf32Character;
    }
}

