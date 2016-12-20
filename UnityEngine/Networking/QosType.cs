namespace UnityEngine.Networking
{
    using System;

    /// <summary>
    /// <para>Enumeration of all supported quality of service channel modes.</para>
    /// </summary>
    public enum QosType
    {
        Unreliable,
        UnreliableFragmented,
        UnreliableSequenced,
        Reliable,
        ReliableFragmented,
        ReliableSequenced,
        StateUpdate,
        ReliableStateUpdate,
        AllCostDelivery
    }
}

