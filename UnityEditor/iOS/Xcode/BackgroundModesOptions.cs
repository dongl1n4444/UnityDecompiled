namespace UnityEditor.iOS.Xcode
{
    using System;

    [Serializable, Flags]
    public enum BackgroundModesOptions
    {
        ActsAsABluetoothLEAccessory = 0x40,
        AudioAirplayPiP = 1,
        BackgroundFetch = 0x80,
        ExternalAccessoryCommunication = 0x10,
        LocationUpdates = 2,
        NewsstandDownloads = 8,
        None = 0,
        RemoteNotifications = 0x100,
        UsesBluetoothLEAccessory = 0x20,
        VoiceOverIP = 4
    }
}

