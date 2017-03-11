namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>(Legacy Particles) Script interface for particle emitters.</para>
    /// </summary>
    [Obsolete("This component is part of the legacy particle system, which is deprecated and will be removed in a future release. Use the ParticleSystem component instead.", false)]
    public class ParticleEmitter : Component
    {
        internal ParticleEmitter()
        {
        }

        /// <summary>
        /// <para>Removes all particles from the particle emitter.</para>
        /// </summary>
        public void ClearParticles()
        {
            INTERNAL_CALL_ClearParticles(this);
        }

        /// <summary>
        /// <para>Emit a number of particles.</para>
        /// </summary>
        public void Emit()
        {
            this.Emit2((int) UnityEngine.Random.Range(this.minEmission, this.maxEmission));
        }

        /// <summary>
        /// <para>Emit count particles immediately.</para>
        /// </summary>
        /// <param name="count"></param>
        public void Emit(int count)
        {
            this.Emit2(count);
        }

        /// <summary>
        /// <para>Emit a single particle with given parameters.</para>
        /// </summary>
        /// <param name="pos">The position of the particle.</param>
        /// <param name="velocity">The velocity of the particle.</param>
        /// <param name="size">The size of the particle.</param>
        /// <param name="energy">The remaining lifetime of the particle.</param>
        /// <param name="color">The color of the particle.</param>
        public void Emit(Vector3 pos, Vector3 velocity, float size, float energy, Color color)
        {
            InternalEmitParticleArguments args = new InternalEmitParticleArguments {
                pos = pos,
                velocity = velocity,
                size = size,
                energy = energy,
                color = color,
                rotation = 0f,
                angularVelocity = 0f
            };
            this.Emit3(ref args);
        }

        /// <summary>
        /// <para></para>
        /// </summary>
        /// <param name="rotation">The initial rotation of the particle in degrees.</param>
        /// <param name="angularVelocity">The angular velocity of the particle in degrees per second.</param>
        /// <param name="pos"></param>
        /// <param name="velocity"></param>
        /// <param name="size"></param>
        /// <param name="energy"></param>
        /// <param name="color"></param>
        public void Emit(Vector3 pos, Vector3 velocity, float size, float energy, Color color, float rotation, float angularVelocity)
        {
            InternalEmitParticleArguments args = new InternalEmitParticleArguments {
                pos = pos,
                velocity = velocity,
                size = size,
                energy = energy,
                color = color,
                rotation = rotation,
                angularVelocity = angularVelocity
            };
            this.Emit3(ref args);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void Emit2(int count);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void Emit3(ref InternalEmitParticleArguments args);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_ClearParticles(ParticleEmitter self);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_localVelocity(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_rndVelocity(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_worldVelocity(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_localVelocity(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_rndVelocity(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_worldVelocity(ref Vector3 value);
        /// <summary>
        /// <para>Advance particle simulation by given time.</para>
        /// </summary>
        /// <param name="deltaTime"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void Simulate(float deltaTime);

        /// <summary>
        /// <para>The angular velocity of new particles in degrees per second.</para>
        /// </summary>
        public float angularVelocity { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Should particles be automatically emitted each frame?</para>
        /// </summary>
        public bool emit { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The amount of the emitter's speed that the particles inherit.</para>
        /// </summary>
        public float emitterVelocityScale { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Turns the ParticleEmitter on or off.</para>
        /// </summary>
        public bool enabled { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The starting speed of particles along X, Y, and Z, measured in the object's orientation.</para>
        /// </summary>
        public Vector3 localVelocity
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_localVelocity(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_localVelocity(ref value);
            }
        }

        /// <summary>
        /// <para>The maximum number of particles that will be spawned every second.</para>
        /// </summary>
        public float maxEmission { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The maximum lifetime of each particle, measured in seconds.</para>
        /// </summary>
        public float maxEnergy { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The maximum size each particle can be at the time when it is spawned.</para>
        /// </summary>
        public float maxSize { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The minimum number of particles that will be spawned every second.</para>
        /// </summary>
        public float minEmission { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The minimum lifetime of each particle, measured in seconds.</para>
        /// </summary>
        public float minEnergy { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The minimum size each particle can be at the time when it is spawned.</para>
        /// </summary>
        public float minSize { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The current number of particles (Read Only).</para>
        /// </summary>
        public int particleCount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Returns a copy of all particles and assigns an array of all particles to be the current particles.</para>
        /// </summary>
        public UnityEngine.Particle[] particles { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>A random angular velocity modifier for new particles.</para>
        /// </summary>
        public float rndAngularVelocity { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>If enabled, the particles will be spawned with random rotations.</para>
        /// </summary>
        public bool rndRotation { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>A random speed along X, Y, and Z that is added to the velocity.</para>
        /// </summary>
        public Vector3 rndVelocity
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_rndVelocity(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_rndVelocity(ref value);
            }
        }

        /// <summary>
        /// <para>If enabled, the particles don't move when the emitter moves. If false, when you move the emitter, the particles follow it around.</para>
        /// </summary>
        public bool useWorldSpace { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The starting speed of particles in world space, along X, Y, and Z.</para>
        /// </summary>
        public Vector3 worldVelocity
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_worldVelocity(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_worldVelocity(ref value);
            }
        }
    }
}

