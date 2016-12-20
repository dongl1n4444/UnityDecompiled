namespace UnityEngine.Networking
{
    using System;

    /// <summary>
    /// <para>An enumeration of the options that can be set on a network channel.</para>
    /// </summary>
    public enum ChannelOption
    {
        /// <summary>
        /// <para>The option to allow packet fragmentation for a channel.</para>
        /// </summary>
        AllowFragmentation = 2,
        /// <summary>
        /// <para>The option to set the maximum packet size allowed for a channel.</para>
        /// </summary>
        MaxPacketSize = 3,
        /// <summary>
        /// <para>The option to set the number of pending buffers for a channel.</para>
        /// </summary>
        MaxPendingBuffers = 1
    }
}

