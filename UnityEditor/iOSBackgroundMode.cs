namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>Background modes supported by the application corresponding to project settings in Xcode.</para>
    /// </summary>
    [Flags]
    public enum iOSBackgroundMode : uint
    {
        /// <summary>
        /// <para>Audio, AirPlay and Picture in Picture.</para>
        /// </summary>
        Audio = 1,
        /// <summary>
        /// <para>Uses Bluetooth LE accessories.</para>
        /// </summary>
        BluetoothCentral = 0x20,
        /// <summary>
        /// <para>Acts as a Bluetooth LE accessory.</para>
        /// </summary>
        BluetoothPeripheral = 0x40,
        /// <summary>
        /// <para>External accessory communication.</para>
        /// </summary>
        ExternalAccessory = 0x10,
        /// <summary>
        /// <para>Background fetch.</para>
        /// </summary>
        Fetch = 0x80,
        /// <summary>
        /// <para>Location updates.</para>
        /// </summary>
        Location = 2,
        /// <summary>
        /// <para>Newsstand downloads.</para>
        /// </summary>
        NewsstandContent = 8,
        /// <summary>
        /// <para>No background modes supported.</para>
        /// </summary>
        None = 0,
        /// <summary>
        /// <para>Remote notifications.</para>
        /// </summary>
        RemoteNotification = 0x100,
        /// <summary>
        /// <para>Voice over IP.</para>
        /// </summary>
        VOIP = 4
    }
}

