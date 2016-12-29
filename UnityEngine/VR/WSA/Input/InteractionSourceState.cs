namespace UnityEngine.VR.WSA.Input
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Represents a snapshot of the state of a spatial interaction source (hand, voice or controller) at a given time.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public struct InteractionSourceState
    {
        internal byte m_pressed;
        internal InteractionSourceProperties m_properties;
        internal InteractionSource m_source;
        internal Ray m_headRay;
        /// <summary>
        /// <para>True if the source is in the pressed state.</para>
        /// </summary>
        public bool pressed =>
            (this.m_pressed != 0);
        /// <summary>
        /// <para>Additional properties to explore the state of the interaction source.</para>
        /// </summary>
        public InteractionSourceProperties properties =>
            this.m_properties;
        /// <summary>
        /// <para>The interaction source that this state describes.</para>
        /// </summary>
        public InteractionSource source =>
            this.m_source;
        /// <summary>
        /// <para>The Ray at the time represented by this InteractionSourceState.</para>
        /// </summary>
        public Ray headRay =>
            this.m_headRay;
    }
}

