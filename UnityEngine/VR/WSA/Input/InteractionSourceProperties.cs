namespace UnityEngine.VR.WSA.Input
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Represents the set of properties available to explore the current state of a hand or controller.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public struct InteractionSourceProperties
    {
        internal double m_sourceLossRisk;
        internal Vector3 m_sourceLossMitigationDirection;
        internal InteractionSourceLocation m_location;
        /// <summary>
        /// <para>Gets the risk that detection of the hand will be lost as a value from 0.0 to 1.0.</para>
        /// </summary>
        public double sourceLossRisk =>
            this.m_sourceLossRisk;
        /// <summary>
        /// <para>The direction you should suggest that the user move their hand if it is nearing the edge of the detection area.</para>
        /// </summary>
        public Vector3 sourceLossMitigationDirection =>
            this.m_sourceLossMitigationDirection;
        /// <summary>
        /// <para>The position and velocity of the hand, expressed in the specified coordinate system.</para>
        /// </summary>
        public InteractionSourceLocation location =>
            this.m_location;
    }
}

