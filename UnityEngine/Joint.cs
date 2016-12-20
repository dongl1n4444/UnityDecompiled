namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>Joint is the base class for all joints.</para>
    /// </summary>
    public class Joint : Component
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_anchor(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_axis(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_connectedAnchor(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_currentForce(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_currentTorque(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_anchor(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_axis(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_connectedAnchor(ref Vector3 value);

        /// <summary>
        /// <para>The Position of the anchor around which the joints motion is constrained.</para>
        /// </summary>
        public Vector3 anchor
        {
            get
            {
                Vector3 vector;
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
        /// <para>The Direction of the axis around which the body is constrained.</para>
        /// </summary>
        public Vector3 axis
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_axis(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_axis(ref value);
            }
        }

        /// <summary>
        /// <para>The force that needs to be applied for this joint to break.</para>
        /// </summary>
        public float breakForce { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The torque that needs to be applied for this joint to break.</para>
        /// </summary>
        public float breakTorque { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Position of the anchor relative to the connected Rigidbody.</para>
        /// </summary>
        public Vector3 connectedAnchor
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_connectedAnchor(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_connectedAnchor(ref value);
            }
        }

        /// <summary>
        /// <para>A reference to another rigidbody this joint connects to.</para>
        /// </summary>
        public Rigidbody connectedBody { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The force applied by the solver to satisfy all constraints.</para>
        /// </summary>
        public Vector3 currentForce
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_currentForce(out vector);
                return vector;
            }
        }

        /// <summary>
        /// <para>The torque applied by the solver to satisfy all constraints.</para>
        /// </summary>
        public Vector3 currentTorque
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_currentTorque(out vector);
                return vector;
            }
        }

        /// <summary>
        /// <para>Enable collision between bodies connected with the joint.</para>
        /// </summary>
        public bool enableCollision { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Toggle preprocessing for this joint.</para>
        /// </summary>
        public bool enablePreprocessing { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

