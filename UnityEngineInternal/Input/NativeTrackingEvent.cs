namespace UnityEngineInternal.Input
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public struct NativeTrackingEvent
    {
        public NativeInputEvent baseEvent;
        public int nodeId;
        public Vector3 localPosition;
        public Quaternion localRotation;
    }
}

