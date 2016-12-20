namespace UnityEngine
{
    using System;

    /// <summary>
    /// <para>Describes the status of the network interface peer type as returned by Network.peerType.</para>
    /// </summary>
    public enum NetworkPeerType
    {
        Disconnected,
        Server,
        Client,
        Connecting
    }
}

