namespace UnityEngine.Networking.Types
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// <para>An Enum representing the priority of a client in a match, starting at 0 and increasing.</para>
    /// </summary>
    [DefaultValue(0x7fffffff)]
    public enum HostPriority
    {
        /// <summary>
        /// <para>The Invalid case for a HostPriority. An Invalid host priority is not a valid host.</para>
        /// </summary>
        Invalid = 0x7fffffff
    }
}

