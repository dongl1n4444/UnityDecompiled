namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>Applies both linear and angular (torque) forces continuously to the rigidbody each physics update.</para>
    /// </summary>
    public sealed class ConstantForce2D : PhysicsUpdateBehaviour2D
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_force(out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_relativeForce(out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_force(ref Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_relativeForce(ref Vector2 value);

        /// <summary>
        /// <para>The linear force applied to the rigidbody each physics update.</para>
        /// </summary>
        public Vector2 force
        {
            get
            {
                Vector2 vector;
                this.INTERNAL_get_force(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_force(ref value);
            }
        }

        /// <summary>
        /// <para>The linear force, relative to the rigid-body coordinate system, applied each physics update.</para>
        /// </summary>
        public Vector2 relativeForce
        {
            get
            {
                Vector2 vector;
                this.INTERNAL_get_relativeForce(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_relativeForce(ref value);
            }
        }

        /// <summary>
        /// <para>The torque applied to the rigidbody each physics update.</para>
        /// </summary>
        public float torque { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

