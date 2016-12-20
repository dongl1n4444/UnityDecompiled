namespace UnityEngine
{
    using System;

    /// <summary>
    /// <para>Different types of synchronization for the NetworkView component.</para>
    /// </summary>
    public enum NetworkStateSynchronization
    {
        Off,
        ReliableDeltaCompressed,
        Unreliable
    }
}

