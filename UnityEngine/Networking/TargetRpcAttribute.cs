namespace UnityEngine.Networking
{
    using System;

    /// <summary>
    /// <para>This is an attribute that can be put on methods of NetworkBehaviour classes to allow them to be invoked on clients from a server. Unlike the ClientRpc attribute, these functions are invoked on one individual target client, not all of the ready clients.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class TargetRpcAttribute : Attribute
    {
        /// <summary>
        /// <para>The channel ID which this RPC transmission will use.</para>
        /// </summary>
        public int channel = 0;
    }
}

