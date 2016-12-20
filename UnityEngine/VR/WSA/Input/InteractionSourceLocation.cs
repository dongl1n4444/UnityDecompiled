namespace UnityEngine.VR.WSA.Input
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Represents the position and velocity of a hand or controller.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public struct InteractionSourceLocation
    {
        internal byte m_hasPosition;
        internal Vector3 m_position;
        internal byte m_hasVelocity;
        internal Vector3 m_velocity;
        public bool TryGetPosition(out Vector3 position)
        {
            position = this.m_position;
            return (this.m_hasPosition != 0);
        }

        public bool TryGetVelocity(out Vector3 velocity)
        {
            velocity = this.m_velocity;
            return (this.m_hasVelocity != 0);
        }
    }
}

