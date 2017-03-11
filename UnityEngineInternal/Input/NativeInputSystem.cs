namespace UnityEngineInternal.Input
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    public sealed class NativeInputSystem
    {
        public static NativeDeviceDiscoveredCallback onDeviceDiscovered;
        public static NativeEventCallback onEvents;
        public static NativeUpdateCallback onUpdate;

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern string GetControlConfiguration(int deviceId, int controlIndex);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern string GetDeviceConfiguration(int deviceId);
        [RequiredByNativeCode]
        internal static void NotifyDeviceDiscovered(NativeInputDeviceInfo deviceInfo)
        {
            NativeDeviceDiscoveredCallback onDeviceDiscovered = NativeInputSystem.onDeviceDiscovered;
            if (onDeviceDiscovered != null)
            {
                onDeviceDiscovered(deviceInfo);
            }
        }

        [RequiredByNativeCode]
        internal static void NotifyEvents(int eventCount, IntPtr eventData)
        {
            NativeEventCallback onEvents = NativeInputSystem.onEvents;
            if (onEvents != null)
            {
                onEvents(eventCount, eventData);
            }
        }

        [RequiredByNativeCode]
        internal static void NotifyUpdate(NativeInputUpdateType updateType)
        {
            NativeUpdateCallback onUpdate = NativeInputSystem.onUpdate;
            if (onUpdate != null)
            {
                onUpdate(updateType);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, GeneratedByOldBindingsGenerator]
        public static extern void SendInput(ref NativeInputEvent inputEvent);
        public static void SetPollingFrequency(float hertz)
        {
            if (hertz < 1f)
            {
                throw new ArgumentException("Polling frequency cannot be less than 1Hz");
            }
            SetPollingFrequencyInternal(hertz);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void SetPollingFrequencyInternal(float hertz);

        internal static bool enabled { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        public static double zeroEventTime { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
    }
}

