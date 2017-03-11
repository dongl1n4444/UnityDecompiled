namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Collision details returned by 2D physics callback functions.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public class Collision2D
    {
        internal int m_Collider;
        internal int m_OtherCollider;
        internal int m_Rigidbody;
        internal int m_OtherRigidbody;
        internal ContactPoint2D[] m_Contacts;
        internal Vector2 m_RelativeVelocity;
        internal int m_Enabled;
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
        /// <para>The Transform of the incoming object involved in the collision.</para>
        /// </summary>
        public Transform transform =>
            ((this.rigidbody == null) ? this.collider.transform : this.rigidbody.transform);
        /// <summary>
        /// <para>The incoming GameObject involved in the collision.</para>
        /// </summary>
        public GameObject gameObject =>
            ((this.rigidbody == null) ? this.collider.gameObject : this.rigidbody.gameObject);
        /// <summary>
        /// <para>The specific points of contact with the incoming Collider2D.</para>
        /// </summary>
        public ContactPoint2D[] contacts =>
            this.m_Contacts;
        /// <summary>
        /// <para>The relative linear velocity of the two colliding objects (Read Only).</para>
        /// </summary>
        public Vector2 relativeVelocity =>
            this.m_RelativeVelocity;
        /// <summary>
        /// <para>Indicates whether the collision response or reaction is enabled or disabled.</para>
        /// </summary>
        public bool enabled =>
            (this.m_Enabled == 1);
    }
}

