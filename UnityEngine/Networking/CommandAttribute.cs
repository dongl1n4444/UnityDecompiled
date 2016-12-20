namespace UnityEngine.Networking
{
    using System;

    /// <summary>
    /// <para>This is an attribute that can be put on methods of NetworkBehaviour classes to allow them to be invoked on the server by sending a command from a client.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {
        /// <summary>
        /// <para>The QoS channel to use to send this command on, see Networking.QosType.</para>
        /// </summary>
        public int channel = 0;
    }
}

