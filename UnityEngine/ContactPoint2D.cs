namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Details about a specific point of contact involved in a 2D physics collision.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct ContactPoint2D
    {
        internal Vector2 m_Point;
        internal Vector2 m_Normal;
        internal Vector2 m_RelativeVelocity;
        internal float m_Separation;
        internal float m_NormalImpulse;
        internal float m_TangentImpulse;
        internal int m_Collider;
        internal int m_OtherCollider;
        internal int m_Rigidbody;
        internal int m_OtherRigidbody;
        internal int m_Enabled;
        /// <summary>
        /// <para>The point of contact between the two colliders in world space.</para>
        /// </summary>
        public Vector2 point =>
            this.m_Point;
        /// <summary>
        /// <para>Surface normal at the contact point.</para>
        /// </summary>
        public Vector2 normal =>
            this.m_Normal;
        /// <summary>
        /// <para>Gets the distance between the colliders at the contact point.</para>
        /// </summary>
        public float separation =>
            this.m_Separation;
        /// <summary>
        /// <para>Gets the impulse force applied at the contact point along the ContactPoint2D.normal.</para>
        /// </summary>
        public float normalImpulse =>
            this.m_NormalImpulse;
        /// <summary>
        /// <para>Gets the impulse force applied at the contact point which is perpendicular to the ContactPoint2D.normal.</para>
        /// </summary>
        public float tangentImpulse =>
            this.m_TangentImpulse;
        /// <summary>
        /// <para>Gets the relative velocity of the two colliders at the contact point (Read Only).</para>
        /// </summary>
        public Vector2 relativeVelocity =>
            this.m_RelativeVelocity;
        /// <summary>
        /// <para>The incoming Collider2D involved in the collision with the otherCollider.</para>
        /// </summary>
        public Collider2D collider =>
            Physics2D.GetColliderFromInstanceID(this.m_Collider);
        /// <summary>
        /// <para>The other Collider2D involved in the collision with the collider.</para>
        /// </summary>
        public Collider2D otherCollider =>
            Physics2D.GetColliderFromInstanceID(this.m_OtherCollider);
        /// <summary>
        /// <para>The incoming Rigidbody2D involved in the collision with the otherRigidbody.</para>
        /// </summary>
        public Rigidbody2D rigidbody =>
            Physics2D.GetRigidbodyFromInstanceID(this.m_Rigidbody);
        /// <summary>
        /// <para>The other Rigidbody2D involved in the collision with the rigidbody.</para>
        /// </summary>
        public Rigidbody2D otherRigidbody =>
            Physics2D.GetRigidbodyFromInstanceID(this.m_OtherRigidbody);
        /// <summary>
        /// <para>Indicates whether the collision response or reaction is enabled or disabled.</para>
        /// </summary>
        public bool enabled =>
            (this.m_Enabled == 1);
    }
}

