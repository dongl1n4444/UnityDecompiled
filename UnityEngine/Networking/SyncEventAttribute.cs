namespace UnityEngine.Networking
{
    using System;

    /// <summary>
    /// <para>This is an attribute that can be put on events in NetworkBehaviour classes to allow them to be invoked on client when the event is called on the sserver.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Event)]
    public class SyncEventAttribute : Attribute
    {
        /// <summary>
        /// <para>The UNET QoS channel that this event should be sent on.</para>
        /// </summary>
        public int channel = 0;
    }
}

