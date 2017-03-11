namespace UnityEngine.iOS
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    /// <summary>
    /// <para>Interface into iOS specific functionality.</para>
    /// </summary>
    public sealed class Device
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern string GetAdvertisingIdentifier();
        /// <summary>
        /// <para>Reset "no backup" file flag: file will be synced with iCloud/iTunes backup and can be deleted by OS in low storage situations.</para>
        /// </summary>
        /// <param name="path"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern void ResetNoBackupFlag(string path);
        /// <summary>
        /// <para>Set file flag to be excluded from iCloud/iTunes backup.</para>
        /// </summary>
        /// <param name="path"></param>
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public static extern void SetNoBackupFlag(string path);

        /// <summary>
        /// <para>Advertising ID.</para>
        /// </summary>
        public static string advertisingIdentifier
        {
            get
            {
                string advertisingIdentifier = GetAdvertisingIdentifier();
                Application.InvokeOnAdvertisingIdentifierCallback(advertisingIdentifier, advertisingTrackingEnabled);
                return advertisingIdentifier;
            }
        }

        /// <summary>
        /// <para>Is advertising tracking enabled.</para>
        /// </summary>
        public static bool advertisingTrackingEnabled { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The generation of the device. (Read Only)</para>
        /// </summary>
        public static DeviceGeneration generation { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>iOS version.</para>
        /// </summary>
        public static string systemVersion { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Vendor ID.</para>
        /// </summary>
        public static string vendorIdentifier { [MethodImpl(MethodImplOptions.InternalCall)] get; }
    }
}

