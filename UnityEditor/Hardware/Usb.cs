namespace UnityEditor.Hardware
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public sealed class Usb
    {
        [field: DebuggerBrowsable(0), CompilerGenerated]
        public static  event OnDevicesChangedHandler DevicesChanged;

        public static void OnDevicesChanged(UsbDevice[] devices)
        {
            if ((DevicesChanged != null) && (devices != null))
            {
                DevicesChanged(devices);
            }
        }

        public delegate void OnDevicesChangedHandler(UsbDevice[] devices);
    }
}

