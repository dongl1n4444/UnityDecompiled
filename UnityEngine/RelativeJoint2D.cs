namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Keeps two Rigidbody2D at their relative orientations.</para>
    /// </summary>
    public sealed class RelativeJoint2D : Joint2D
    {
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_linearOffset(out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_target(out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_linearOffset(ref Vector2 value);

        /// <summary>
        /// <para>The current angular offset between the Rigidbody2D that the joint connects.</para>
        /// </summary>
        public float angularOffset { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Should both the linearOffset and angularOffset be calculated automatically?</para>
        /// </summary>
        public bool autoConfigureOffset { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Scales both the linear and angular forces used to correct the required relative orientation.</para>
        /// </summary>
        public float correctionScale { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The current linear offset between the Rigidbody2D that the joint connects.</para>
        /// </summary>
        public Vector2 linearOffset
        {
            get
            {
                Vector2 vector;
                this.INTERNAL_get_linearOffset(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_linearOffset(ref value);
            }
        }

        /// <summary>
        /// <para>The maximum force that can be generated when trying to maintain the relative joint constraint.</para>
        /// </summary>
        public float maxForce { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The maximum torque that can be generated when trying to maintain the relative joint constraint.</para>
        /// </summary>
        public float maxTorque { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The world-space position that is currently trying to be maintained.</para>
        /// </summary>
        public Vector2 target
        {
            get
            {
                Vector2 vector;
                this.INTERNAL_get_target(out vector);
                return vector;
            }
        }
    }
}

