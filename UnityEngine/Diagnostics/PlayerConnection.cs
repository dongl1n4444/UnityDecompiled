namespace UnityEngine.Diagnostics
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Networking.PlayerConnection;
    using UnityEngine.Scripting;

    public static class PlayerConnection
    {
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("PlayerConnection.SendFile is no longer supported.", true), GeneratedByOldBindingsGenerator]
        public static extern void SendFile(string remoteFilePath, byte[] data);

        [Obsolete("Use UnityEngine.Networking.PlayerConnection.PlayerConnection.instance.isConnected instead.")]
        public static bool connected =>
            UnityEngine.Networking.PlayerConnection.PlayerConnection.instance.isConnected;
    }
}

