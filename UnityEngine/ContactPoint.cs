namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Describes a contact point where the collision occurs.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct ContactPoint
    {
        internal Vector3 m_Point;
        internal Vector3 m_Normal;
        internal int m_ThisColliderInstanceID;
        internal int m_OtherColliderInstanceID;
        internal float m_Separation;
        /// <summary>
        /// <para>The point of contact.</para>
        /// </summary>
        public Vector3 point =>
            this.m_Point;
        /// <summary>
        /// <para>Normal of the contact point.</para>
        /// </summary>
        public Vector3 normal =>
            this.m_Normal;
        /// <summary>
        /// <para>The first collider in contact at the point.</para>
        /// </summary>
        public Collider thisCollider =>
            ColliderFromInstanceId(this.m_ThisColliderInstanceID);
        /// <summary>
        /// <para>The other collider in contact at the point.</para>
        /// </summary>
        public Collider otherCollider =>
            ColliderFromInstanceId(this.m_OtherColliderInstanceID);
        /// <summary>
        /// <para>The distance between the colliders at the contact point.</para>
        /// </summary>
        public float separation =>
            this.m_Separation;
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Collider ColliderFromInstanceId(int instanceID);
    }
}

