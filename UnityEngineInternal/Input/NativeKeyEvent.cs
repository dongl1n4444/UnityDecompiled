namespace UnityEngineInternal.Input
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public struct NativeKeyEvent
    {
        public NativeInputEvent baseEvent;
        public KeyCode key;
        public int modifiers;
    }
}

