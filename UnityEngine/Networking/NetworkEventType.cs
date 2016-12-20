namespace UnityEngine.Networking
{
    using System;

    /// <summary>
    /// <para>Event that is returned when calling the Networking.NetworkTransport.Receive and Networking.NetworkTransport.ReceiveFromHost functions.</para>
    /// </summary>
    public enum NetworkEventType
    {
        DataEvent,
        ConnectEvent,
        DisconnectEvent,
        Nothing,
        BroadcastEvent
    }
}

