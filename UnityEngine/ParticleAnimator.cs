namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>(Legacy Particles) Particle animators move your particles over time, you use them to apply wind, drag &amp; color cycling to your particle emitters.</para>
    /// </summary>
    [Obsolete("This component is part of the legacy particle system, which is deprecated and will be removed in a future release. Use the ParticleSystem component instead.", false), RequireComponent(typeof(Transform))]
    public sealed class ParticleAnimator : Component
    {
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_force(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_localRotationAxis(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_rndForce(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_worldRotationAxis(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_force(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_localRotationAxis(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_rndForce(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_worldRotationAxis(ref Vector3 value);

        /// <summary>
        /// <para>Does the GameObject of this particle animator auto destructs?</para>
        /// </summary>
        public bool autodestruct { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Colors the particles will cycle through over their lifetime.</para>
        /// </summary>
        public Color[] colorAnimation { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>How much particles are slowed down every frame.</para>
        /// </summary>
        public float damping { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Do particles cycle their color over their lifetime?</para>
        /// </summary>
        public bool doesAnimateColor { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The force being applied to particles every frame.</para>
        /// </summary>
        public Vector3 force
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_force(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_force(ref value);
            }
        }

        /// <summary>
        /// <para>Local space axis the particles rotate around.</para>
        /// </summary>
        public Vector3 localRotationAxis
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_localRotationAxis(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_localRotationAxis(ref value);
            }
        }

        /// <summary>
        /// <para>A random force added to particles every frame.</para>
        /// </summary>
        public Vector3 rndForce
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_rndForce(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_rndForce(ref value);
            }
        }

        /// <summary>
        /// <para>How the particle sizes grow over their lifetime.</para>
        /// </summary>
        public float sizeGrow { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>World space axis the particles rotate around.</para>
        /// </summary>
        public Vector3 worldRotationAxis
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_worldRotationAxis(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_worldRotationAxis(ref value);
            }
        }
    }
}

