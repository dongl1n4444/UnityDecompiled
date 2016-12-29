namespace UnityEngine
{
    using System;

    [Obsolete("iPhone class is deprecated. Please use iOS.Device instead (UnityUpgradable) -> UnityEngine.iOS.Device", true)]
    public sealed class iPhone
    {
        public static void ResetNoBackupFlag(string path)
        {
        }

        public static void SetNoBackupFlag(string path)
        {
        }

        public static string advertisingIdentifier =>
            null;

        public static bool advertisingTrackingEnabled =>
            false;

        public static iPhoneGeneration generation =>
            iPhoneGeneration.Unknown;

        public static string vendorIdentifier =>
            null;
    }
}

