namespace UnityEngine.Networking
{
    using System;

    /// <summary>
    /// <para>This is an attribute that can be put on methods of NetworkBehaviour classes to allow them to be invoked on clients from a server.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ClientRpcAttribute : Attribute
    {
        /// <summary>
        /// <para>The channel ID which this RPC transmission will use.</para>
        /// </summary>
        public int channel = 0;
    }
}

