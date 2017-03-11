namespace UnityEngine.VR
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Contains all functionality related to a VR device.</para>
    /// </summary>
    public static class VRDevice
    {
        /// <summary>
        /// <para>This method returns an IntPtr representing the native pointer to the VR device if one is available, otherwise the value will be IntPtr.Zero.</para>
        /// </summary>
        /// <returns>
        /// <para>The native pointer to the VR device.</para>
        /// </returns>
        public static IntPtr GetNativePtr()
        {
            IntPtr ptr;
            INTERNAL_CALL_GetNativePtr(out ptr);
            return ptr;
        }

        /// <summary>
        /// <para>Returns the device's current TrackingSpaceType. This value determines how the camera is positioned relative to its starting position. For more, see the section "Understanding the camera" in.</para>
        /// </summary>
        /// <returns>
        /// <para>The device's current TrackingSpaceType.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern TrackingSpaceType GetTrackingSpaceType();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetNativePtr(out IntPtr value);
        /// <summary>
        /// <para>Sets the device's current TrackingSpaceType. Returns true on success. Returns false if the given TrackingSpaceType is not supported or the device fails to switch.</para>
        /// </summary>
        /// <param name="TrackingSpaceType">The TrackingSpaceType the device should switch to.</param>
        /// <param name="trackingSpaceType"></param>
        /// <returns>
        /// <para>True on success. False if the given TrackingSpaceType is not supported or the device fails to switch.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool SetTrackingSpaceType(TrackingSpaceType trackingSpaceType);

        /// <summary>
        /// <para>The name of the family of the loaded VR device.</para>
        /// </summary>
        [Obsolete("family is deprecated.  Use VRSettings.loadedDeviceName instead.")]
        public static string family { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Successfully detected a VR device in working order.</para>
        /// </summary>
        public static bool isPresent { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Specific model of loaded VR device.</para>
        /// </summary>
        public static string model { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Refresh rate of the display in Hertz.</para>
        /// </summary>
        public static float refreshRate { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
    }
}

