namespace UnityEngine.VR
{
    using System;

    /// <summary>
    /// <para>Supported VR devices.</para>
    /// </summary>
    [Obsolete("VRDeviceType is deprecated. Use VRSettings.supportedDevices instead.")]
    public enum VRDeviceType
    {
        /// <summary>
        /// <para>Sony's Project Morpheus VR device for Playstation 4. (Obsolete please use VRDeviceType.PlayStationVR instead).</para>
        /// </summary>
        [Obsolete("Enum member VRDeviceType.Morpheus has been deprecated. Use VRDeviceType.PlayStationVR instead (UnityUpgradable) -> PlayStationVR", true)]
        Morpheus = -1,
        /// <summary>
        /// <para>No VR Device.</para>
        /// </summary>
        None = 0,
        /// <summary>
        /// <para>Oculus family of VR devices.</para>
        /// </summary>
        Oculus = 3,
        /// <summary>
        /// <para>Sony's PlayStation VR device for Playstation 4 (formerly called Project Morpheus VR).Sony's PlayStation VR device for Playstation 4 (formerly called Project Morpheus VR).</para>
        /// </summary>
        PlayStationVR = 4,
        /// <summary>
        /// <para>Split screen stereo 3D (the left and right cameras are rendered side by side).</para>
        /// </summary>
        Split = 2,
        /// <summary>
        /// <para>Stereo 3D via D3D11 or OpenGL.</para>
        /// </summary>
        Stereo = 1,
        /// <summary>
        /// <para>This value is returned when running on a device that does not have its own value in this VRDeviceType enum.  To find out the device name, you can use VRSettings.loadedDeviceName.</para>
        /// </summary>
        Unknown = 5
    }
}

