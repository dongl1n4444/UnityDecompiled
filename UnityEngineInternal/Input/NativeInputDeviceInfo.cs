namespace UnityEngineInternal.Input
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct NativeInputDeviceInfo
    {
        public int deviceId;
        public string deviceDescriptor;
    }
}

