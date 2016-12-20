namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>Parent class for all joints that have anchor points.</para>
    /// </summary>
    public class AnchoredJoint2D : Joint2D
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_anchor(out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_connectedAnchor(out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_anchor(ref Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_connectedAnchor(ref Vector2 value);

        /// <summary>
        /// <para>The joint's anchor point on the object that has the joint component.</para>
        /// </summary>
        public Vector2 anchor
        {
            get
            {
                Vector2 vector;
                this.INTERNAL_get_anchor(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_anchor(ref value);
            }
        }

        /// <summary>
        /// <para>Should the connectedAnchor be calculated automatically?</para>
        /// </summary>
        public bool autoConfigureConnectedAnchor { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The joint's anchor point on the second object (ie, the one which doesn't have the joint component).</para>
        /// </summary>
        public Vector2 connectedAnchor
        {
            get
            {
                Vector2 vector;
                this.INTERNAL_get_connectedAnchor(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_connectedAnchor(ref value);
            }
        }
    }
}

