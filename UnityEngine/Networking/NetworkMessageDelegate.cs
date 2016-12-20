namespace UnityEngine.Networking
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>The callback delegate used in message handler functions.</para>
    /// </summary>
    /// <param name="netMsg">Network message for the message callback.</param>
    public delegate void NetworkMessageDelegate(NetworkMessage netMsg);
}

