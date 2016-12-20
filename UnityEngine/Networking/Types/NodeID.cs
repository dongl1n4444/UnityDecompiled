﻿namespace UnityEngine.Networking.Types
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// <para>The NodeID is the ID used in Relay matches to track nodes in a network.</para>
    /// </summary>
    [DefaultValue(0)]
    public enum NodeID : ushort
    {
        /// <summary>
        /// <para>The invalid case of a NodeID.</para>
        /// </summary>
        Invalid = 0
    }
}

