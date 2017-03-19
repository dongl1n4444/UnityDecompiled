﻿namespace UnityEditor.Hardware
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using UnityEngine.Scripting;

    public sealed class DevDeviceList
    {
        [field: DebuggerBrowsable(0), CompilerGenerated]
        public static  event OnChangedHandler Changed;

        public static bool FindDevice(string deviceId, out DevDevice device)
        {
            foreach (DevDevice device2 in GetDevices())
            {
                if (device2.id == deviceId)
                {
                    device = device2;
                    return true;
                }
            }
            device = new DevDevice();
            return false;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern DevDevice[] GetDevices();
        public static void OnChanged()
        {
            if (Changed != null)
            {
                Changed();
            }
        }

        internal static void Update(string target, DevDevice[] devices)
        {
            UpdateInternal(target, devices);
            OnChanged();
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void UpdateInternal(string target, DevDevice[] devices);

        public delegate void OnChangedHandler();
    }
}

