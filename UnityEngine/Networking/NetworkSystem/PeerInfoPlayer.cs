namespace UnityEngine.Networking.NetworkSystem
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Networking;

    /// <summary>
    /// <para>A structure used to identify player object on other peers for host migration.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct PeerInfoPlayer
    {
        /// <summary>
        /// <para>The networkId of the player object.</para>
        /// </summary>
        public NetworkInstanceId netId;
        /// <summary>
        /// <para>The playerControllerId of the player GameObject.</para>
        /// </summary>
        public short playerControllerId;
    }
}

