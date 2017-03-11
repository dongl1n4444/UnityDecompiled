namespace UnityEngine.Playables
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// <para>Struct that holds information regarding an output of a PlayableAsset.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size=1)]
    public struct PlayableBinding
    {
        /// <summary>
        /// <para>A constant to represent a PlayableAsset has no bindings.</para>
        /// </summary>
        public static readonly PlayableBinding[] None;
        /// <summary>
        /// <para>The default duration used when a PlayableOutput has no fixed duration.</para>
        /// </summary>
        public static readonly double DefaultDuration;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <streamName>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private DataStreamType <streamType>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private UnityEngine.Object <sourceObject>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private System.Type <sourceBindingType>k__BackingField;
        /// <summary>
        /// <para>The name of the output or input stream.</para>
        /// </summary>
        public string streamName { get; set; }
        /// <summary>
        /// <para>The type of the output or input stream.</para>
        /// </summary>
        public DataStreamType streamType { get; set; }
        /// <summary>
        /// <para>A reference to a UnityEngine.Object that acts a key for this binding.</para>
        /// </summary>
        public UnityEngine.Object sourceObject { get; set; }
        /// <summary>
        /// <para>When the StreamType is set to None, a binding can be represented using System.Type.</para>
        /// </summary>
        public System.Type sourceBindingType { get; set; }
        static PlayableBinding()
        {
            None = new PlayableBinding[0];
            DefaultDuration = double.PositiveInfinity;
        }
    }
}

