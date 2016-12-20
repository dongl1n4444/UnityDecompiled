namespace UnityEditor.iOS
{
    using System;
    using System.Runtime.InteropServices;

    public static class Usbmuxd
    {
        private const string nativeDll = "__Internal";

        [DllImport("__Internal", CharSet=CharSet.Ansi)]
        public static extern bool StartIosProxy(ushort localPort, ushort devicePort, [MarshalAs(UnmanagedType.LPStr)] string deviceId);
        [DllImport("__Internal")]
        public static extern void StopIosProxy(ushort localPort);
    }
}

