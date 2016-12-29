namespace UnityEngine.VR
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>Global VR related settings.</para>
    /// </summary>
    public static class VRSettings
    {
        /// <summary>
        /// <para>Loads the requested device at the beginning of the next frame.</para>
        /// </summary>
        /// <param name="deviceName">Name of the device from VRSettings.supportedDevices.</param>
        /// <param name="prioritizedDeviceNameList">Prioritized list of device names from VRSettings.supportedDevices.</param>
        public static void LoadDeviceByName(string deviceName)
        {
            string[] prioritizedDeviceNameList = new string[] { deviceName };
            LoadDeviceByName(prioritizedDeviceNameList);
        }

        /// <summary>
        /// <para>Loads the requested device at the beginning of the next frame.</para>
        /// </summary>
        /// <param name="deviceName">Name of the device from VRSettings.supportedDevices.</param>
        /// <param name="prioritizedDeviceNameList">Prioritized list of device names from VRSettings.supportedDevices.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void LoadDeviceByName(string[] prioritizedDeviceNameList);

        /// <summary>
        /// <para>Globally enables or disables VR for the application.</para>
        /// </summary>
        public static bool enabled { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The current height of an eye texture for the loaded device.</para>
        /// </summary>
        public static int eyeTextureHeight { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The current width of an eye texture for the loaded device.</para>
        /// </summary>
        public static int eyeTextureWidth { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Read-only value that can be used to determine if the VR device is active.</para>
        /// </summary>
        public static bool isDeviceActive { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Type of VR device that is currently in use.</para>
        /// </summary>
        [Obsolete("loadedDevice is deprecated.  Use loadedDeviceName and LoadDeviceByName instead.")]
        public static VRDeviceType loadedDevice
        {
            get
            {
                VRDeviceType unknown = VRDeviceType.Unknown;
                try
                {
                    unknown = (VRDeviceType) Enum.Parse(typeof(VRDeviceType), loadedDeviceName, true);
                }
                catch (Exception)
                {
                }
                return unknown;
            }
            set
            {
                LoadDeviceByName(value.ToString());
            }
        }

        /// <summary>
        /// <para>Type of VR device that is currently loaded.</para>
        /// </summary>
        public static string loadedDeviceName { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Controls the texel:pixel ratio before lens correction, trading performance for sharpness.</para>
        /// </summary>
        public static float renderScale { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Controls the texel:pixel ratio before lens correction, trading performance for sharpness.</para>
        /// </summary>
        public static float renderViewportScale
        {
            get => 
                renderViewportScaleInternal;
            set
            {
                if ((value < 0f) || (value > 1f))
                {
                    throw new ArgumentOutOfRangeException("value", "Render viewport scale should be between 0 and 1.");
                }
                renderViewportScaleInternal = value;
            }
        }

        internal static float renderViewportScaleInternal { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Mirror what is shown on the device to the main display, if possible.</para>
        /// </summary>
        public static bool showDeviceView { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Returns a list of supported VR devices that were included at build time.</para>
        /// </summary>
        public static string[] supportedDevices { [MethodImpl(MethodImplOptions.InternalCall)] get; }
    }
}

