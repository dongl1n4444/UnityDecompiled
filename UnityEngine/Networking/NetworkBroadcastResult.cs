namespace UnityEngine.Networking
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>A structure that contains data from a NetworkDiscovery server broadcast.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NetworkBroadcastResult
    {
        /// <summary>
        /// <para>The IP address of the server that broadcasts this data.</para>
        /// </summary>
        public string serverAddress;
        /// <summary>
        /// <para>The data broadcast by the server.</para>
        /// </summary>
        public byte[] broadcastData;
    }
}

