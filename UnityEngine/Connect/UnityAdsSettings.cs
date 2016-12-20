namespace UnityEngine.Connect
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class UnityAdsSettings
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string GetGameId(RuntimePlatform platform);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool IsPlatformEnabled(RuntimePlatform platform);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void SetGameId(RuntimePlatform platform, string gameId);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void SetPlatformEnabled(RuntimePlatform platform, bool value);

        [ThreadAndSerializationSafe]
        public static bool enabled { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        public static bool initializeOnStartup { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        public static bool testMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

