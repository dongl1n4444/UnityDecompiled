namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>The Cloth class provides an interface to cloth simulation physics.</para>
    /// </summary>
    [RequireComponent(typeof(Transform), typeof(SkinnedMeshRenderer)), NativeClass("Unity::Cloth")]
    public sealed class Cloth : Component
    {
        /// <summary>
        /// <para>Clear the pending transform changes from affecting the cloth simulation.</para>
        /// </summary>
        public void ClearTransformMotion()
        {
            INTERNAL_CALL_ClearTransformMotion(this);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_ClearTransformMotion(Cloth self);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_externalAcceleration(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_randomAcceleration(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_externalAcceleration(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_randomAcceleration(ref Vector3 value);
        [ExcludeFromDocs]
        public void SetEnabledFading(bool enabled)
        {
            float interpolationTime = 0.5f;
            this.SetEnabledFading(enabled, interpolationTime);
        }

        /// <summary>
        /// <para>Fade the cloth simulation in or out.</para>
        /// </summary>
        /// <param name="enabled">Fading enabled or not.</param>
        /// <param name="interpolationTime"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void SetEnabledFading(bool enabled, [DefaultValue("0.5f")] float interpolationTime);

        /// <summary>
        /// <para>Bending stiffness of the cloth.</para>
        /// </summary>
        public float bendingStiffness { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>An array of CapsuleColliders which this Cloth instance should collide with.</para>
        /// </summary>
        public CapsuleCollider[] capsuleColliders { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The cloth skinning coefficients used to set up how the cloth interacts with the skinned mesh.</para>
        /// </summary>
        public ClothSkinningCoefficient[] coefficients { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>How much to increase mass of colliding particles.</para>
        /// </summary>
        public float collisionMassScale { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Damp cloth motion.</para>
        /// </summary>
        public float damping { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Is this cloth enabled?</para>
        /// </summary>
        public bool enabled { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>A constant, external acceleration applied to the cloth.</para>
        /// </summary>
        public Vector3 externalAcceleration
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_externalAcceleration(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_externalAcceleration(ref value);
            }
        }

        /// <summary>
        /// <para>The friction of the cloth when colliding with the character.</para>
        /// </summary>
        public float friction { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The current normals of the cloth object.</para>
        /// </summary>
        public Vector3[] normals { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>A random, external acceleration applied to the cloth.</para>
        /// </summary>
        public Vector3 randomAcceleration
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_randomAcceleration(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_randomAcceleration(ref value);
            }
        }

        [Obsolete("Deprecated. Cloth.selfCollisions is no longer supported since Unity 5.0.", true)]
        public bool selfCollision { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Cloth's sleep threshold.</para>
        /// </summary>
        public float sleepThreshold { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        public bool solverFrequency { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>An array of ClothSphereColliderPairs which this Cloth instance should collide with.</para>
        /// </summary>
        public ClothSphereColliderPair[] sphereColliders { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Stretching stiffness of the cloth.</para>
        /// </summary>
        public float stretchingStiffness { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        public float useContinuousCollision { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Should gravity affect the cloth simulation?</para>
        /// </summary>
        public bool useGravity { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Add one virtual particle per triangle to improve collision stability.</para>
        /// </summary>
        public float useVirtualParticles { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The current vertex positions of the cloth object.</para>
        /// </summary>
        public Vector3[] vertices { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>How much world-space acceleration of the character will affect cloth vertices.</para>
        /// </summary>
        public float worldAccelerationScale { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>How much world-space movement of the character will affect cloth vertices.</para>
        /// </summary>
        public float worldVelocityScale { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

