namespace UnityEngine
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;
    using UnityEngine.Rendering;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Script interface for particle systems (Shuriken).</para>
    /// </summary>
    [RequireComponent(typeof(Transform))]
    public sealed class ParticleSystem : Component
    {
        [CompilerGenerated]
        private static IteratorDelegate <>f__am$cache0;
        [CompilerGenerated]
        private static IteratorDelegate <>f__am$cache1;
        [CompilerGenerated]
        private static IteratorDelegate <>f__am$cache2;
        [CompilerGenerated]
        private static IteratorDelegate <>f__am$cache3;

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern bool CheckVertexStreamsMatchShader(bool hasTangent, bool hasColor, int texCoordChannelCount, Material material, ref bool tangentError, ref bool colorError, ref bool uvError);
        [ExcludeFromDocs]
        public void Clear()
        {
            bool withChildren = true;
            this.Clear(withChildren);
        }

        /// <summary>
        /// <para>Remove all particles in the particle system.</para>
        /// </summary>
        /// <param name="withChildren">Clear all child particle systems as well.</param>
        public void Clear([DefaultValue("true")] bool withChildren)
        {
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = ps => Internal_Clear(ps);
            }
            this.IterateParticleSystems(withChildren, <>f__am$cache2);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern bool CountSubEmitterParticles(ref int count);
        /// <summary>
        /// <para>Emit count particles immediately.</para>
        /// </summary>
        /// <param name="count">Number of particles to emit.</param>
        public void Emit(int count)
        {
            INTERNAL_CALL_Emit(this, count);
        }

        [Obsolete("Emit with a single particle structure is deprecated. Pass a ParticleSystem.EmitParams parameter instead, which allows you to override some/all of the emission properties")]
        public void Emit(Particle particle)
        {
            this.Internal_EmitOld(ref particle);
        }

        public void Emit(EmitParams emitParams, int count)
        {
            this.Internal_Emit(ref emitParams, count);
        }

        /// <summary>
        /// <para></para>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="velocity"></param>
        /// <param name="size"></param>
        /// <param name="lifetime"></param>
        /// <param name="color"></param>
        [Obsolete("Emit with specific parameters is deprecated. Pass a ParticleSystem.EmitParams parameter instead, which allows you to override some/all of the emission properties")]
        public void Emit(Vector3 position, Vector3 velocity, float size, float lifetime, Color32 color)
        {
            Particle particle = new Particle {
                position = position,
                velocity = velocity,
                lifetime = lifetime,
                startLifetime = lifetime,
                startSize = size,
                rotation3D = Vector3.zero,
                angularVelocity3D = Vector3.zero,
                startColor = color,
                randomSeed = 5
            };
            this.Internal_EmitOld(ref particle);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void GenerateNoisePreviewTexture(Texture2D dst);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern int GenerateRandomSeed();
        public int GetCustomParticleData(List<Vector4> customData, ParticleSystemCustomData streamIndex) => 
            this.GetCustomParticleDataInternal(customData, (int) streamIndex);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern int GetCustomParticleDataInternal(object customData, int streamIndex);
        internal Matrix4x4 GetLocalToWorldMatrix()
        {
            Matrix4x4 matrixx;
            INTERNAL_CALL_GetLocalToWorldMatrix(this, out matrixx);
            return matrixx;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern int GetMaxTexCoordStreams();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern int GetParticles(Particle[] particles);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Emit(ParticleSystem self, int count);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetLocalToWorldMatrix(ParticleSystem self, out Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool Internal_Clear(ParticleSystem self);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void Internal_Emit(ref EmitParams emitParams, int count);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void Internal_EmitOld(ref Particle particle);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_startColor(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_startRotation3D(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool Internal_IsAlive(ParticleSystem self);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool Internal_Pause(ParticleSystem self);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool Internal_Play(ParticleSystem self);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_startColor(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_startRotation3D(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool Internal_Simulate(ParticleSystem self, float t, bool restart, bool fixedTimeStep);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool Internal_Stop(ParticleSystem self, ParticleSystemStopBehavior stopBehavior);
        [ExcludeFromDocs]
        public bool IsAlive()
        {
            bool withChildren = true;
            return this.IsAlive(withChildren);
        }

        /// <summary>
        /// <para>Does the system have any live particles (or will produce more)?</para>
        /// </summary>
        /// <param name="withChildren">Check all child particle systems as well.</param>
        /// <returns>
        /// <para>True if the particle system is still "alive", false if the particle system is done emitting particles and all particles are dead.</para>
        /// </returns>
        public bool IsAlive([DefaultValue("true")] bool withChildren)
        {
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = ps => Internal_IsAlive(ps);
            }
            return this.IterateParticleSystems(withChildren, <>f__am$cache3);
        }

        internal bool IterateParticleSystems(bool recurse, IteratorDelegate func)
        {
            bool flag = func(this);
            if (recurse)
            {
                flag |= IterateParticleSystemsRecursive(base.transform, func);
            }
            return flag;
        }

        private static bool IterateParticleSystemsRecursive(Transform transform, IteratorDelegate func)
        {
            bool flag = false;
            int childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform child = transform.GetChild(i);
                ParticleSystem component = child.gameObject.GetComponent<ParticleSystem>();
                if (component != null)
                {
                    flag = func(component);
                    if (flag)
                    {
                        return flag;
                    }
                    IterateParticleSystemsRecursive(child, func);
                }
            }
            return flag;
        }

        [ExcludeFromDocs]
        public void Pause()
        {
            bool withChildren = true;
            this.Pause(withChildren);
        }

        /// <summary>
        /// <para>Pauses the system so no new particles are emitted and the existing particles are not updated.</para>
        /// </summary>
        /// <param name="withChildren">Pause all child particle systems as well.</param>
        public void Pause([DefaultValue("true")] bool withChildren)
        {
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = ps => Internal_Pause(ps);
            }
            this.IterateParticleSystems(withChildren, <>f__am$cache1);
        }

        [ExcludeFromDocs]
        public void Play()
        {
            bool withChildren = true;
            this.Play(withChildren);
        }

        /// <summary>
        /// <para>Starts the particle system.</para>
        /// </summary>
        /// <param name="withChildren">Play all child particle systems as well.</param>
        public void Play([DefaultValue("true")] bool withChildren)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = ps => Internal_Play(ps);
            }
            this.IterateParticleSystems(withChildren, <>f__am$cache0);
        }

        public void SetCustomParticleData(List<Vector4> customData, ParticleSystemCustomData streamIndex)
        {
            this.SetCustomParticleDataInternal(customData, (int) streamIndex);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void SetCustomParticleDataInternal(object customData, int streamIndex);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void SetParticles(Particle[] particles, int size);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void SetupDefaultType(int type);
        [ExcludeFromDocs]
        public void Simulate(float t)
        {
            bool fixedTimeStep = true;
            bool restart = true;
            bool withChildren = true;
            this.Simulate(t, withChildren, restart, fixedTimeStep);
        }

        [ExcludeFromDocs]
        public void Simulate(float t, bool withChildren)
        {
            bool fixedTimeStep = true;
            bool restart = true;
            this.Simulate(t, withChildren, restart, fixedTimeStep);
        }

        [ExcludeFromDocs]
        public void Simulate(float t, bool withChildren, bool restart)
        {
            bool fixedTimeStep = true;
            this.Simulate(t, withChildren, restart, fixedTimeStep);
        }

        /// <summary>
        /// <para>Fastforwards the particle system by simulating particles over given period of time, then pauses it.</para>
        /// </summary>
        /// <param name="t">Time period in seconds to advance the ParticleSystem simulation by. If restart is true, the ParticleSystem will be reset to 0 time, and then advanced by this value. If restart is false, the ParticleSystem simulation will be advanced in time from its current state by this value.</param>
        /// <param name="withChildren">Fastforward all child particle systems as well.</param>
        /// <param name="restart">Restart and start from the beginning.</param>
        /// <param name="fixedTimeStep">Only update the system at fixed intervals, based on the value in "Fixed Time" in the Time options.</param>
        public void Simulate(float t, [DefaultValue("true")] bool withChildren, [DefaultValue("true")] bool restart, [DefaultValue("true")] bool fixedTimeStep)
        {
            <Simulate>c__AnonStorey0 storey = new <Simulate>c__AnonStorey0 {
                t = t,
                restart = restart,
                fixedTimeStep = fixedTimeStep
            };
            this.IterateParticleSystems(withChildren, new IteratorDelegate(storey.<>m__0));
        }

        [ExcludeFromDocs]
        public void Stop()
        {
            ParticleSystemStopBehavior stopEmitting = ParticleSystemStopBehavior.StopEmitting;
            bool withChildren = true;
            this.Stop(withChildren, stopEmitting);
        }

        /// <summary>
        /// <para>Stops playing the particle system using the supplied stop behaviour.</para>
        /// </summary>
        /// <param name="withChildren">Stop all child particle systems as well.</param>
        /// <param name="stopBehavior">Stop emitting or stop emitting and clear the system.</param>
        [ExcludeFromDocs]
        public void Stop(bool withChildren)
        {
            ParticleSystemStopBehavior stopEmitting = ParticleSystemStopBehavior.StopEmitting;
            this.Stop(withChildren, stopEmitting);
        }

        /// <summary>
        /// <para>Stops playing the particle system using the supplied stop behaviour.</para>
        /// </summary>
        /// <param name="withChildren">Stop all child particle systems as well.</param>
        /// <param name="stopBehavior">Stop emitting or stop emitting and clear the system.</param>
        public void Stop([DefaultValue("true")] bool withChildren, [DefaultValue("ParticleSystemStopBehavior.StopEmitting")] ParticleSystemStopBehavior stopBehavior)
        {
            <Stop>c__AnonStorey1 storey = new <Stop>c__AnonStorey1 {
                stopBehavior = stopBehavior
            };
            this.IterateParticleSystems(withChildren, new IteratorDelegate(storey.<>m__0));
        }

        /// <summary>
        /// <para>Access the particle system collision module.</para>
        /// </summary>
        public CollisionModule collision =>
            new CollisionModule(this);

        /// <summary>
        /// <para>Access the particle system color by lifetime module.</para>
        /// </summary>
        public ColorBySpeedModule colorBySpeed =>
            new ColorBySpeedModule(this);

        /// <summary>
        /// <para>Access the particle system color over lifetime module.</para>
        /// </summary>
        public ColorOverLifetimeModule colorOverLifetime =>
            new ColorOverLifetimeModule(this);

        /// <summary>
        /// <para>Access the particle system Custom Data module.</para>
        /// </summary>
        public CustomDataModule customData =>
            new CustomDataModule(this);

        /// <summary>
        /// <para>The duration of the particle system in seconds (Read Only).</para>
        /// </summary>
        [Obsolete("duration property is deprecated. Use main.duration instead.")]
        public float duration { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Access the particle system emission module.</para>
        /// </summary>
        public EmissionModule emission =>
            new EmissionModule(this);

        /// <summary>
        /// <para>The rate of emission.</para>
        /// </summary>
        [Obsolete("emissionRate property is deprecated. Use emission.rateOverTime, emission.rateOverDistance, emission.rateOverTimeMultiplier or emission.rateOverDistanceMultiplier instead.")]
        public float emissionRate
        {
            get => 
                this.emission.rateOverTimeMultiplier;
            set
            {
                this.emission.rateOverTime = value;
            }
        }

        /// <summary>
        /// <para>When set to false, the particle system will not emit particles.</para>
        /// </summary>
        [Obsolete("enableEmission property is deprecated. Use emission.enabled instead.")]
        public bool enableEmission
        {
            get => 
                this.emission.enabled;
            set
            {
                this.emission.enabled = value;
            }
        }

        /// <summary>
        /// <para>Access the particle system external forces module.</para>
        /// </summary>
        public ExternalForcesModule externalForces =>
            new ExternalForcesModule(this);

        /// <summary>
        /// <para>Access the particle system force over lifetime module.</para>
        /// </summary>
        public ForceOverLifetimeModule forceOverLifetime =>
            new ForceOverLifetimeModule(this);

        /// <summary>
        /// <para>Scale being applied to the gravity defined by Physics.gravity.</para>
        /// </summary>
        [Obsolete("gravityModifier property is deprecated. Use main.gravityModifier or main.gravityModifierMultiplier instead.")]
        public float gravityModifier { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Access the particle system velocity inheritance module.</para>
        /// </summary>
        public InheritVelocityModule inheritVelocity =>
            new InheritVelocityModule(this);

        /// <summary>
        /// <para>Is the particle system currently emitting particles? A particle system may stop emitting when its emission module has finished, it has been paused or if the system has been stopped using ParticleSystem.Stop|Stop with the ParticleSystemStopBehavior.StopEmitting|StopEmitting flag. Resume emitting by calling ParticleSystem.Play|Play.</para>
        /// </summary>
        public bool isEmitting { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Is the particle system paused right now ?</para>
        /// </summary>
        public bool isPaused { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Is the particle system playing right now ?</para>
        /// </summary>
        public bool isPlaying { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Is the particle system stopped right now ?</para>
        /// </summary>
        public bool isStopped { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Access the particle system lights module.</para>
        /// </summary>
        public LightsModule lights =>
            new LightsModule(this);

        /// <summary>
        /// <para>Access the particle system limit velocity over lifetime module.</para>
        /// </summary>
        public LimitVelocityOverLifetimeModule limitVelocityOverLifetime =>
            new LimitVelocityOverLifetimeModule(this);

        /// <summary>
        /// <para>Is the particle system looping?</para>
        /// </summary>
        [Obsolete("loop property is deprecated. Use main.loop instead.")]
        public bool loop { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Access the main particle system settings.</para>
        /// </summary>
        public MainModule main =>
            new MainModule(this);

        /// <summary>
        /// <para>The maximum number of particles to emit.</para>
        /// </summary>
        [Obsolete("maxParticles property is deprecated. Use main.maxParticles instead.")]
        public int maxParticles { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Access the particle system noise module.</para>
        /// </summary>
        public NoiseModule noise =>
            new NoiseModule(this);

        /// <summary>
        /// <para>The current number of particles (Read Only).</para>
        /// </summary>
        public int particleCount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The playback speed of the particle system. 1 is normal playback speed.</para>
        /// </summary>
        [Obsolete("playbackSpeed property is deprecated. Use main.simulationSpeed instead.")]
        public float playbackSpeed { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>If set to true, the particle system will automatically start playing on startup.</para>
        /// </summary>
        [Obsolete("playOnAwake property is deprecated. Use main.playOnAwake instead.")]
        public bool playOnAwake { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Override the random seed used for the particle system emission.</para>
        /// </summary>
        public uint randomSeed { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Access the particle system rotation by speed  module.</para>
        /// </summary>
        public RotationBySpeedModule rotationBySpeed =>
            new RotationBySpeedModule(this);

        /// <summary>
        /// <para>Access the particle system rotation over lifetime module.</para>
        /// </summary>
        public RotationOverLifetimeModule rotationOverLifetime =>
            new RotationOverLifetimeModule(this);

        [Obsolete("safeCollisionEventSize has been deprecated. Use GetSafeCollisionEventSize() instead (UnityUpgradable) -> ParticlePhysicsExtensions.GetSafeCollisionEventSize(UnityEngine.ParticleSystem)", false)]
        public int safeCollisionEventSize =>
            ParticleSystemExtensionsImpl.GetSafeCollisionEventSize(this);

        /// <summary>
        /// <para>The scaling mode applied to particle sizes and positions.</para>
        /// </summary>
        [Obsolete("scalingMode property is deprecated. Use main.scalingMode instead.")]
        public ParticleSystemScalingMode scalingMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Access the particle system shape module.</para>
        /// </summary>
        public ShapeModule shape =>
            new ShapeModule(this);

        /// <summary>
        /// <para>This selects the space in which to simulate particles. It can be either world or local space.</para>
        /// </summary>
        [Obsolete("simulationSpace property is deprecated. Use main.simulationSpace instead.")]
        public ParticleSystemSimulationSpace simulationSpace { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Access the particle system size by speed module.</para>
        /// </summary>
        public SizeBySpeedModule sizeBySpeed =>
            new SizeBySpeedModule(this);

        /// <summary>
        /// <para>Access the particle system size over lifetime module.</para>
        /// </summary>
        public SizeOverLifetimeModule sizeOverLifetime =>
            new SizeOverLifetimeModule(this);

        /// <summary>
        /// <para>The initial color of particles when emitted.</para>
        /// </summary>
        [Obsolete("startColor property is deprecated. Use main.startColor instead.")]
        public Color startColor
        {
            get
            {
                Color color;
                this.INTERNAL_get_startColor(out color);
                return color;
            }
            set
            {
                this.INTERNAL_set_startColor(ref value);
            }
        }

        /// <summary>
        /// <para>Start delay in seconds.</para>
        /// </summary>
        [Obsolete("startDelay property is deprecated. Use main.startDelay or main.startDelayMultiplier instead.")]
        public float startDelay { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The total lifetime in seconds that particles will have when emitted. When using curves, this values acts as a scale on the curve. This value is set in the particle when it is created by the particle system.</para>
        /// </summary>
        [Obsolete("startLifetime property is deprecated. Use main.startLifetime or main.startLifetimeMultiplier instead.")]
        public float startLifetime { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The initial rotation of particles when emitted. When using curves, this values acts as a scale on the curve.</para>
        /// </summary>
        [Obsolete("startRotation property is deprecated. Use main.startRotation or main.startRotationMultiplier instead.")]
        public float startRotation { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The initial 3D rotation of particles when emitted. When using curves, this values acts as a scale on the curves.</para>
        /// </summary>
        [Obsolete("startRotation3D property is deprecated. Use main.startRotationX, main.startRotationY and main.startRotationZ instead. (Or main.startRotationXMultiplier, main.startRotationYMultiplier and main.startRotationZMultiplier).")]
        public Vector3 startRotation3D
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_startRotation3D(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_startRotation3D(ref value);
            }
        }

        /// <summary>
        /// <para>The initial size of particles when emitted. When using curves, this values acts as a scale on the curve.</para>
        /// </summary>
        [Obsolete("startSize property is deprecated. Use main.startSize or main.startSizeMultiplier instead.")]
        public float startSize { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The initial speed of particles when emitted. When using curves, this values acts as a scale on the curve.</para>
        /// </summary>
        [Obsolete("startSpeed property is deprecated. Use main.startSpeed or main.startSpeedMultiplier instead.")]
        public float startSpeed { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Access the particle system sub emitters module.</para>
        /// </summary>
        public SubEmittersModule subEmitters =>
            new SubEmittersModule(this);

        /// <summary>
        /// <para>Access the particle system texture sheet animation module.</para>
        /// </summary>
        public TextureSheetAnimationModule textureSheetAnimation =>
            new TextureSheetAnimationModule(this);

        /// <summary>
        /// <para>Playback position in seconds.</para>
        /// </summary>
        public float time { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Access the particle system trails module.</para>
        /// </summary>
        public TrailModule trails =>
            new TrailModule(this);

        /// <summary>
        /// <para>Access the particle system trigger module.</para>
        /// </summary>
        public TriggerModule trigger =>
            new TriggerModule(this);

        /// <summary>
        /// <para>Controls whether the Particle System uses an automatically-generated random number to seed the random number generator.</para>
        /// </summary>
        public bool useAutoRandomSeed { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Access the particle system velocity over lifetime module.</para>
        /// </summary>
        public VelocityOverLifetimeModule velocityOverLifetime =>
            new VelocityOverLifetimeModule(this);

        [CompilerGenerated]
        private sealed class <Simulate>c__AnonStorey0
        {
            internal bool fixedTimeStep;
            internal bool restart;
            internal float t;

            internal bool <>m__0(ParticleSystem ps) => 
                ParticleSystem.Internal_Simulate(ps, this.t, this.restart, this.fixedTimeStep);
        }

        [CompilerGenerated]
        private sealed class <Stop>c__AnonStorey1
        {
            internal ParticleSystemStopBehavior stopBehavior;

            internal bool <>m__0(ParticleSystem ps) => 
                ParticleSystem.Internal_Stop(ps, this.stopBehavior);
        }

        /// <summary>
        /// <para>Script interface for a Burst.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct Burst
        {
            private float m_Time;
            private short m_MinCount;
            private short m_MaxCount;
            private int m_RepeatCount;
            private float m_RepeatInterval;
            /// <summary>
            /// <para>Construct a new Burst with a time and count.</para>
            /// </summary>
            /// <param name="_time">Time to emit the burst.</param>
            /// <param name="_minCount">Minimum number of particles to emit.</param>
            /// <param name="_maxCount">Maximum number of particles to emit.</param>
            /// <param name="_count">Number of particles to emit.</param>
            public Burst(float _time, short _count)
            {
                this.m_Time = _time;
                this.m_MinCount = _count;
                this.m_MaxCount = _count;
                this.m_RepeatCount = 0;
                this.m_RepeatInterval = 0f;
            }

            /// <summary>
            /// <para>Construct a new Burst with a time and count.</para>
            /// </summary>
            /// <param name="_time">Time to emit the burst.</param>
            /// <param name="_minCount">Minimum number of particles to emit.</param>
            /// <param name="_maxCount">Maximum number of particles to emit.</param>
            /// <param name="_count">Number of particles to emit.</param>
            public Burst(float _time, short _minCount, short _maxCount)
            {
                this.m_Time = _time;
                this.m_MinCount = _minCount;
                this.m_MaxCount = _maxCount;
                this.m_RepeatCount = 0;
                this.m_RepeatInterval = 0f;
            }

            public Burst(float _time, short _minCount, short _maxCount, int _cycleCount, float _repeatInterval)
            {
                this.m_Time = _time;
                this.m_MinCount = _minCount;
                this.m_MaxCount = _maxCount;
                this.m_RepeatCount = _cycleCount - 1;
                this.m_RepeatInterval = _repeatInterval;
            }

            /// <summary>
            /// <para>The time that each burst occurs.</para>
            /// </summary>
            public float time
            {
                get => 
                    this.m_Time;
                set
                {
                    this.m_Time = value;
                }
            }
            /// <summary>
            /// <para>Minimum number of bursts to be emitted.</para>
            /// </summary>
            public short minCount
            {
                get => 
                    this.m_MinCount;
                set
                {
                    this.m_MinCount = value;
                }
            }
            /// <summary>
            /// <para>Maximum number of bursts to be emitted.</para>
            /// </summary>
            public short maxCount
            {
                get => 
                    this.m_MaxCount;
                set
                {
                    this.m_MaxCount = value;
                }
            }
            /// <summary>
            /// <para>How many times to play the burst. (0 means infinitely).</para>
            /// </summary>
            public int cycleCount
            {
                get => 
                    (this.m_RepeatCount + 1);
                set
                {
                    this.m_RepeatCount = value - 1;
                }
            }
            /// <summary>
            /// <para>How often to repeat the burst, in seconds.</para>
            /// </summary>
            public float repeatInterval
            {
                get => 
                    this.m_RepeatInterval;
                set
                {
                    this.m_RepeatInterval = value;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential, Size=1), Obsolete("ParticleSystem.CollisionEvent has been deprecated. Use ParticleCollisionEvent instead (UnityUpgradable) -> ParticleCollisionEvent", true)]
        public struct CollisionEvent
        {
            public Vector3 intersection =>
                new Vector3();
            public Vector3 normal =>
                new Vector3();
            public Vector3 velocity =>
                new Vector3();
            public Component collider =>
                null;
        }

        /// <summary>
        /// <para>Script interface for the Collision module.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct CollisionModule
        {
            private ParticleSystem m_ParticleSystem;
            internal CollisionModule(ParticleSystem particleSystem)
            {
                this.m_ParticleSystem = particleSystem;
            }

            /// <summary>
            /// <para>Enable/disable the Collision module.</para>
            /// </summary>
            public bool enabled
            {
                get => 
                    GetEnabled(this.m_ParticleSystem);
                set
                {
                    SetEnabled(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>The type of particle collision to perform.</para>
            /// </summary>
            public ParticleSystemCollisionType type
            {
                get => 
                    ((ParticleSystemCollisionType) GetType(this.m_ParticleSystem));
                set
                {
                    SetType(this.m_ParticleSystem, (int) value);
                }
            }
            /// <summary>
            /// <para>Choose between 2D and 3D world collisions.</para>
            /// </summary>
            public ParticleSystemCollisionMode mode
            {
                get => 
                    ((ParticleSystemCollisionMode) GetMode(this.m_ParticleSystem));
                set
                {
                    SetMode(this.m_ParticleSystem, (int) value);
                }
            }
            /// <summary>
            /// <para>How much speed is lost from each particle after a collision.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve dampen
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetDampen(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetDampen(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Change the dampen multiplier.</para>
            /// </summary>
            public float dampenMultiplier
            {
                get => 
                    GetDampenMultiplier(this.m_ParticleSystem);
                set
                {
                    SetDampenMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>How much force is applied to each particle after a collision.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve bounce
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetBounce(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetBounce(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Change the bounce multiplier.</para>
            /// </summary>
            public float bounceMultiplier
            {
                get => 
                    GetBounceMultiplier(this.m_ParticleSystem);
                set
                {
                    SetBounceMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>How much a particle's lifetime is reduced after a collision.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve lifetimeLoss
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetLifetimeLoss(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetLifetimeLoss(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Change the lifetime loss multiplier.</para>
            /// </summary>
            public float lifetimeLossMultiplier
            {
                get => 
                    GetLifetimeLossMultiplier(this.m_ParticleSystem);
                set
                {
                    SetLifetimeLossMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Kill particles whose speed falls below this threshold, after a collision.</para>
            /// </summary>
            public float minKillSpeed
            {
                get => 
                    GetMinKillSpeed(this.m_ParticleSystem);
                set
                {
                    SetMinKillSpeed(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Kill particles whose speed goes above this threshold, after a collision.</para>
            /// </summary>
            public float maxKillSpeed
            {
                get => 
                    GetMaxKillSpeed(this.m_ParticleSystem);
                set
                {
                    SetMaxKillSpeed(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Control which layers this particle system collides with.</para>
            /// </summary>
            public LayerMask collidesWith
            {
                get => 
                    GetCollidesWith(this.m_ParticleSystem);
                set
                {
                    SetCollidesWith(this.m_ParticleSystem, (int) value);
                }
            }
            /// <summary>
            /// <para>Allow particles to collide with dynamic colliders when using world collision mode.</para>
            /// </summary>
            public bool enableDynamicColliders
            {
                get => 
                    GetEnableDynamicColliders(this.m_ParticleSystem);
                set
                {
                    SetEnableDynamicColliders(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Allow particles to collide when inside colliders.</para>
            /// </summary>
            public bool enableInteriorCollisions
            {
                get => 
                    GetEnableInteriorCollisions(this.m_ParticleSystem);
                set
                {
                    SetEnableInteriorCollisions(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>The maximum number of collision shapes that will be considered for particle collisions. Excess shapes will be ignored. Terrains take priority.</para>
            /// </summary>
            public int maxCollisionShapes
            {
                get => 
                    GetMaxCollisionShapes(this.m_ParticleSystem);
                set
                {
                    SetMaxCollisionShapes(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Specifies the accuracy of particle collisions against colliders in the scene.</para>
            /// </summary>
            public ParticleSystemCollisionQuality quality
            {
                get => 
                    ((ParticleSystemCollisionQuality) GetQuality(this.m_ParticleSystem));
                set
                {
                    SetQuality(this.m_ParticleSystem, (int) value);
                }
            }
            /// <summary>
            /// <para>Size of voxels in the collision cache.</para>
            /// </summary>
            public float voxelSize
            {
                get => 
                    GetVoxelSize(this.m_ParticleSystem);
                set
                {
                    SetVoxelSize(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>A multiplier applied to the size of each particle before collisions are processed.</para>
            /// </summary>
            public float radiusScale
            {
                get => 
                    GetRadiusScale(this.m_ParticleSystem);
                set
                {
                    SetRadiusScale(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Send collision callback messages.</para>
            /// </summary>
            public bool sendCollisionMessages
            {
                get => 
                    GetUsesCollisionMessages(this.m_ParticleSystem);
                set
                {
                    SetUsesCollisionMessages(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Set a collision plane to be used with this particle system.</para>
            /// </summary>
            /// <param name="index">Specifies which plane to set.</param>
            /// <param name="transform">The plane to set.</param>
            public void SetPlane(int index, Transform transform)
            {
                SetPlane(this.m_ParticleSystem, index, transform);
            }

            /// <summary>
            /// <para>Get a collision plane associated with this particle system.</para>
            /// </summary>
            /// <param name="index">Specifies which plane to access.</param>
            /// <returns>
            /// <para>The plane.</para>
            /// </returns>
            public Transform GetPlane(int index) => 
                GetPlane(this.m_ParticleSystem, index);

            /// <summary>
            /// <para>The maximum number of planes it is possible to set as colliders.</para>
            /// </summary>
            public int maxPlaneCount =>
                GetMaxPlaneCount(this.m_ParticleSystem);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetEnabled(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetEnabled(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetType(ParticleSystem system, int value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern int GetType(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetMode(ParticleSystem system, int value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern int GetMode(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetDampen(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetDampen(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetDampenMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetDampenMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetBounce(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetBounce(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetBounceMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetBounceMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetLifetimeLoss(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetLifetimeLoss(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetLifetimeLossMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetLifetimeLossMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetMinKillSpeed(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetMinKillSpeed(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetMaxKillSpeed(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetMaxKillSpeed(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetCollidesWith(ParticleSystem system, int value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern int GetCollidesWith(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetEnableDynamicColliders(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetEnableDynamicColliders(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetEnableInteriorCollisions(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetEnableInteriorCollisions(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetMaxCollisionShapes(ParticleSystem system, int value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern int GetMaxCollisionShapes(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetQuality(ParticleSystem system, int value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern int GetQuality(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetVoxelSize(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetVoxelSize(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetRadiusScale(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetRadiusScale(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetUsesCollisionMessages(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetUsesCollisionMessages(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetPlane(ParticleSystem system, int index, Transform transform);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern Transform GetPlane(ParticleSystem system, int index);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern int GetMaxPlaneCount(ParticleSystem system);
        }

        /// <summary>
        /// <para>Script interface for the Color By Speed module.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ColorBySpeedModule
        {
            private ParticleSystem m_ParticleSystem;
            internal ColorBySpeedModule(ParticleSystem particleSystem)
            {
                this.m_ParticleSystem = particleSystem;
            }

            /// <summary>
            /// <para>Enable/disable the Color By Speed module.</para>
            /// </summary>
            public bool enabled
            {
                get => 
                    GetEnabled(this.m_ParticleSystem);
                set
                {
                    SetEnabled(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>The gradient controlling the particle colors.</para>
            /// </summary>
            public ParticleSystem.MinMaxGradient color
            {
                get
                {
                    ParticleSystem.MinMaxGradient gradient = new ParticleSystem.MinMaxGradient();
                    GetColor(this.m_ParticleSystem, ref gradient);
                    return gradient;
                }
                set
                {
                    SetColor(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Apply the color gradient between these minimum and maximum speeds.</para>
            /// </summary>
            public Vector2 range
            {
                get => 
                    GetRange(this.m_ParticleSystem);
                set
                {
                    SetRange(this.m_ParticleSystem, value);
                }
            }
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetEnabled(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetEnabled(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetColor(ParticleSystem system, ref ParticleSystem.MinMaxGradient gradient);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetColor(ParticleSystem system, ref ParticleSystem.MinMaxGradient gradient);
            private static void SetRange(ParticleSystem system, Vector2 value)
            {
                INTERNAL_CALL_SetRange(system, ref value);
            }

            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void INTERNAL_CALL_SetRange(ParticleSystem system, ref Vector2 value);
            private static Vector2 GetRange(ParticleSystem system)
            {
                Vector2 vector;
                INTERNAL_CALL_GetRange(system, out vector);
                return vector;
            }

            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void INTERNAL_CALL_GetRange(ParticleSystem system, out Vector2 value);
        }

        /// <summary>
        /// <para>Script interface for the Color Over Lifetime module.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ColorOverLifetimeModule
        {
            private ParticleSystem m_ParticleSystem;
            internal ColorOverLifetimeModule(ParticleSystem particleSystem)
            {
                this.m_ParticleSystem = particleSystem;
            }

            /// <summary>
            /// <para>Enable/disable the Color Over Lifetime module.</para>
            /// </summary>
            public bool enabled
            {
                get => 
                    GetEnabled(this.m_ParticleSystem);
                set
                {
                    SetEnabled(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>The gradient controlling the particle colors.</para>
            /// </summary>
            public ParticleSystem.MinMaxGradient color
            {
                get
                {
                    ParticleSystem.MinMaxGradient gradient = new ParticleSystem.MinMaxGradient();
                    GetColor(this.m_ParticleSystem, ref gradient);
                    return gradient;
                }
                set
                {
                    SetColor(this.m_ParticleSystem, ref value);
                }
            }
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetEnabled(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetEnabled(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetColor(ParticleSystem system, ref ParticleSystem.MinMaxGradient gradient);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetColor(ParticleSystem system, ref ParticleSystem.MinMaxGradient gradient);
        }

        /// <summary>
        /// <para>Script interface for the Custom Data module.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct CustomDataModule
        {
            private ParticleSystem m_ParticleSystem;
            internal CustomDataModule(ParticleSystem particleSystem)
            {
                this.m_ParticleSystem = particleSystem;
            }

            /// <summary>
            /// <para>Enable/disable the Custom Data module.</para>
            /// </summary>
            public bool enabled
            {
                get => 
                    GetEnabled(this.m_ParticleSystem);
                set
                {
                    SetEnabled(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Choose the type of custom data to generate for the chosen data stream.</para>
            /// </summary>
            /// <param name="stream">The name of the custom data stream to enable data generation on.</param>
            /// <param name="mode">The type of data to generate.</param>
            public void SetMode(ParticleSystemCustomData stream, ParticleSystemCustomDataMode mode)
            {
                SetMode(this.m_ParticleSystem, (int) stream, (int) mode);
            }

            /// <summary>
            /// <para>Find out the type of custom data that is being generated for the chosen data stream.</para>
            /// </summary>
            /// <param name="stream">The name of the custom data stream to query.</param>
            /// <returns>
            /// <para>The type of data being generated for the requested stream.</para>
            /// </returns>
            public ParticleSystemCustomDataMode GetMode(ParticleSystemCustomData stream) => 
                ((ParticleSystemCustomDataMode) GetMode(this.m_ParticleSystem, (int) stream));

            /// <summary>
            /// <para>Specify how many curves are used to generate custom data for this stream.</para>
            /// </summary>
            /// <param name="stream">The name of the custom data stream to apply the curve to.</param>
            /// <param name="curveCount">The number of curves to generate data for.</param>
            /// <param name="count"></param>
            public void SetVectorComponentCount(ParticleSystemCustomData stream, int count)
            {
                SetVectorComponentCount(this.m_ParticleSystem, (int) stream, count);
            }

            /// <summary>
            /// <para>Query how many ParticleSystem.MinMaxCurve elements are being used to generate this stream of custom data.</para>
            /// </summary>
            /// <param name="stream">The name of the custom data stream to retrieve the curve from.</param>
            /// <returns>
            /// <para>The number of curves.</para>
            /// </returns>
            public int GetVectorComponentCount(ParticleSystemCustomData stream) => 
                GetVectorComponentCount(this.m_ParticleSystem, (int) stream);

            public void SetVector(ParticleSystemCustomData stream, int component, ParticleSystem.MinMaxCurve curve)
            {
                SetVector(this.m_ParticleSystem, (int) stream, component, ref curve);
            }

            /// <summary>
            /// <para>Get a ParticleSystem.MinMaxCurve, that is being used to generate custom data.</para>
            /// </summary>
            /// <param name="stream">The name of the custom data stream to retrieve the curve from.</param>
            /// <param name="component">The component index to retrieve the curve for (0-3, mapping to the xyzw components of a Vector4 or float4).</param>
            /// <returns>
            /// <para>The curve being used to generate custom data.</para>
            /// </returns>
            public ParticleSystem.MinMaxCurve GetVector(ParticleSystemCustomData stream, int component)
            {
                ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                GetVector(this.m_ParticleSystem, (int) stream, component, ref curve);
                return curve;
            }

            public void SetColor(ParticleSystemCustomData stream, ParticleSystem.MinMaxGradient gradient)
            {
                SetColor(this.m_ParticleSystem, (int) stream, ref gradient);
            }

            /// <summary>
            /// <para>Get a ParticleSystem.MinMaxGradient, that is being used to generate custom HDR color data.</para>
            /// </summary>
            /// <param name="stream">The name of the custom data stream to retrieve the gradient from.</param>
            /// <returns>
            /// <para>The color gradient being used to generate custom color data.</para>
            /// </returns>
            public ParticleSystem.MinMaxGradient GetColor(ParticleSystemCustomData stream)
            {
                ParticleSystem.MinMaxGradient gradient = new ParticleSystem.MinMaxGradient();
                GetColor(this.m_ParticleSystem, (int) stream, ref gradient);
                return gradient;
            }

            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetEnabled(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetEnabled(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetMode(ParticleSystem system, int stream, int mode);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetVectorComponentCount(ParticleSystem system, int stream, int count);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetVector(ParticleSystem system, int stream, int component, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetColor(ParticleSystem system, int stream, ref ParticleSystem.MinMaxGradient gradient);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern int GetMode(ParticleSystem system, int stream);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern int GetVectorComponentCount(ParticleSystem system, int stream);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetVector(ParticleSystem system, int stream, int component, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetColor(ParticleSystem system, int stream, ref ParticleSystem.MinMaxGradient gradient);
        }

        /// <summary>
        /// <para>Script interface for the Emission module.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct EmissionModule
        {
            private ParticleSystem m_ParticleSystem;
            internal EmissionModule(ParticleSystem particleSystem)
            {
                this.m_ParticleSystem = particleSystem;
            }

            /// <summary>
            /// <para>Enable/disable the Emission module.</para>
            /// </summary>
            public bool enabled
            {
                get => 
                    GetEnabled(this.m_ParticleSystem);
                set
                {
                    SetEnabled(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>The rate at which new particles are spawned, over time.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve rateOverTime
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetRateOverTime(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetRateOverTime(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Change the rate over time multiplier.</para>
            /// </summary>
            public float rateOverTimeMultiplier
            {
                get => 
                    GetRateOverTimeMultiplier(this.m_ParticleSystem);
                set
                {
                    SetRateOverTimeMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>The rate at which new particles are spawned, over distance.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve rateOverDistance
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetRateOverDistance(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetRateOverDistance(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Change the rate over distance multiplier.</para>
            /// </summary>
            public float rateOverDistanceMultiplier
            {
                get => 
                    GetRateOverDistanceMultiplier(this.m_ParticleSystem);
                set
                {
                    SetRateOverDistanceMultiplier(this.m_ParticleSystem, value);
                }
            }
            public void SetBursts(ParticleSystem.Burst[] bursts)
            {
                SetBursts(this.m_ParticleSystem, bursts, bursts.Length);
            }

            public void SetBursts(ParticleSystem.Burst[] bursts, int size)
            {
                SetBursts(this.m_ParticleSystem, bursts, size);
            }

            public int GetBursts(ParticleSystem.Burst[] bursts) => 
                GetBursts(this.m_ParticleSystem, bursts);

            /// <summary>
            /// <para>The current number of bursts.</para>
            /// </summary>
            public int burstCount =>
                GetBurstCount(this.m_ParticleSystem);
            /// <summary>
            /// <para>The emission type.</para>
            /// </summary>
            [Obsolete("ParticleSystemEmissionType no longer does anything. Time and Distance based emission are now both always active.")]
            public ParticleSystemEmissionType type
            {
                get => 
                    ParticleSystemEmissionType.Time;
                set
                {
                }
            }
            /// <summary>
            /// <para>The rate at which new particles are spawned.</para>
            /// </summary>
            [Obsolete("rate property is deprecated. Use rateOverTime or rateOverDistance instead.")]
            public ParticleSystem.MinMaxCurve rate
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetRateOverTime(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetRateOverTime(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Change the rate multiplier.</para>
            /// </summary>
            [Obsolete("rateMultiplier property is deprecated. Use rateOverTimeMultiplier or rateOverDistanceMultiplier instead.")]
            public float rateMultiplier
            {
                get => 
                    GetRateOverTimeMultiplier(this.m_ParticleSystem);
                set
                {
                    SetRateOverTimeMultiplier(this.m_ParticleSystem, value);
                }
            }
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetEnabled(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetEnabled(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern int GetBurstCount(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetRateOverTime(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetRateOverTime(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetRateOverTimeMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetRateOverTimeMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetRateOverDistance(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetRateOverDistance(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetRateOverDistanceMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetRateOverDistanceMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetBursts(ParticleSystem system, ParticleSystem.Burst[] bursts, int size);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern int GetBursts(ParticleSystem system, ParticleSystem.Burst[] bursts);
        }

        /// <summary>
        /// <para>Script interface for particle emission parameters.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct EmitParams
        {
            internal ParticleSystem.Particle m_Particle;
            internal bool m_PositionSet;
            internal bool m_VelocitySet;
            internal bool m_AxisOfRotationSet;
            internal bool m_RotationSet;
            internal bool m_AngularVelocitySet;
            internal bool m_StartSizeSet;
            internal bool m_StartColorSet;
            internal bool m_RandomSeedSet;
            internal bool m_StartLifetimeSet;
            internal bool m_ApplyShapeToPosition;
            /// <summary>
            /// <para>Override the position of emitted particles.</para>
            /// </summary>
            public Vector3 position
            {
                get => 
                    this.m_Particle.position;
                set
                {
                    this.m_Particle.position = value;
                    this.m_PositionSet = true;
                }
            }
            /// <summary>
            /// <para>When overriding the position of particles, setting this flag to true allows you to retain the influence of the shape module.</para>
            /// </summary>
            public bool applyShapeToPosition
            {
                get => 
                    this.m_ApplyShapeToPosition;
                set
                {
                    this.m_ApplyShapeToPosition = value;
                }
            }
            /// <summary>
            /// <para>Override the velocity of emitted particles.</para>
            /// </summary>
            public Vector3 velocity
            {
                get => 
                    this.m_Particle.velocity;
                set
                {
                    this.m_Particle.velocity = value;
                    this.m_VelocitySet = true;
                }
            }
            /// <summary>
            /// <para>Override the lifetime of emitted particles.</para>
            /// </summary>
            public float startLifetime
            {
                get => 
                    this.m_Particle.startLifetime;
                set
                {
                    this.m_Particle.startLifetime = value;
                    this.m_StartLifetimeSet = true;
                }
            }
            /// <summary>
            /// <para>Override the initial size of emitted particles.</para>
            /// </summary>
            public float startSize
            {
                get => 
                    this.m_Particle.startSize;
                set
                {
                    this.m_Particle.startSize = value;
                    this.m_StartSizeSet = true;
                }
            }
            /// <summary>
            /// <para>Override the initial 3D size of emitted particles.</para>
            /// </summary>
            public Vector3 startSize3D
            {
                get => 
                    this.m_Particle.startSize3D;
                set
                {
                    this.m_Particle.startSize3D = value;
                    this.m_StartSizeSet = true;
                }
            }
            /// <summary>
            /// <para>Override the axis of rotation of emitted particles.</para>
            /// </summary>
            public Vector3 axisOfRotation
            {
                get => 
                    this.m_Particle.axisOfRotation;
                set
                {
                    this.m_Particle.axisOfRotation = value;
                    this.m_AxisOfRotationSet = true;
                }
            }
            /// <summary>
            /// <para>Override the rotation of emitted particles.</para>
            /// </summary>
            public float rotation
            {
                get => 
                    this.m_Particle.rotation;
                set
                {
                    this.m_Particle.rotation = value;
                    this.m_RotationSet = true;
                }
            }
            /// <summary>
            /// <para>Override the 3D rotation of emitted particles.</para>
            /// </summary>
            public Vector3 rotation3D
            {
                get => 
                    this.m_Particle.rotation3D;
                set
                {
                    this.m_Particle.rotation3D = value;
                    this.m_RotationSet = true;
                }
            }
            /// <summary>
            /// <para>Override the angular velocity of emitted particles.</para>
            /// </summary>
            public float angularVelocity
            {
                get => 
                    this.m_Particle.angularVelocity;
                set
                {
                    this.m_Particle.angularVelocity = value;
                    this.m_AngularVelocitySet = true;
                }
            }
            /// <summary>
            /// <para>Override the 3D angular velocity of emitted particles.</para>
            /// </summary>
            public Vector3 angularVelocity3D
            {
                get => 
                    this.m_Particle.angularVelocity3D;
                set
                {
                    this.m_Particle.angularVelocity3D = value;
                    this.m_AngularVelocitySet = true;
                }
            }
            /// <summary>
            /// <para>Override the initial color of emitted particles.</para>
            /// </summary>
            public Color32 startColor
            {
                get => 
                    this.m_Particle.startColor;
                set
                {
                    this.m_Particle.startColor = value;
                    this.m_StartColorSet = true;
                }
            }
            /// <summary>
            /// <para>Override the random seed of emitted particles.</para>
            /// </summary>
            public uint randomSeed
            {
                get => 
                    this.m_Particle.randomSeed;
                set
                {
                    this.m_Particle.randomSeed = value;
                    this.m_RandomSeedSet = true;
                }
            }
            /// <summary>
            /// <para>Revert the position back to the value specified in the inspector.</para>
            /// </summary>
            public void ResetPosition()
            {
                this.m_PositionSet = false;
            }

            /// <summary>
            /// <para>Revert the velocity back to the value specified in the inspector.</para>
            /// </summary>
            public void ResetVelocity()
            {
                this.m_VelocitySet = false;
            }

            /// <summary>
            /// <para>Revert the axis of rotation back to the value specified in the inspector.</para>
            /// </summary>
            public void ResetAxisOfRotation()
            {
                this.m_AxisOfRotationSet = false;
            }

            /// <summary>
            /// <para>Reverts rotation and rotation3D back to the values specified in the inspector.</para>
            /// </summary>
            public void ResetRotation()
            {
                this.m_RotationSet = false;
            }

            /// <summary>
            /// <para>Reverts angularVelocity and angularVelocity3D back to the values specified in the inspector.</para>
            /// </summary>
            public void ResetAngularVelocity()
            {
                this.m_AngularVelocitySet = false;
            }

            /// <summary>
            /// <para>Revert the initial size back to the value specified in the inspector.</para>
            /// </summary>
            public void ResetStartSize()
            {
                this.m_StartSizeSet = false;
            }

            /// <summary>
            /// <para>Revert the initial color back to the value specified in the inspector.</para>
            /// </summary>
            public void ResetStartColor()
            {
                this.m_StartColorSet = false;
            }

            /// <summary>
            /// <para>Revert the random seed back to the value specified in the inspector.</para>
            /// </summary>
            public void ResetRandomSeed()
            {
                this.m_RandomSeedSet = false;
            }

            /// <summary>
            /// <para>Revert the lifetime back to the value specified in the inspector.</para>
            /// </summary>
            public void ResetStartLifetime()
            {
                this.m_StartLifetimeSet = false;
            }
        }

        /// <summary>
        /// <para>Script interface for the External Forces module.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ExternalForcesModule
        {
            private ParticleSystem m_ParticleSystem;
            internal ExternalForcesModule(ParticleSystem particleSystem)
            {
                this.m_ParticleSystem = particleSystem;
            }

            /// <summary>
            /// <para>Enable/disable the External Forces module.</para>
            /// </summary>
            public bool enabled
            {
                get => 
                    GetEnabled(this.m_ParticleSystem);
                set
                {
                    SetEnabled(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Multiplies the magnitude of applied external forces.</para>
            /// </summary>
            public float multiplier
            {
                get => 
                    GetMultiplier(this.m_ParticleSystem);
                set
                {
                    SetMultiplier(this.m_ParticleSystem, value);
                }
            }
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetEnabled(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetEnabled(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetMultiplier(ParticleSystem system);
        }

        /// <summary>
        /// <para>Script interface for the Force Over Lifetime module.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ForceOverLifetimeModule
        {
            private ParticleSystem m_ParticleSystem;
            internal ForceOverLifetimeModule(ParticleSystem particleSystem)
            {
                this.m_ParticleSystem = particleSystem;
            }

            /// <summary>
            /// <para>Enable/disable the Force Over Lifetime module.</para>
            /// </summary>
            public bool enabled
            {
                get => 
                    GetEnabled(this.m_ParticleSystem);
                set
                {
                    SetEnabled(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>The curve defining particle forces in the X axis.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve x
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetX(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetX(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>The curve defining particle forces in the Y axis.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve y
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetY(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetY(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>The curve defining particle forces in the Z axis.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve z
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetZ(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetZ(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Change the X axis mulutiplier.</para>
            /// </summary>
            public float xMultiplier
            {
                get => 
                    GetXMultiplier(this.m_ParticleSystem);
                set
                {
                    SetXMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Change the Y axis multiplier.</para>
            /// </summary>
            public float yMultiplier
            {
                get => 
                    GetYMultiplier(this.m_ParticleSystem);
                set
                {
                    SetYMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Change the Z axis multiplier.</para>
            /// </summary>
            public float zMultiplier
            {
                get => 
                    GetZMultiplier(this.m_ParticleSystem);
                set
                {
                    SetZMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Are the forces being applied in local or world space?</para>
            /// </summary>
            public ParticleSystemSimulationSpace space
            {
                get => 
                    (!GetWorldSpace(this.m_ParticleSystem) ? ParticleSystemSimulationSpace.Local : ParticleSystemSimulationSpace.World);
                set
                {
                    SetWorldSpace(this.m_ParticleSystem, value == ParticleSystemSimulationSpace.World);
                }
            }
            /// <summary>
            /// <para>When randomly selecting values between two curves or constants, this flag will cause a new random force to be chosen on each frame.</para>
            /// </summary>
            public bool randomized
            {
                get => 
                    GetRandomized(this.m_ParticleSystem);
                set
                {
                    SetRandomized(this.m_ParticleSystem, value);
                }
            }
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetEnabled(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetEnabled(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetXMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetXMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetYMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetYMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetZMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetZMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetWorldSpace(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetWorldSpace(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetRandomized(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetRandomized(ParticleSystem system);
        }

        /// <summary>
        /// <para>The Inherit Velocity Module controls how the velocity of the emitter is transferred to the particles as they are emitted.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct InheritVelocityModule
        {
            private ParticleSystem m_ParticleSystem;
            internal InheritVelocityModule(ParticleSystem particleSystem)
            {
                this.m_ParticleSystem = particleSystem;
            }

            /// <summary>
            /// <para>Enable/disable the InheritVelocity module.</para>
            /// </summary>
            public bool enabled
            {
                get => 
                    GetEnabled(this.m_ParticleSystem);
                set
                {
                    SetEnabled(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>How to apply emitter velocity to particles.</para>
            /// </summary>
            public ParticleSystemInheritVelocityMode mode
            {
                get => 
                    ((ParticleSystemInheritVelocityMode) GetMode(this.m_ParticleSystem));
                set
                {
                    SetMode(this.m_ParticleSystem, (int) value);
                }
            }
            /// <summary>
            /// <para>Curve to define how much emitter velocity is applied during the lifetime of a particle.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve curve
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetCurve(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetCurve(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Change the curve multiplier.</para>
            /// </summary>
            public float curveMultiplier
            {
                get => 
                    GetCurveMultiplier(this.m_ParticleSystem);
                set
                {
                    SetCurveMultiplier(this.m_ParticleSystem, value);
                }
            }
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetEnabled(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetEnabled(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetMode(ParticleSystem system, int value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern int GetMode(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetCurve(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetCurve(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetCurveMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetCurveMultiplier(ParticleSystem system);
        }

        internal delegate bool IteratorDelegate(ParticleSystem ps);

        /// <summary>
        /// <para>Access the particle system lights module.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct LightsModule
        {
            private ParticleSystem m_ParticleSystem;
            internal LightsModule(ParticleSystem particleSystem)
            {
                this.m_ParticleSystem = particleSystem;
            }

            /// <summary>
            /// <para>Enable/disable the Lights module.</para>
            /// </summary>
            public bool enabled
            {
                get => 
                    GetEnabled(this.m_ParticleSystem);
                set
                {
                    SetEnabled(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Choose what proportion of particles will receive a dynamic light.</para>
            /// </summary>
            public float ratio
            {
                get => 
                    GetRatio(this.m_ParticleSystem);
                set
                {
                    SetRatio(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Randomly assign lights to new particles based on ParticleSystem.LightsModule.ratio.</para>
            /// </summary>
            public bool useRandomDistribution
            {
                get => 
                    GetUseRandomDistribution(this.m_ParticleSystem);
                set
                {
                    SetUseRandomDistribution(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Select what Light prefab you want to base your particle lights on.</para>
            /// </summary>
            public Light light
            {
                get => 
                    GetLightPrefab(this.m_ParticleSystem);
                set
                {
                    SetLightPrefab(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Toggle whether the particle lights will have their color multiplied by the particle color.</para>
            /// </summary>
            public bool useParticleColor
            {
                get => 
                    GetUseParticleColor(this.m_ParticleSystem);
                set
                {
                    SetUseParticleColor(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Toggle where the particle size will be multiplied by the light range, to determine the final light range.</para>
            /// </summary>
            public bool sizeAffectsRange
            {
                get => 
                    GetSizeAffectsRange(this.m_ParticleSystem);
                set
                {
                    SetSizeAffectsRange(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Toggle whether the particle alpha gets multiplied by the light intensity, when computing the final light intensity.</para>
            /// </summary>
            public bool alphaAffectsIntensity
            {
                get => 
                    GetAlphaAffectsIntensity(this.m_ParticleSystem);
                set
                {
                    SetAlphaAffectsIntensity(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Define a curve to apply custom range scaling to particle lights.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve range
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetRange(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetRange(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Range multiplier.</para>
            /// </summary>
            public float rangeMultiplier
            {
                get => 
                    GetRangeMultiplier(this.m_ParticleSystem);
                set
                {
                    SetRangeMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Define a curve to apply custom intensity scaling to particle lights.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve intensity
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetIntensity(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetIntensity(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Intensity multiplier.</para>
            /// </summary>
            public float intensityMultiplier
            {
                get => 
                    GetIntensityMultiplier(this.m_ParticleSystem);
                set
                {
                    SetIntensityMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Set a limit on how many lights this Module can create.</para>
            /// </summary>
            public int maxLights
            {
                get => 
                    GetMaxLights(this.m_ParticleSystem);
                set
                {
                    SetMaxLights(this.m_ParticleSystem, value);
                }
            }
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetEnabled(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetEnabled(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetRatio(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetRatio(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetUseRandomDistribution(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetUseRandomDistribution(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetLightPrefab(ParticleSystem system, Light value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern Light GetLightPrefab(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetUseParticleColor(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetUseParticleColor(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetSizeAffectsRange(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetSizeAffectsRange(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetAlphaAffectsIntensity(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetAlphaAffectsIntensity(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetRange(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetRange(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetRangeMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetRangeMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetIntensity(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetIntensity(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetIntensityMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetIntensityMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetMaxLights(ParticleSystem system, int value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern int GetMaxLights(ParticleSystem system);
        }

        /// <summary>
        /// <para>Script interface for the Limit Velocity Over Lifetime module.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct LimitVelocityOverLifetimeModule
        {
            private ParticleSystem m_ParticleSystem;
            internal LimitVelocityOverLifetimeModule(ParticleSystem particleSystem)
            {
                this.m_ParticleSystem = particleSystem;
            }

            /// <summary>
            /// <para>Enable/disable the Limit Force Over Lifetime module.</para>
            /// </summary>
            public bool enabled
            {
                get => 
                    GetEnabled(this.m_ParticleSystem);
                set
                {
                    SetEnabled(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Maximum velocity curve for the X axis.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve limitX
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetX(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetX(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Change the limit multiplier on the X axis.</para>
            /// </summary>
            public float limitXMultiplier
            {
                get => 
                    GetXMultiplier(this.m_ParticleSystem);
                set
                {
                    SetXMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Maximum velocity curve for the Y axis.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve limitY
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetY(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetY(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Change the limit multiplier on the Y axis.</para>
            /// </summary>
            public float limitYMultiplier
            {
                get => 
                    GetYMultiplier(this.m_ParticleSystem);
                set
                {
                    SetYMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Maximum velocity curve for the Z axis.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve limitZ
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetZ(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetZ(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Change the limit multiplier on the Z axis.</para>
            /// </summary>
            public float limitZMultiplier
            {
                get => 
                    GetZMultiplier(this.m_ParticleSystem);
                set
                {
                    SetZMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Maximum velocity curve, when not using one curve per axis.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve limit
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetMagnitude(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetMagnitude(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Change the limit multiplier.</para>
            /// </summary>
            public float limitMultiplier
            {
                get => 
                    GetMagnitudeMultiplier(this.m_ParticleSystem);
                set
                {
                    SetMagnitudeMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Controls how much the velocity that exceeds the velocity limit should be dampened.</para>
            /// </summary>
            public float dampen
            {
                get => 
                    GetDampen(this.m_ParticleSystem);
                set
                {
                    SetDampen(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Set the velocity limit on each axis separately.</para>
            /// </summary>
            public bool separateAxes
            {
                get => 
                    GetSeparateAxes(this.m_ParticleSystem);
                set
                {
                    SetSeparateAxes(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Specifies if the velocity limits are in local space (rotated with the transform) or world space.</para>
            /// </summary>
            public ParticleSystemSimulationSpace space
            {
                get => 
                    (!GetWorldSpace(this.m_ParticleSystem) ? ParticleSystemSimulationSpace.Local : ParticleSystemSimulationSpace.World);
                set
                {
                    SetWorldSpace(this.m_ParticleSystem, value == ParticleSystemSimulationSpace.World);
                }
            }
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetEnabled(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetEnabled(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetXMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetXMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetYMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetYMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetZMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetZMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetMagnitude(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetMagnitude(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetMagnitudeMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetMagnitudeMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetDampen(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetDampen(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetSeparateAxes(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetSeparateAxes(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetWorldSpace(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetWorldSpace(ParticleSystem system);
        }

        /// <summary>
        /// <para>Script interface for the main module.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MainModule
        {
            private ParticleSystem m_ParticleSystem;
            internal MainModule(ParticleSystem particleSystem)
            {
                this.m_ParticleSystem = particleSystem;
            }

            /// <summary>
            /// <para>The duration of the particle system in seconds.</para>
            /// </summary>
            public float duration
            {
                get => 
                    GetDuration(this.m_ParticleSystem);
                set
                {
                    SetDuration(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Is the particle system looping?</para>
            /// </summary>
            public bool loop
            {
                get => 
                    GetLoop(this.m_ParticleSystem);
                set
                {
                    SetLoop(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>When looping is enabled, this controls whether this particle system will look like it has already simulated for one loop when first becoming visible.</para>
            /// </summary>
            public bool prewarm
            {
                get => 
                    GetPrewarm(this.m_ParticleSystem);
                set
                {
                    SetPrewarm(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Start delay in seconds.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve startDelay
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetStartDelay(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetStartDelay(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Start delay multiplier in seconds.</para>
            /// </summary>
            public float startDelayMultiplier
            {
                get => 
                    GetStartDelayMultiplier(this.m_ParticleSystem);
                set
                {
                    SetStartDelayMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>The total lifetime in seconds that each new particle will have.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve startLifetime
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetStartLifetime(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetStartLifetime(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Start lifetime multiplier.</para>
            /// </summary>
            public float startLifetimeMultiplier
            {
                get => 
                    GetStartLifetimeMultiplier(this.m_ParticleSystem);
                set
                {
                    SetStartLifetimeMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>The initial speed of particles when emitted.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve startSpeed
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetStartSpeed(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetStartSpeed(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>A multiplier of the initial speed of particles when emitted.</para>
            /// </summary>
            public float startSpeedMultiplier
            {
                get => 
                    GetStartSpeedMultiplier(this.m_ParticleSystem);
                set
                {
                    SetStartSpeedMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>A flag to enable specifying particle size individually for each axis.</para>
            /// </summary>
            public bool startSize3D
            {
                get => 
                    GetStartSize3D(this.m_ParticleSystem);
                set
                {
                    SetStartSize3D(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>The initial size of particles when emitted.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve startSize
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetStartSizeX(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetStartSizeX(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Start size multiplier.</para>
            /// </summary>
            public float startSizeMultiplier
            {
                get => 
                    GetStartSizeXMultiplier(this.m_ParticleSystem);
                set
                {
                    SetStartSizeXMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>The initial size of particles along the X axis when emitted.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve startSizeX
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetStartSizeX(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetStartSizeX(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Start rotation multiplier along the X axis.</para>
            /// </summary>
            public float startSizeXMultiplier
            {
                get => 
                    GetStartSizeXMultiplier(this.m_ParticleSystem);
                set
                {
                    SetStartSizeXMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>The initial size of particles along the Y axis when emitted.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve startSizeY
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetStartSizeY(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetStartSizeY(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Start rotation multiplier along the Y axis.</para>
            /// </summary>
            public float startSizeYMultiplier
            {
                get => 
                    GetStartSizeYMultiplier(this.m_ParticleSystem);
                set
                {
                    SetStartSizeYMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>The initial size of particles along the Z axis when emitted.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve startSizeZ
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetStartSizeZ(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetStartSizeZ(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Start rotation multiplier along the Z axis.</para>
            /// </summary>
            public float startSizeZMultiplier
            {
                get => 
                    GetStartSizeZMultiplier(this.m_ParticleSystem);
                set
                {
                    SetStartSizeZMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>A flag to enable 3D particle rotation.</para>
            /// </summary>
            public bool startRotation3D
            {
                get => 
                    GetStartRotation3D(this.m_ParticleSystem);
                set
                {
                    SetStartRotation3D(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>The initial rotation of particles when emitted.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve startRotation
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetStartRotationZ(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetStartRotationZ(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Start rotation multiplier.</para>
            /// </summary>
            public float startRotationMultiplier
            {
                get => 
                    GetStartRotationZMultiplier(this.m_ParticleSystem);
                set
                {
                    SetStartRotationZMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>The initial rotation of particles around the X axis when emitted.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve startRotationX
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetStartRotationX(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetStartRotationX(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Start rotation multiplier around the X axis.</para>
            /// </summary>
            public float startRotationXMultiplier
            {
                get => 
                    GetStartRotationXMultiplier(this.m_ParticleSystem);
                set
                {
                    SetStartRotationXMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>The initial rotation of particles around the Y axis when emitted.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve startRotationY
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetStartRotationY(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetStartRotationY(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Start rotation multiplier around the Y axis.</para>
            /// </summary>
            public float startRotationYMultiplier
            {
                get => 
                    GetStartRotationYMultiplier(this.m_ParticleSystem);
                set
                {
                    SetStartRotationYMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>The initial rotation of particles around the Z axis when emitted.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve startRotationZ
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetStartRotationZ(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetStartRotationZ(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Start rotation multiplier around the Z axis.</para>
            /// </summary>
            public float startRotationZMultiplier
            {
                get => 
                    GetStartRotationZMultiplier(this.m_ParticleSystem);
                set
                {
                    SetStartRotationZMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Cause some particles to spin in  the opposite direction.</para>
            /// </summary>
            public float randomizeRotationDirection
            {
                get => 
                    GetRandomizeRotationDirection(this.m_ParticleSystem);
                set
                {
                    SetRandomizeRotationDirection(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>The initial color of particles when emitted.</para>
            /// </summary>
            public ParticleSystem.MinMaxGradient startColor
            {
                get
                {
                    ParticleSystem.MinMaxGradient gradient = new ParticleSystem.MinMaxGradient();
                    GetStartColor(this.m_ParticleSystem, ref gradient);
                    return gradient;
                }
                set
                {
                    SetStartColor(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Scale applied to the gravity, defined by Physics.gravity.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve gravityModifier
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetGravityModifier(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetGravityModifier(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Change the gravity mulutiplier.</para>
            /// </summary>
            public float gravityModifierMultiplier
            {
                get => 
                    GetGravityModifierMultiplier(this.m_ParticleSystem);
                set
                {
                    SetGravityModifierMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>This selects the space in which to simulate particles. It can be either world or local space.</para>
            /// </summary>
            public ParticleSystemSimulationSpace simulationSpace
            {
                get => 
                    GetSimulationSpace(this.m_ParticleSystem);
                set
                {
                    SetSimulationSpace(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Simulate particles relative to a custom transform component.</para>
            /// </summary>
            public Transform customSimulationSpace
            {
                get => 
                    GetCustomSimulationSpace(this.m_ParticleSystem);
                set
                {
                    SetCustomSimulationSpace(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Override the default playback speed of the Particle System.</para>
            /// </summary>
            public float simulationSpeed
            {
                get => 
                    GetSimulationSpeed(this.m_ParticleSystem);
                set
                {
                    SetSimulationSpeed(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Control how the particle system's Transform Component is applied to the particle system.</para>
            /// </summary>
            public ParticleSystemScalingMode scalingMode
            {
                get => 
                    GetScalingMode(this.m_ParticleSystem);
                set
                {
                    SetScalingMode(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>If set to true, the particle system will automatically start playing on startup.</para>
            /// </summary>
            public bool playOnAwake
            {
                get => 
                    GetPlayOnAwake(this.m_ParticleSystem);
                set
                {
                    SetPlayOnAwake(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>The maximum number of particles to emit.</para>
            /// </summary>
            public int maxParticles
            {
                get => 
                    GetMaxParticles(this.m_ParticleSystem);
                set
                {
                    SetMaxParticles(this.m_ParticleSystem, value);
                }
            }
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetEnabled(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetEnabled(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetDuration(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetDuration(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetLoop(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetLoop(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetPrewarm(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetPrewarm(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetStartDelay(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetStartDelay(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetStartDelayMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetStartDelayMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetStartLifetime(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetStartLifetime(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetStartLifetimeMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetStartLifetimeMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetStartSpeed(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetStartSpeed(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetStartSpeedMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetStartSpeedMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetStartSize3D(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetStartSize3D(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetStartSizeX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetStartSizeX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetStartSizeXMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetStartSizeXMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetStartSizeY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetStartSizeY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetStartSizeYMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetStartSizeYMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetStartSizeZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetStartSizeZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetStartSizeZMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetStartSizeZMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetStartRotation3D(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetStartRotation3D(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetStartRotationX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetStartRotationX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetStartRotationXMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetStartRotationXMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetStartRotationY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetStartRotationY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetStartRotationYMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetStartRotationYMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetStartRotationZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetStartRotationZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetStartRotationZMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetStartRotationZMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetRandomizeRotationDirection(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetRandomizeRotationDirection(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetStartColor(ParticleSystem system, ref ParticleSystem.MinMaxGradient gradient);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetStartColor(ParticleSystem system, ref ParticleSystem.MinMaxGradient gradient);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetGravityModifier(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetGravityModifier(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetGravityModifierMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetGravityModifierMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetSimulationSpace(ParticleSystem system, ParticleSystemSimulationSpace value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern ParticleSystemSimulationSpace GetSimulationSpace(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetCustomSimulationSpace(ParticleSystem system, Transform value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern Transform GetCustomSimulationSpace(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetSimulationSpeed(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetSimulationSpeed(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetScalingMode(ParticleSystem system, ParticleSystemScalingMode value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern ParticleSystemScalingMode GetScalingMode(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetPlayOnAwake(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetPlayOnAwake(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetMaxParticles(ParticleSystem system, int value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern int GetMaxParticles(ParticleSystem system);
        }

        /// <summary>
        /// <para>Script interface for a Min-Max Curve.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MinMaxCurve
        {
            private ParticleSystemCurveMode m_Mode;
            private float m_CurveMultiplier;
            private AnimationCurve m_CurveMin;
            private AnimationCurve m_CurveMax;
            private float m_ConstantMin;
            private float m_ConstantMax;
            /// <summary>
            /// <para>A single constant value for the entire curve.</para>
            /// </summary>
            /// <param name="constant">Constant value.</param>
            public MinMaxCurve(float constant)
            {
                this.m_Mode = ParticleSystemCurveMode.Constant;
                this.m_CurveMultiplier = 0f;
                this.m_CurveMin = null;
                this.m_CurveMax = null;
                this.m_ConstantMin = 0f;
                this.m_ConstantMax = constant;
            }

            /// <summary>
            /// <para>Use one curve when evaluating numbers along this Min-Max curve.</para>
            /// </summary>
            /// <param name="scalar">A multiplier to be applied to the curve.</param>
            /// <param name="curve">A single curve for evaluating against.</param>
            /// <param name="multiplier"></param>
            public MinMaxCurve(float multiplier, AnimationCurve curve)
            {
                this.m_Mode = ParticleSystemCurveMode.Curve;
                this.m_CurveMultiplier = multiplier;
                this.m_CurveMin = null;
                this.m_CurveMax = curve;
                this.m_ConstantMin = 0f;
                this.m_ConstantMax = 0f;
            }

            /// <summary>
            /// <para>Randomly select values based on the interval between the minimum and maximum curves.</para>
            /// </summary>
            /// <param name="scalar">A multiplier to be applied to the curves.</param>
            /// <param name="min">The curve describing the minimum values to be evaluated.</param>
            /// <param name="max">The curve describing the maximum values to be evaluated.</param>
            /// <param name="multiplier"></param>
            public MinMaxCurve(float multiplier, AnimationCurve min, AnimationCurve max)
            {
                this.m_Mode = ParticleSystemCurveMode.TwoCurves;
                this.m_CurveMultiplier = multiplier;
                this.m_CurveMin = min;
                this.m_CurveMax = max;
                this.m_ConstantMin = 0f;
                this.m_ConstantMax = 0f;
            }

            /// <summary>
            /// <para>Randomly select values based on the interval between the minimum and maximum constants.</para>
            /// </summary>
            /// <param name="min">The constant describing the minimum values to be evaluated.</param>
            /// <param name="max">The constant describing the maximum values to be evaluated.</param>
            public MinMaxCurve(float min, float max)
            {
                this.m_Mode = ParticleSystemCurveMode.TwoConstants;
                this.m_CurveMultiplier = 0f;
                this.m_CurveMin = null;
                this.m_CurveMax = null;
                this.m_ConstantMin = min;
                this.m_ConstantMax = max;
            }

            /// <summary>
            /// <para>Set the mode that the min-max curve will use to evaluate values.</para>
            /// </summary>
            public ParticleSystemCurveMode mode
            {
                get => 
                    this.m_Mode;
                set
                {
                    this.m_Mode = value;
                }
            }
            [Obsolete("Please use MinMaxCurve.curveMultiplier instead. (UnityUpgradable) -> UnityEngine.ParticleSystem/MinMaxCurve.curveMultiplier")]
            public float curveScalar
            {
                get => 
                    this.m_CurveMultiplier;
                set
                {
                    this.m_CurveMultiplier = value;
                }
            }
            /// <summary>
            /// <para>Set a multiplier to be applied to the curves.</para>
            /// </summary>
            public float curveMultiplier
            {
                get => 
                    this.m_CurveMultiplier;
                set
                {
                    this.m_CurveMultiplier = value;
                }
            }
            /// <summary>
            /// <para>Set a curve for the upper bound.</para>
            /// </summary>
            public AnimationCurve curveMax
            {
                get => 
                    this.m_CurveMax;
                set
                {
                    this.m_CurveMax = value;
                }
            }
            /// <summary>
            /// <para>Set a curve for the lower bound.</para>
            /// </summary>
            public AnimationCurve curveMin
            {
                get => 
                    this.m_CurveMin;
                set
                {
                    this.m_CurveMin = value;
                }
            }
            /// <summary>
            /// <para>Set a constant for the upper bound.</para>
            /// </summary>
            public float constantMax
            {
                get => 
                    this.m_ConstantMax;
                set
                {
                    this.m_ConstantMax = value;
                }
            }
            /// <summary>
            /// <para>Set a constant for the lower bound.</para>
            /// </summary>
            public float constantMin
            {
                get => 
                    this.m_ConstantMin;
                set
                {
                    this.m_ConstantMin = value;
                }
            }
            /// <summary>
            /// <para>Set the constant value.</para>
            /// </summary>
            public float constant
            {
                get => 
                    this.m_ConstantMax;
                set
                {
                    this.m_ConstantMax = value;
                }
            }
            /// <summary>
            /// <para>Set the curve.</para>
            /// </summary>
            public AnimationCurve curve
            {
                get => 
                    this.m_CurveMax;
                set
                {
                    this.m_CurveMax = value;
                }
            }
            /// <summary>
            /// <para>Manually query the curve to calculate values based on what mode it is in.</para>
            /// </summary>
            /// <param name="time">Normalized time (in the range 0 - 1, where 1 represents 100%) at which to evaluate the curve. This is valid when ParticleSystem.MinMaxCurve.mode is set to ParticleSystemCurveMode.Curve or ParticleSystemCurveMode.TwoCurves.</param>
            /// <param name="lerpFactor">Blend between the 2 curves/constants (Valid when ParticleSystem.MinMaxCurve.mode is set to ParticleSystemCurveMode.TwoConstants or ParticleSystemCurveMode.TwoCurves).</param>
            /// <returns>
            /// <para>Calculated curve/constant value.</para>
            /// </returns>
            public float Evaluate(float time) => 
                this.Evaluate(time, 1f);

            /// <summary>
            /// <para>Manually query the curve to calculate values based on what mode it is in.</para>
            /// </summary>
            /// <param name="time">Normalized time (in the range 0 - 1, where 1 represents 100%) at which to evaluate the curve. This is valid when ParticleSystem.MinMaxCurve.mode is set to ParticleSystemCurveMode.Curve or ParticleSystemCurveMode.TwoCurves.</param>
            /// <param name="lerpFactor">Blend between the 2 curves/constants (Valid when ParticleSystem.MinMaxCurve.mode is set to ParticleSystemCurveMode.TwoConstants or ParticleSystemCurveMode.TwoCurves).</param>
            /// <returns>
            /// <para>Calculated curve/constant value.</para>
            /// </returns>
            public float Evaluate(float time, float lerpFactor)
            {
                time = Mathf.Clamp(time, 0f, 1f);
                lerpFactor = Mathf.Clamp(lerpFactor, 0f, 1f);
                if (this.m_Mode == ParticleSystemCurveMode.Constant)
                {
                    return this.m_ConstantMax;
                }
                if (this.m_Mode == ParticleSystemCurveMode.TwoConstants)
                {
                    return Mathf.Lerp(this.m_ConstantMin, this.m_ConstantMax, lerpFactor);
                }
                float b = this.m_CurveMax.Evaluate(time) * this.m_CurveMultiplier;
                if (this.m_Mode == ParticleSystemCurveMode.TwoCurves)
                {
                    return Mathf.Lerp(this.m_CurveMin.Evaluate(time) * this.m_CurveMultiplier, b, lerpFactor);
                }
                return b;
            }

            public static implicit operator ParticleSystem.MinMaxCurve(float constant) => 
                new ParticleSystem.MinMaxCurve(constant);
        }

        /// <summary>
        /// <para>Script interface for a Min-Max Gradient.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MinMaxGradient
        {
            private ParticleSystemGradientMode m_Mode;
            private Gradient m_GradientMin;
            private Gradient m_GradientMax;
            private Color m_ColorMin;
            private Color m_ColorMax;
            /// <summary>
            /// <para>A single constant color for the entire gradient.</para>
            /// </summary>
            /// <param name="color">Constant color.</param>
            public MinMaxGradient(Color color)
            {
                this.m_Mode = ParticleSystemGradientMode.Color;
                this.m_GradientMin = null;
                this.m_GradientMax = null;
                this.m_ColorMin = Color.black;
                this.m_ColorMax = color;
            }

            /// <summary>
            /// <para>Use one gradient when evaluating numbers along this Min-Max gradient.</para>
            /// </summary>
            /// <param name="gradient">A single gradient for evaluating against.</param>
            public MinMaxGradient(Gradient gradient)
            {
                this.m_Mode = ParticleSystemGradientMode.Gradient;
                this.m_GradientMin = null;
                this.m_GradientMax = gradient;
                this.m_ColorMin = Color.black;
                this.m_ColorMax = Color.black;
            }

            /// <summary>
            /// <para>Randomly select colors based on the interval between the minimum and maximum constants.</para>
            /// </summary>
            /// <param name="min">The constant color describing the minimum colors to be evaluated.</param>
            /// <param name="max">The constant color describing the maximum colors to be evaluated.</param>
            public MinMaxGradient(Color min, Color max)
            {
                this.m_Mode = ParticleSystemGradientMode.TwoColors;
                this.m_GradientMin = null;
                this.m_GradientMax = null;
                this.m_ColorMin = min;
                this.m_ColorMax = max;
            }

            /// <summary>
            /// <para>Randomly select colors based on the interval between the minimum and maximum gradients.</para>
            /// </summary>
            /// <param name="min">The gradient describing the minimum colors to be evaluated.</param>
            /// <param name="max">The gradient describing the maximum colors to be evaluated.</param>
            public MinMaxGradient(Gradient min, Gradient max)
            {
                this.m_Mode = ParticleSystemGradientMode.TwoGradients;
                this.m_GradientMin = min;
                this.m_GradientMax = max;
                this.m_ColorMin = Color.black;
                this.m_ColorMax = Color.black;
            }

            /// <summary>
            /// <para>Set the mode that the min-max gradient will use to evaluate colors.</para>
            /// </summary>
            public ParticleSystemGradientMode mode
            {
                get => 
                    this.m_Mode;
                set
                {
                    this.m_Mode = value;
                }
            }
            /// <summary>
            /// <para>Set a gradient for the upper bound.</para>
            /// </summary>
            public Gradient gradientMax
            {
                get => 
                    this.m_GradientMax;
                set
                {
                    this.m_GradientMax = value;
                }
            }
            /// <summary>
            /// <para>Set a gradient for the lower bound.</para>
            /// </summary>
            public Gradient gradientMin
            {
                get => 
                    this.m_GradientMin;
                set
                {
                    this.m_GradientMin = value;
                }
            }
            /// <summary>
            /// <para>Set a constant color for the upper bound.</para>
            /// </summary>
            public Color colorMax
            {
                get => 
                    this.m_ColorMax;
                set
                {
                    this.m_ColorMax = value;
                }
            }
            /// <summary>
            /// <para>Set a constant color for the lower bound.</para>
            /// </summary>
            public Color colorMin
            {
                get => 
                    this.m_ColorMin;
                set
                {
                    this.m_ColorMin = value;
                }
            }
            /// <summary>
            /// <para>Set a constant color.</para>
            /// </summary>
            public Color color
            {
                get => 
                    this.m_ColorMax;
                set
                {
                    this.m_ColorMax = value;
                }
            }
            /// <summary>
            /// <para>Set the gradient.</para>
            /// </summary>
            public Gradient gradient
            {
                get => 
                    this.m_GradientMax;
                set
                {
                    this.m_GradientMax = value;
                }
            }
            /// <summary>
            /// <para>Manually query the gradient to calculate colors based on what mode it is in.</para>
            /// </summary>
            /// <param name="time">Normalized time (in the range 0 - 1, where 1 represents 100%) at which to evaluate the gradient. This is valid when ParticleSystem.MinMaxGradient.mode is set to ParticleSystemGradientMode.Gradient or ParticleSystemGradientMode.TwoGradients.</param>
            /// <param name="lerpFactor">Blend between the 2 gradients/colors (Valid when ParticleSystem.MinMaxGradient.mode is set to ParticleSystemGradientMode.TwoColors or ParticleSystemGradientMode.TwoGradients).</param>
            /// <returns>
            /// <para>Calculated gradient/color value.</para>
            /// </returns>
            public Color Evaluate(float time) => 
                this.Evaluate(time, 1f);

            /// <summary>
            /// <para>Manually query the gradient to calculate colors based on what mode it is in.</para>
            /// </summary>
            /// <param name="time">Normalized time (in the range 0 - 1, where 1 represents 100%) at which to evaluate the gradient. This is valid when ParticleSystem.MinMaxGradient.mode is set to ParticleSystemGradientMode.Gradient or ParticleSystemGradientMode.TwoGradients.</param>
            /// <param name="lerpFactor">Blend between the 2 gradients/colors (Valid when ParticleSystem.MinMaxGradient.mode is set to ParticleSystemGradientMode.TwoColors or ParticleSystemGradientMode.TwoGradients).</param>
            /// <returns>
            /// <para>Calculated gradient/color value.</para>
            /// </returns>
            public Color Evaluate(float time, float lerpFactor)
            {
                time = Mathf.Clamp(time, 0f, 1f);
                lerpFactor = Mathf.Clamp(lerpFactor, 0f, 1f);
                if (this.m_Mode == ParticleSystemGradientMode.Color)
                {
                    return this.m_ColorMax;
                }
                if (this.m_Mode == ParticleSystemGradientMode.TwoColors)
                {
                    return Color.Lerp(this.m_ColorMin, this.m_ColorMax, lerpFactor);
                }
                Color b = this.m_GradientMax.Evaluate(time);
                if (this.m_Mode == ParticleSystemGradientMode.TwoGradients)
                {
                    return Color.Lerp(this.m_GradientMin.Evaluate(time), b, lerpFactor);
                }
                return b;
            }

            public static implicit operator ParticleSystem.MinMaxGradient(Color color) => 
                new ParticleSystem.MinMaxGradient(color);

            public static implicit operator ParticleSystem.MinMaxGradient(Gradient gradient) => 
                new ParticleSystem.MinMaxGradient(gradient);
        }

        /// <summary>
        /// <para>Script interface for the Noise module.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct NoiseModule
        {
            private ParticleSystem m_ParticleSystem;
            internal NoiseModule(ParticleSystem particleSystem)
            {
                this.m_ParticleSystem = particleSystem;
            }

            /// <summary>
            /// <para>Enable/disable the Noise module.</para>
            /// </summary>
            public bool enabled
            {
                get => 
                    GetEnabled(this.m_ParticleSystem);
                set
                {
                    SetEnabled(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Control the noise separately for each axis.</para>
            /// </summary>
            public bool separateAxes
            {
                get => 
                    GetSeparateAxes(this.m_ParticleSystem);
                set
                {
                    SetSeparateAxes(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>How strong the overall noise effect is.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve strength
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetStrengthX(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetStrengthX(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Strength multiplier.</para>
            /// </summary>
            public float strengthMultiplier
            {
                get => 
                    GetStrengthXMultiplier(this.m_ParticleSystem);
                set
                {
                    SetStrengthXMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Define the strength of the effect on the X axis, when using the ParticleSystem.NoiseModule.separateAxes option.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve strengthX
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetStrengthX(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetStrengthX(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>X axis strength multiplier.</para>
            /// </summary>
            public float strengthXMultiplier
            {
                get => 
                    GetStrengthXMultiplier(this.m_ParticleSystem);
                set
                {
                    SetStrengthXMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Define the strength of the effect on the Y axis, when using the ParticleSystem.NoiseModule.separateAxes option.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve strengthY
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetStrengthY(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetStrengthY(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Y axis strength multiplier.</para>
            /// </summary>
            public float strengthYMultiplier
            {
                get => 
                    GetStrengthYMultiplier(this.m_ParticleSystem);
                set
                {
                    SetStrengthYMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Define the strength of the effect on the Z axis, when using the ParticleSystem.NoiseModule.separateAxes option.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve strengthZ
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetStrengthZ(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetStrengthZ(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Z axis strength multiplier.</para>
            /// </summary>
            public float strengthZMultiplier
            {
                get => 
                    GetStrengthZMultiplier(this.m_ParticleSystem);
                set
                {
                    SetStrengthZMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Low values create soft, smooth noise, and high values create rapidly changing noise.</para>
            /// </summary>
            public float frequency
            {
                get => 
                    GetFrequency(this.m_ParticleSystem);
                set
                {
                    SetFrequency(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Higher frequency noise will reduce the strength by a proportional amount, if enabled.</para>
            /// </summary>
            public bool damping
            {
                get => 
                    GetDamping(this.m_ParticleSystem);
                set
                {
                    SetDamping(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Layers of noise that combine to produce final noise.</para>
            /// </summary>
            public int octaveCount
            {
                get => 
                    GetOctaveCount(this.m_ParticleSystem);
                set
                {
                    SetOctaveCount(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>When combining each octave, scale the intensity by this amount.</para>
            /// </summary>
            public float octaveMultiplier
            {
                get => 
                    GetOctaveMultiplier(this.m_ParticleSystem);
                set
                {
                    SetOctaveMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>When combining each octave, zoom in by this amount.</para>
            /// </summary>
            public float octaveScale
            {
                get => 
                    GetOctaveScale(this.m_ParticleSystem);
                set
                {
                    SetOctaveScale(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Generate 1D, 2D or 3D noise.</para>
            /// </summary>
            public ParticleSystemNoiseQuality quality
            {
                get => 
                    ((ParticleSystemNoiseQuality) GetQuality(this.m_ParticleSystem));
                set
                {
                    SetQuality(this.m_ParticleSystem, (int) value);
                }
            }
            /// <summary>
            /// <para>Scroll the noise map over the particle system.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve scrollSpeed
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetScrollSpeed(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetScrollSpeed(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Scroll speed multiplier.</para>
            /// </summary>
            public float scrollSpeedMultiplier
            {
                get => 
                    GetScrollSpeedMultiplier(this.m_ParticleSystem);
                set
                {
                    SetScrollSpeedMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Enable remapping of the final noise values, allowing for noise values to be translated into different values.</para>
            /// </summary>
            public bool remapEnabled
            {
                get => 
                    GetRemapEnabled(this.m_ParticleSystem);
                set
                {
                    SetRemapEnabled(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Define how the noise values are remapped.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve remap
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetRemapX(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetRemapX(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Remap multiplier.</para>
            /// </summary>
            public float remapMultiplier
            {
                get => 
                    GetRemapXMultiplier(this.m_ParticleSystem);
                set
                {
                    SetRemapXMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Define how the noise values are remapped on the X axis, when using the ParticleSystem.NoiseModule.separateAxes option.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve remapX
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetRemapX(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetRemapX(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>X axis remap multiplier.</para>
            /// </summary>
            public float remapXMultiplier
            {
                get => 
                    GetRemapXMultiplier(this.m_ParticleSystem);
                set
                {
                    SetRemapXMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Define how the noise values are remapped on the Y axis, when using the ParticleSystem.NoiseModule.separateAxes option.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve remapY
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetRemapY(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetRemapY(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Y axis remap multiplier.</para>
            /// </summary>
            public float remapYMultiplier
            {
                get => 
                    GetRemapYMultiplier(this.m_ParticleSystem);
                set
                {
                    SetRemapYMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Define how the noise values are remapped on the Z axis, when using the ParticleSystem.NoiseModule.separateAxes option.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve remapZ
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetRemapZ(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetRemapZ(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Z axis remap multiplier.</para>
            /// </summary>
            public float remapZMultiplier
            {
                get => 
                    GetRemapZMultiplier(this.m_ParticleSystem);
                set
                {
                    SetRemapZMultiplier(this.m_ParticleSystem, value);
                }
            }
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetEnabled(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetEnabled(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetSeparateAxes(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetSeparateAxes(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetStrengthX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetStrengthX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetStrengthY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetStrengthY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetStrengthZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetStrengthZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetStrengthXMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetStrengthXMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetStrengthYMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetStrengthYMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetStrengthZMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetStrengthZMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetFrequency(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetFrequency(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetDamping(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetDamping(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetOctaveCount(ParticleSystem system, int value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern int GetOctaveCount(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetOctaveMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetOctaveMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetOctaveScale(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetOctaveScale(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetQuality(ParticleSystem system, int value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern int GetQuality(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetScrollSpeed(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetScrollSpeed(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetScrollSpeedMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetScrollSpeedMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetRemapEnabled(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetRemapEnabled(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetRemapX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetRemapX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetRemapY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetRemapY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetRemapZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetRemapZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetRemapXMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetRemapXMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetRemapYMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetRemapYMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetRemapZMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetRemapZMultiplier(ParticleSystem system);
        }

        /// <summary>
        /// <para>Script interface for a Particle.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential), RequiredByNativeCode("particleSystemParticle", Optional=true)]
        public struct Particle
        {
            private Vector3 m_Position;
            private Vector3 m_Velocity;
            private Vector3 m_AnimatedVelocity;
            private Vector3 m_InitialVelocity;
            private Vector3 m_AxisOfRotation;
            private Vector3 m_Rotation;
            private Vector3 m_AngularVelocity;
            private Vector3 m_StartSize;
            private Color32 m_StartColor;
            private uint m_RandomSeed;
            private float m_Lifetime;
            private float m_StartLifetime;
            private float m_EmitAccumulator0;
            private float m_EmitAccumulator1;
            /// <summary>
            /// <para>The position of the particle.</para>
            /// </summary>
            public Vector3 position
            {
                get => 
                    this.m_Position;
                set
                {
                    this.m_Position = value;
                }
            }
            /// <summary>
            /// <para>The velocity of the particle.</para>
            /// </summary>
            public Vector3 velocity
            {
                get => 
                    this.m_Velocity;
                set
                {
                    this.m_Velocity = value;
                }
            }
            /// <summary>
            /// <para>The lifetime of the particle.</para>
            /// </summary>
            [Obsolete("Please use Particle.remainingLifetime instead. (UnityUpgradable) -> UnityEngine.ParticleSystem/Particle.remainingLifetime")]
            public float lifetime
            {
                get => 
                    this.m_Lifetime;
                set
                {
                    this.m_Lifetime = value;
                }
            }
            /// <summary>
            /// <para>The remaining lifetime of the particle.</para>
            /// </summary>
            public float remainingLifetime
            {
                get => 
                    this.m_Lifetime;
                set
                {
                    this.m_Lifetime = value;
                }
            }
            /// <summary>
            /// <para>The starting lifetime of the particle.</para>
            /// </summary>
            public float startLifetime
            {
                get => 
                    this.m_StartLifetime;
                set
                {
                    this.m_StartLifetime = value;
                }
            }
            /// <summary>
            /// <para>The initial size of the particle. The current size of the particle is calculated procedurally based on this value and the active size modules.</para>
            /// </summary>
            public float startSize
            {
                get => 
                    this.m_StartSize.x;
                set
                {
                    this.m_StartSize = new Vector3(value, value, value);
                }
            }
            /// <summary>
            /// <para>The initial 3D size of the particle. The current size of the particle is calculated procedurally based on this value and the active size modules.</para>
            /// </summary>
            public Vector3 startSize3D
            {
                get => 
                    this.m_StartSize;
                set
                {
                    this.m_StartSize = value;
                }
            }
            public Vector3 axisOfRotation
            {
                get => 
                    this.m_AxisOfRotation;
                set
                {
                    this.m_AxisOfRotation = value;
                }
            }
            /// <summary>
            /// <para>The rotation of the particle.</para>
            /// </summary>
            public float rotation
            {
                get => 
                    (this.m_Rotation.z * 57.29578f);
                set
                {
                    this.m_Rotation = new Vector3(0f, 0f, value * 0.01745329f);
                }
            }
            /// <summary>
            /// <para>The 3D rotation of the particle.</para>
            /// </summary>
            public Vector3 rotation3D
            {
                get => 
                    ((Vector3) (this.m_Rotation * 57.29578f));
                set
                {
                    this.m_Rotation = (Vector3) (value * 0.01745329f);
                }
            }
            /// <summary>
            /// <para>The angular velocity of the particle.</para>
            /// </summary>
            public float angularVelocity
            {
                get => 
                    (this.m_AngularVelocity.z * 57.29578f);
                set
                {
                    this.m_AngularVelocity.z = value * 0.01745329f;
                }
            }
            /// <summary>
            /// <para>The 3D angular velocity of the particle.</para>
            /// </summary>
            public Vector3 angularVelocity3D
            {
                get => 
                    ((Vector3) (this.m_AngularVelocity * 57.29578f));
                set
                {
                    this.m_AngularVelocity = (Vector3) (value * 0.01745329f);
                }
            }
            /// <summary>
            /// <para>The initial color of the particle. The current color of the particle is calculated procedurally based on this value and the active color modules.</para>
            /// </summary>
            public Color32 startColor
            {
                get => 
                    this.m_StartColor;
                set
                {
                    this.m_StartColor = value;
                }
            }
            /// <summary>
            /// <para>The random value of the particle.</para>
            /// </summary>
            [Obsolete("randomValue property is deprecated. Use randomSeed instead to control random behavior of particles.")]
            public float randomValue
            {
                get => 
                    BitConverter.ToSingle(BitConverter.GetBytes(this.m_RandomSeed), 0);
                set
                {
                    this.m_RandomSeed = BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);
                }
            }
            /// <summary>
            /// <para>The random seed of the particle.</para>
            /// </summary>
            public uint randomSeed
            {
                get => 
                    this.m_RandomSeed;
                set
                {
                    this.m_RandomSeed = value;
                }
            }
            /// <summary>
            /// <para>Calculate the current size of the particle by applying the relevant curves to its startSize property.</para>
            /// </summary>
            /// <param name="system">The particle system from which this particle was emitted.</param>
            /// <returns>
            /// <para>Current size.</para>
            /// </returns>
            public float GetCurrentSize(ParticleSystem system) => 
                GetCurrentSize(system, ref this);

            /// <summary>
            /// <para>Calculate the current 3D size of the particle by applying the relevant curves to its startSize3D property.</para>
            /// </summary>
            /// <param name="system">The particle system from which this particle was emitted.</param>
            /// <returns>
            /// <para>Current size.</para>
            /// </returns>
            public Vector3 GetCurrentSize3D(ParticleSystem system) => 
                GetCurrentSize3D(system, ref this);

            /// <summary>
            /// <para>Calculate the current color of the particle by applying the relevant curves to its startColor property.</para>
            /// </summary>
            /// <param name="system">The particle system from which this particle was emitted.</param>
            /// <returns>
            /// <para>Current color.</para>
            /// </returns>
            public Color32 GetCurrentColor(ParticleSystem system) => 
                GetCurrentColor(system, ref this);

            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetCurrentSize(ParticleSystem system, ref ParticleSystem.Particle particle);
            private static Vector3 GetCurrentSize3D(ParticleSystem system, ref ParticleSystem.Particle particle)
            {
                Vector3 vector;
                INTERNAL_CALL_GetCurrentSize3D(system, ref particle, out vector);
                return vector;
            }

            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void INTERNAL_CALL_GetCurrentSize3D(ParticleSystem system, ref ParticleSystem.Particle particle, out Vector3 value);
            private static Color32 GetCurrentColor(ParticleSystem system, ref ParticleSystem.Particle particle)
            {
                Color32 color;
                INTERNAL_CALL_GetCurrentColor(system, ref particle, out color);
                return color;
            }

            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void INTERNAL_CALL_GetCurrentColor(ParticleSystem system, ref ParticleSystem.Particle particle, out Color32 value);
            [Obsolete("size property is deprecated. Use startSize or GetCurrentSize() instead.")]
            public float size
            {
                get => 
                    this.m_StartSize.x;
                set
                {
                    this.m_StartSize = new Vector3(value, value, value);
                }
            }
            [Obsolete("color property is deprecated. Use startColor or GetCurrentColor() instead.")]
            public Color32 color
            {
                get => 
                    this.m_StartColor;
                set
                {
                    this.m_StartColor = value;
                }
            }
        }

        /// <summary>
        /// <para>Script interface for the Rotation By Speed module.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RotationBySpeedModule
        {
            private ParticleSystem m_ParticleSystem;
            internal RotationBySpeedModule(ParticleSystem particleSystem)
            {
                this.m_ParticleSystem = particleSystem;
            }

            /// <summary>
            /// <para>Enable/disable the Rotation By Speed module.</para>
            /// </summary>
            public bool enabled
            {
                get => 
                    GetEnabled(this.m_ParticleSystem);
                set
                {
                    SetEnabled(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Rotation by speed curve for the X axis.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve x
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetX(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetX(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Speed multiplier along the X axis.</para>
            /// </summary>
            public float xMultiplier
            {
                get => 
                    GetXMultiplier(this.m_ParticleSystem);
                set
                {
                    SetXMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Rotation by speed curve for the Y axis.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve y
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetY(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetY(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Speed multiplier along the Y axis.</para>
            /// </summary>
            public float yMultiplier
            {
                get => 
                    GetYMultiplier(this.m_ParticleSystem);
                set
                {
                    SetYMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Rotation by speed curve for the Z axis.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve z
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetZ(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetZ(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Speed multiplier along the Z axis.</para>
            /// </summary>
            public float zMultiplier
            {
                get => 
                    GetZMultiplier(this.m_ParticleSystem);
                set
                {
                    SetZMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Set the rotation by speed on each axis separately.</para>
            /// </summary>
            public bool separateAxes
            {
                get => 
                    GetSeparateAxes(this.m_ParticleSystem);
                set
                {
                    SetSeparateAxes(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Apply the rotation curve between these minimum and maximum speeds.</para>
            /// </summary>
            public Vector2 range
            {
                get => 
                    GetRange(this.m_ParticleSystem);
                set
                {
                    SetRange(this.m_ParticleSystem, value);
                }
            }
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetEnabled(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetEnabled(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetXMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetXMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetYMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetYMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetZMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetZMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetSeparateAxes(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetSeparateAxes(ParticleSystem system);
            private static void SetRange(ParticleSystem system, Vector2 value)
            {
                INTERNAL_CALL_SetRange(system, ref value);
            }

            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void INTERNAL_CALL_SetRange(ParticleSystem system, ref Vector2 value);
            private static Vector2 GetRange(ParticleSystem system)
            {
                Vector2 vector;
                INTERNAL_CALL_GetRange(system, out vector);
                return vector;
            }

            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void INTERNAL_CALL_GetRange(ParticleSystem system, out Vector2 value);
        }

        /// <summary>
        /// <para>Script interface for the Rotation Over Lifetime module.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RotationOverLifetimeModule
        {
            private ParticleSystem m_ParticleSystem;
            internal RotationOverLifetimeModule(ParticleSystem particleSystem)
            {
                this.m_ParticleSystem = particleSystem;
            }

            /// <summary>
            /// <para>Enable/disable the Rotation Over Lifetime module.</para>
            /// </summary>
            public bool enabled
            {
                get => 
                    GetEnabled(this.m_ParticleSystem);
                set
                {
                    SetEnabled(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Rotation over lifetime curve for the X axis.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve x
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetX(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetX(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Rotation multiplier around the X axis.</para>
            /// </summary>
            public float xMultiplier
            {
                get => 
                    GetXMultiplier(this.m_ParticleSystem);
                set
                {
                    SetXMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Rotation over lifetime curve for the Y axis.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve y
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetY(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetY(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Rotation multiplier around the Y axis.</para>
            /// </summary>
            public float yMultiplier
            {
                get => 
                    GetYMultiplier(this.m_ParticleSystem);
                set
                {
                    SetYMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Rotation over lifetime curve for the Z axis.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve z
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetZ(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetZ(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Rotation multiplier around the Z axis.</para>
            /// </summary>
            public float zMultiplier
            {
                get => 
                    GetZMultiplier(this.m_ParticleSystem);
                set
                {
                    SetZMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Set the rotation over lifetime on each axis separately.</para>
            /// </summary>
            public bool separateAxes
            {
                get => 
                    GetSeparateAxes(this.m_ParticleSystem);
                set
                {
                    SetSeparateAxes(this.m_ParticleSystem, value);
                }
            }
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetEnabled(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetEnabled(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetXMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetXMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetYMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetYMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetZMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetZMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetSeparateAxes(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetSeparateAxes(ParticleSystem system);
        }

        /// <summary>
        /// <para>Script interface for the Shape module.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ShapeModule
        {
            private ParticleSystem m_ParticleSystem;
            internal ShapeModule(ParticleSystem particleSystem)
            {
                this.m_ParticleSystem = particleSystem;
            }

            /// <summary>
            /// <para>Enable/disable the Shape module.</para>
            /// </summary>
            public bool enabled
            {
                get => 
                    GetEnabled(this.m_ParticleSystem);
                set
                {
                    SetEnabled(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Type of shape to emit particles from.</para>
            /// </summary>
            public ParticleSystemShapeType shapeType
            {
                get => 
                    ((ParticleSystemShapeType) GetShapeType(this.m_ParticleSystem));
                set
                {
                    SetShapeType(this.m_ParticleSystem, (int) value);
                }
            }
            /// <summary>
            /// <para>Randomizes the starting direction of particles.</para>
            /// </summary>
            public float randomDirectionAmount
            {
                get => 
                    GetRandomDirectionAmount(this.m_ParticleSystem);
                set
                {
                    SetRandomDirectionAmount(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Spherizes the starting direction of particles.</para>
            /// </summary>
            public float sphericalDirectionAmount
            {
                get => 
                    GetSphericalDirectionAmount(this.m_ParticleSystem);
                set
                {
                    SetSphericalDirectionAmount(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Set particles to face their initial direction of travel.</para>
            /// </summary>
            public bool alignToDirection
            {
                get => 
                    GetAlignToDirection(this.m_ParticleSystem);
                set
                {
                    SetAlignToDirection(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Radius of the shape.</para>
            /// </summary>
            public float radius
            {
                get => 
                    GetRadius(this.m_ParticleSystem);
                set
                {
                    SetRadius(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>The mode used for generating particles along the radius.</para>
            /// </summary>
            public ParticleSystemShapeMultiModeValue radiusMode
            {
                get => 
                    ((ParticleSystemShapeMultiModeValue) GetRadiusMode(this.m_ParticleSystem));
                set
                {
                    SetRadiusMode(this.m_ParticleSystem, (int) value);
                }
            }
            /// <summary>
            /// <para>Control the gap between emission points along the radius.</para>
            /// </summary>
            public float radiusSpread
            {
                get => 
                    GetRadiusSpread(this.m_ParticleSystem);
                set
                {
                    SetRadiusSpread(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>When using one of the animated modes, how quickly to move the emission position along the radius.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve radiusSpeed
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetRadiusSpeed(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetRadiusSpeed(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>A multiplier of the radius speed of the emission shape.</para>
            /// </summary>
            public float radiusSpeedMultiplier
            {
                get => 
                    GetRadiusSpeedMultiplier(this.m_ParticleSystem);
                set
                {
                    SetRadiusSpeedMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Angle of the cone.</para>
            /// </summary>
            public float angle
            {
                get => 
                    GetAngle(this.m_ParticleSystem);
                set
                {
                    SetAngle(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Length of the cone.</para>
            /// </summary>
            public float length
            {
                get => 
                    GetLength(this.m_ParticleSystem);
                set
                {
                    SetLength(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Scale of the box.</para>
            /// </summary>
            public Vector3 box
            {
                get => 
                    GetBox(this.m_ParticleSystem);
                set
                {
                    SetBox(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Where on the mesh to emit particles from.</para>
            /// </summary>
            public ParticleSystemMeshShapeType meshShapeType
            {
                get => 
                    ((ParticleSystemMeshShapeType) GetMeshShapeType(this.m_ParticleSystem));
                set
                {
                    SetMeshShapeType(this.m_ParticleSystem, (int) value);
                }
            }
            /// <summary>
            /// <para>Mesh to emit particles from.</para>
            /// </summary>
            public Mesh mesh
            {
                get => 
                    GetMesh(this.m_ParticleSystem);
                set
                {
                    SetMesh(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>MeshRenderer to emit particles from.</para>
            /// </summary>
            public MeshRenderer meshRenderer
            {
                get => 
                    GetMeshRenderer(this.m_ParticleSystem);
                set
                {
                    SetMeshRenderer(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>SkinnedMeshRenderer to emit particles from.</para>
            /// </summary>
            public SkinnedMeshRenderer skinnedMeshRenderer
            {
                get => 
                    GetSkinnedMeshRenderer(this.m_ParticleSystem);
                set
                {
                    SetSkinnedMeshRenderer(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Emit from a single material, or the whole mesh.</para>
            /// </summary>
            public bool useMeshMaterialIndex
            {
                get => 
                    GetUseMeshMaterialIndex(this.m_ParticleSystem);
                set
                {
                    SetUseMeshMaterialIndex(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Emit particles from a single material of a mesh.</para>
            /// </summary>
            public int meshMaterialIndex
            {
                get => 
                    GetMeshMaterialIndex(this.m_ParticleSystem);
                set
                {
                    SetMeshMaterialIndex(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Modulate the particle colors with the vertex colors, or the material color if no vertex colors exist.</para>
            /// </summary>
            public bool useMeshColors
            {
                get => 
                    GetUseMeshColors(this.m_ParticleSystem);
                set
                {
                    SetUseMeshColors(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Move particles away from the surface of the source mesh.</para>
            /// </summary>
            public float normalOffset
            {
                get => 
                    GetNormalOffset(this.m_ParticleSystem);
                set
                {
                    SetNormalOffset(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Apply a scaling factor to the mesh used for generating source positions.</para>
            /// </summary>
            public float meshScale
            {
                get => 
                    GetMeshScale(this.m_ParticleSystem);
                set
                {
                    SetMeshScale(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Circle arc angle.</para>
            /// </summary>
            public float arc
            {
                get => 
                    GetArc(this.m_ParticleSystem);
                set
                {
                    SetArc(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>The mode used for generating particles around the arc.</para>
            /// </summary>
            public ParticleSystemShapeMultiModeValue arcMode
            {
                get => 
                    ((ParticleSystemShapeMultiModeValue) GetArcMode(this.m_ParticleSystem));
                set
                {
                    SetArcMode(this.m_ParticleSystem, (int) value);
                }
            }
            /// <summary>
            /// <para>Control the gap between emission points around the arc.</para>
            /// </summary>
            public float arcSpread
            {
                get => 
                    GetArcSpread(this.m_ParticleSystem);
                set
                {
                    SetArcSpread(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>When using one of the animated modes, how quickly to move the emission position around the arc.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve arcSpeed
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetArcSpeed(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetArcSpeed(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>A multiplier of the arc speed of the emission shape.</para>
            /// </summary>
            public float arcSpeedMultiplier
            {
                get => 
                    GetArcSpeedMultiplier(this.m_ParticleSystem);
                set
                {
                    SetArcSpeedMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Randomizes the starting direction of particles.</para>
            /// </summary>
            [Obsolete("randomDirection property is deprecated. Use randomDirectionAmount instead.")]
            public bool randomDirection
            {
                get => 
                    (GetRandomDirectionAmount(this.m_ParticleSystem) >= 0.5f);
                set
                {
                    SetRandomDirectionAmount(this.m_ParticleSystem, 1f);
                }
            }
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetEnabled(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetEnabled(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetShapeType(ParticleSystem system, int value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern int GetShapeType(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetRandomDirectionAmount(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetRandomDirectionAmount(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetSphericalDirectionAmount(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetSphericalDirectionAmount(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetAlignToDirection(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetAlignToDirection(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetRadius(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetRadius(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetRadiusMode(ParticleSystem system, int value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern int GetRadiusMode(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetRadiusSpread(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetRadiusSpread(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetRadiusSpeed(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetRadiusSpeed(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetRadiusSpeedMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetRadiusSpeedMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetAngle(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetAngle(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetLength(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetLength(ParticleSystem system);
            private static void SetBox(ParticleSystem system, Vector3 value)
            {
                INTERNAL_CALL_SetBox(system, ref value);
            }

            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void INTERNAL_CALL_SetBox(ParticleSystem system, ref Vector3 value);
            private static Vector3 GetBox(ParticleSystem system)
            {
                Vector3 vector;
                INTERNAL_CALL_GetBox(system, out vector);
                return vector;
            }

            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void INTERNAL_CALL_GetBox(ParticleSystem system, out Vector3 value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetMeshShapeType(ParticleSystem system, int value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern int GetMeshShapeType(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetMesh(ParticleSystem system, Mesh value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern Mesh GetMesh(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetMeshRenderer(ParticleSystem system, MeshRenderer value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern MeshRenderer GetMeshRenderer(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetSkinnedMeshRenderer(ParticleSystem system, SkinnedMeshRenderer value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern SkinnedMeshRenderer GetSkinnedMeshRenderer(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetUseMeshMaterialIndex(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetUseMeshMaterialIndex(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetMeshMaterialIndex(ParticleSystem system, int value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern int GetMeshMaterialIndex(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetUseMeshColors(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetUseMeshColors(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetNormalOffset(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetNormalOffset(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetMeshScale(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetMeshScale(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetArc(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetArc(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetArcMode(ParticleSystem system, int value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern int GetArcMode(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetArcSpread(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetArcSpread(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetArcSpeed(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetArcSpeed(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetArcSpeedMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetArcSpeedMultiplier(ParticleSystem system);
        }

        /// <summary>
        /// <para>Script interface for the Size By Speed module.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SizeBySpeedModule
        {
            private ParticleSystem m_ParticleSystem;
            internal SizeBySpeedModule(ParticleSystem particleSystem)
            {
                this.m_ParticleSystem = particleSystem;
            }

            /// <summary>
            /// <para>Enable/disable the Size By Speed module.</para>
            /// </summary>
            public bool enabled
            {
                get => 
                    GetEnabled(this.m_ParticleSystem);
                set
                {
                    SetEnabled(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Curve to control particle size based on speed.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve size
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetX(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetX(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Size multiplier.</para>
            /// </summary>
            public float sizeMultiplier
            {
                get => 
                    GetXMultiplier(this.m_ParticleSystem);
                set
                {
                    SetXMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Size by speed curve for the X axis.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve x
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetX(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetX(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>X axis size multiplier.</para>
            /// </summary>
            public float xMultiplier
            {
                get => 
                    GetXMultiplier(this.m_ParticleSystem);
                set
                {
                    SetXMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Size by speed curve for the Y axis.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve y
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetY(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetY(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Y axis size multiplier.</para>
            /// </summary>
            public float yMultiplier
            {
                get => 
                    GetYMultiplier(this.m_ParticleSystem);
                set
                {
                    SetYMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Size by speed curve for the Z axis.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve z
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetZ(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetZ(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Z axis size multiplier.</para>
            /// </summary>
            public float zMultiplier
            {
                get => 
                    GetZMultiplier(this.m_ParticleSystem);
                set
                {
                    SetZMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Set the size by speed on each axis separately.</para>
            /// </summary>
            public bool separateAxes
            {
                get => 
                    GetSeparateAxes(this.m_ParticleSystem);
                set
                {
                    SetSeparateAxes(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Apply the size curve between these minimum and maximum speeds.</para>
            /// </summary>
            public Vector2 range
            {
                get => 
                    GetRange(this.m_ParticleSystem);
                set
                {
                    SetRange(this.m_ParticleSystem, value);
                }
            }
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetEnabled(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetEnabled(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetXMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetXMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetYMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetYMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetZMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetZMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetSeparateAxes(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetSeparateAxes(ParticleSystem system);
            private static void SetRange(ParticleSystem system, Vector2 value)
            {
                INTERNAL_CALL_SetRange(system, ref value);
            }

            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void INTERNAL_CALL_SetRange(ParticleSystem system, ref Vector2 value);
            private static Vector2 GetRange(ParticleSystem system)
            {
                Vector2 vector;
                INTERNAL_CALL_GetRange(system, out vector);
                return vector;
            }

            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void INTERNAL_CALL_GetRange(ParticleSystem system, out Vector2 value);
        }

        /// <summary>
        /// <para>Script interface for the Size Over Lifetime module.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SizeOverLifetimeModule
        {
            private ParticleSystem m_ParticleSystem;
            internal SizeOverLifetimeModule(ParticleSystem particleSystem)
            {
                this.m_ParticleSystem = particleSystem;
            }

            /// <summary>
            /// <para>Enable/disable the Size Over Lifetime module.</para>
            /// </summary>
            public bool enabled
            {
                get => 
                    GetEnabled(this.m_ParticleSystem);
                set
                {
                    SetEnabled(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Curve to control particle size based on lifetime.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve size
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetX(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetX(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Size multiplier.</para>
            /// </summary>
            public float sizeMultiplier
            {
                get => 
                    GetXMultiplier(this.m_ParticleSystem);
                set
                {
                    SetXMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Size over lifetime curve for the X axis.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve x
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetX(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetX(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>X axis size multiplier.</para>
            /// </summary>
            public float xMultiplier
            {
                get => 
                    GetXMultiplier(this.m_ParticleSystem);
                set
                {
                    SetXMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Size over lifetime curve for the Y axis.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve y
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetY(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetY(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Y axis size multiplier.</para>
            /// </summary>
            public float yMultiplier
            {
                get => 
                    GetYMultiplier(this.m_ParticleSystem);
                set
                {
                    SetYMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Size over lifetime curve for the Z axis.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve z
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetZ(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetZ(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Z axis size multiplier.</para>
            /// </summary>
            public float zMultiplier
            {
                get => 
                    GetZMultiplier(this.m_ParticleSystem);
                set
                {
                    SetZMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Set the size over lifetime on each axis separately.</para>
            /// </summary>
            public bool separateAxes
            {
                get => 
                    GetSeparateAxes(this.m_ParticleSystem);
                set
                {
                    SetSeparateAxes(this.m_ParticleSystem, value);
                }
            }
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetEnabled(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetEnabled(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetXMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetXMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetYMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetYMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetZMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetZMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetSeparateAxes(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetSeparateAxes(ParticleSystem system);
        }

        /// <summary>
        /// <para>Script interface for the Sub Emitters module.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SubEmittersModule
        {
            private ParticleSystem m_ParticleSystem;
            internal SubEmittersModule(ParticleSystem particleSystem)
            {
                this.m_ParticleSystem = particleSystem;
            }

            /// <summary>
            /// <para>Enable/disable the Sub Emitters module.</para>
            /// </summary>
            public bool enabled
            {
                get => 
                    GetEnabled(this.m_ParticleSystem);
                set
                {
                    SetEnabled(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>The total number of sub-emitters.</para>
            /// </summary>
            public int subEmittersCount =>
                GetSubEmittersCount(this.m_ParticleSystem);
            /// <summary>
            /// <para>Add a new sub-emitter.</para>
            /// </summary>
            /// <param name="subEmitter">The sub-emitter to be added.</param>
            /// <param name="type">The event that creates new particles.</param>
            /// <param name="properties">The properties of the new particles.</param>
            public void AddSubEmitter(ParticleSystem subEmitter, ParticleSystemSubEmitterType type, ParticleSystemSubEmitterProperties properties)
            {
                AddSubEmitter(this.m_ParticleSystem, subEmitter, (int) type, (int) properties);
            }

            /// <summary>
            /// <para>Remove a sub-emitter from the given index in the array.</para>
            /// </summary>
            /// <param name="index">The index from which to remove a sub-emitter.</param>
            public void RemoveSubEmitter(int index)
            {
                RemoveSubEmitter(this.m_ParticleSystem, index);
            }

            /// <summary>
            /// <para>Set the Particle System to use as the sub-emitter at the given index.</para>
            /// </summary>
            /// <param name="index">The index of the sub-emitter being modified.</param>
            /// <param name="subEmitter">The Particle System being used as this sub-emitter.</param>
            public void SetSubEmitterSystem(int index, ParticleSystem subEmitter)
            {
                SetSubEmitterSystem(this.m_ParticleSystem, index, subEmitter);
            }

            /// <summary>
            /// <para>Set the type of the sub-emitter at the given index.</para>
            /// </summary>
            /// <param name="index">The index of the sub-emitter being modified.</param>
            /// <param name="type">The new spawning type to assign to this sub-emitter.</param>
            public void SetSubEmitterType(int index, ParticleSystemSubEmitterType type)
            {
                SetSubEmitterType(this.m_ParticleSystem, index, (int) type);
            }

            /// <summary>
            /// <para>Set the properties of the sub-emitter at the given index.</para>
            /// </summary>
            /// <param name="index">The index of the sub-emitter being modified.</param>
            /// <param name="properties">The new properties to assign to this sub-emitter.</param>
            public void SetSubEmitterProperties(int index, ParticleSystemSubEmitterProperties properties)
            {
                SetSubEmitterProperties(this.m_ParticleSystem, index, (int) properties);
            }

            /// <summary>
            /// <para>Get the sub-emitter Particle System at the given index.</para>
            /// </summary>
            /// <param name="index">The index of the desired sub-emitter.</param>
            /// <returns>
            /// <para>The sub-emitter being requested.</para>
            /// </returns>
            public ParticleSystem GetSubEmitterSystem(int index) => 
                GetSubEmitterSystem(this.m_ParticleSystem, index);

            /// <summary>
            /// <para>Get the type of the sub-emitter at the given index.</para>
            /// </summary>
            /// <param name="index">The index of the desired sub-emitter.</param>
            /// <returns>
            /// <para>The type of the requested sub-emitter.</para>
            /// </returns>
            public ParticleSystemSubEmitterType GetSubEmitterType(int index) => 
                ((ParticleSystemSubEmitterType) GetSubEmitterType(this.m_ParticleSystem, index));

            /// <summary>
            /// <para>Get the properties of the sub-emitter at the given index.</para>
            /// </summary>
            /// <param name="index">The index of the desired sub-emitter.</param>
            /// <returns>
            /// <para>The properties of the requested sub-emitter.</para>
            /// </returns>
            public ParticleSystemSubEmitterProperties GetSubEmitterProperties(int index) => 
                ((ParticleSystemSubEmitterProperties) GetSubEmitterProperties(this.m_ParticleSystem, index));

            /// <summary>
            /// <para>Sub particle system which spawns at the locations of the birth of the particles from the parent system.</para>
            /// </summary>
            [Obsolete("birth0 property is deprecated. Use AddSubEmitter, RemoveSubEmitter, SetSubEmitterSystem and GetSubEmitterSystem instead.")]
            public ParticleSystem birth0
            {
                get => 
                    GetBirth(this.m_ParticleSystem, 0);
                set
                {
                    SetBirth(this.m_ParticleSystem, 0, value);
                }
            }
            /// <summary>
            /// <para>Sub particle system which spawns at the locations of the birth of the particles from the parent system.</para>
            /// </summary>
            [Obsolete("birth1 property is deprecated. Use AddSubEmitter, RemoveSubEmitter, SetSubEmitterSystem and GetSubEmitterSystem instead.")]
            public ParticleSystem birth1
            {
                get => 
                    GetBirth(this.m_ParticleSystem, 1);
                set
                {
                    SetBirth(this.m_ParticleSystem, 1, value);
                }
            }
            /// <summary>
            /// <para>Sub particle system which spawns at the locations of the collision of the particles from the parent system.</para>
            /// </summary>
            [Obsolete("collision0 property is deprecated. Use AddSubEmitter, RemoveSubEmitter, SetSubEmitterSystem and GetSubEmitterSystem instead.")]
            public ParticleSystem collision0
            {
                get => 
                    GetCollision(this.m_ParticleSystem, 0);
                set
                {
                    SetCollision(this.m_ParticleSystem, 0, value);
                }
            }
            /// <summary>
            /// <para>Sub particle system which spawns at the locations of the collision of the particles from the parent system.</para>
            /// </summary>
            [Obsolete("collision1 property is deprecated. Use AddSubEmitter, RemoveSubEmitter, SetSubEmitterSystem and GetSubEmitterSystem instead.")]
            public ParticleSystem collision1
            {
                get => 
                    GetCollision(this.m_ParticleSystem, 1);
                set
                {
                    SetCollision(this.m_ParticleSystem, 1, value);
                }
            }
            /// <summary>
            /// <para>Sub particle system which spawns at the locations of the death of the particles from the parent system.</para>
            /// </summary>
            [Obsolete("death0 property is deprecated. Use AddSubEmitter, RemoveSubEmitter, SetSubEmitterSystem and GetSubEmitterSystem instead.")]
            public ParticleSystem death0
            {
                get => 
                    GetDeath(this.m_ParticleSystem, 0);
                set
                {
                    SetDeath(this.m_ParticleSystem, 0, value);
                }
            }
            /// <summary>
            /// <para>Sub particle system to spawn on death of the parent system's particles.</para>
            /// </summary>
            [Obsolete("death1 property is deprecated. Use AddSubEmitter, RemoveSubEmitter, SetSubEmitterSystem and GetSubEmitterSystem instead.")]
            public ParticleSystem death1
            {
                get => 
                    GetDeath(this.m_ParticleSystem, 1);
                set
                {
                    SetDeath(this.m_ParticleSystem, 1, value);
                }
            }
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetEnabled(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetEnabled(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern int GetSubEmittersCount(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetBirth(ParticleSystem system, int index, ParticleSystem value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern ParticleSystem GetBirth(ParticleSystem system, int index);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetCollision(ParticleSystem system, int index, ParticleSystem value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern ParticleSystem GetCollision(ParticleSystem system, int index);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetDeath(ParticleSystem system, int index, ParticleSystem value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern ParticleSystem GetDeath(ParticleSystem system, int index);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void AddSubEmitter(ParticleSystem system, ParticleSystem subEmitter, int type, int properties);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void RemoveSubEmitter(ParticleSystem system, int index);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetSubEmitterSystem(ParticleSystem system, int index, ParticleSystem subEmitter);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetSubEmitterType(ParticleSystem system, int index, int type);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetSubEmitterProperties(ParticleSystem system, int index, int properties);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern ParticleSystem GetSubEmitterSystem(ParticleSystem system, int index);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern int GetSubEmitterType(ParticleSystem system, int index);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern int GetSubEmitterProperties(ParticleSystem system, int index);
        }

        /// <summary>
        /// <para>Script interface for the Texture Sheet Animation module.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct TextureSheetAnimationModule
        {
            private ParticleSystem m_ParticleSystem;
            internal TextureSheetAnimationModule(ParticleSystem particleSystem)
            {
                this.m_ParticleSystem = particleSystem;
            }

            /// <summary>
            /// <para>Enable/disable the Texture Sheet Animation module.</para>
            /// </summary>
            public bool enabled
            {
                get => 
                    GetEnabled(this.m_ParticleSystem);
                set
                {
                    SetEnabled(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Defines the tiling of the texture in the X axis.</para>
            /// </summary>
            public int numTilesX
            {
                get => 
                    GetNumTilesX(this.m_ParticleSystem);
                set
                {
                    SetNumTilesX(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Defines the tiling of the texture in the Y axis.</para>
            /// </summary>
            public int numTilesY
            {
                get => 
                    GetNumTilesY(this.m_ParticleSystem);
                set
                {
                    SetNumTilesY(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Specifies the animation type.</para>
            /// </summary>
            public ParticleSystemAnimationType animation
            {
                get => 
                    ((ParticleSystemAnimationType) GetAnimationType(this.m_ParticleSystem));
                set
                {
                    SetAnimationType(this.m_ParticleSystem, (int) value);
                }
            }
            /// <summary>
            /// <para>Use a random row of the texture sheet for each particle emitted.</para>
            /// </summary>
            public bool useRandomRow
            {
                get => 
                    GetUseRandomRow(this.m_ParticleSystem);
                set
                {
                    SetUseRandomRow(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Curve to control which frame of the texture sheet animation to play.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve frameOverTime
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetFrameOverTime(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetFrameOverTime(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Frame over time mutiplier.</para>
            /// </summary>
            public float frameOverTimeMultiplier
            {
                get => 
                    GetFrameOverTimeMultiplier(this.m_ParticleSystem);
                set
                {
                    SetFrameOverTimeMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Define a random starting frame for the texture sheet animation.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve startFrame
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetStartFrame(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetStartFrame(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Starting frame multiplier.</para>
            /// </summary>
            public float startFrameMultiplier
            {
                get => 
                    GetStartFrameMultiplier(this.m_ParticleSystem);
                set
                {
                    SetStartFrameMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Specifies how many times the animation will loop during the lifetime of the particle.</para>
            /// </summary>
            public int cycleCount
            {
                get => 
                    GetCycleCount(this.m_ParticleSystem);
                set
                {
                    SetCycleCount(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Explicitly select which row of the texture sheet is used, when ParticleSystem.TextureSheetAnimationModule.useRandomRow is set to false.</para>
            /// </summary>
            public int rowIndex
            {
                get => 
                    GetRowIndex(this.m_ParticleSystem);
                set
                {
                    SetRowIndex(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Choose which UV channels will receive texture animation.</para>
            /// </summary>
            public UVChannelFlags uvChannelMask
            {
                get => 
                    ((UVChannelFlags) GetUVChannelMask(this.m_ParticleSystem));
                set
                {
                    SetUVChannelMask(this.m_ParticleSystem, (int) value);
                }
            }
            /// <summary>
            /// <para>Flip the U coordinate on particles, causing them to appear mirrored horizontally.</para>
            /// </summary>
            public float flipU
            {
                get => 
                    GetFlipU(this.m_ParticleSystem);
                set
                {
                    SetFlipU(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Flip the V coordinate on particles, causing them to appear mirrored vertically.</para>
            /// </summary>
            public float flipV
            {
                get => 
                    GetFlipV(this.m_ParticleSystem);
                set
                {
                    SetFlipV(this.m_ParticleSystem, value);
                }
            }
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetEnabled(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetEnabled(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetNumTilesX(ParticleSystem system, int value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern int GetNumTilesX(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetNumTilesY(ParticleSystem system, int value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern int GetNumTilesY(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetAnimationType(ParticleSystem system, int value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern int GetAnimationType(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetUseRandomRow(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetUseRandomRow(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetFrameOverTime(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetFrameOverTime(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetFrameOverTimeMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetFrameOverTimeMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetStartFrame(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetStartFrame(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetStartFrameMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetStartFrameMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetCycleCount(ParticleSystem system, int value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern int GetCycleCount(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetRowIndex(ParticleSystem system, int value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern int GetRowIndex(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetUVChannelMask(ParticleSystem system, int value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern int GetUVChannelMask(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetFlipU(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetFlipU(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetFlipV(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetFlipV(ParticleSystem system);
        }

        /// <summary>
        /// <para>Access the particle system trails module.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct TrailModule
        {
            private ParticleSystem m_ParticleSystem;
            internal TrailModule(ParticleSystem particleSystem)
            {
                this.m_ParticleSystem = particleSystem;
            }

            /// <summary>
            /// <para>Enable/disable the Trail module.</para>
            /// </summary>
            public bool enabled
            {
                get => 
                    GetEnabled(this.m_ParticleSystem);
                set
                {
                    SetEnabled(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Choose what proportion of particles will receive a trail.</para>
            /// </summary>
            public float ratio
            {
                get => 
                    GetRatio(this.m_ParticleSystem);
                set
                {
                    SetRatio(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>The curve describing the trail lifetime, throughout the lifetime of the particle.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve lifetime
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetLifetime(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetLifetime(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Change the lifetime multiplier.</para>
            /// </summary>
            public float lifetimeMultiplier
            {
                get => 
                    GetLifetimeMultiplier(this.m_ParticleSystem);
                set
                {
                    SetLifetimeMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Set the minimum distance each trail can travel before a new vertex is added to it.</para>
            /// </summary>
            public float minVertexDistance
            {
                get => 
                    GetMinVertexDistance(this.m_ParticleSystem);
                set
                {
                    SetMinVertexDistance(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Choose whether the U coordinate of the trail texture is tiled or stretched.</para>
            /// </summary>
            public ParticleSystemTrailTextureMode textureMode
            {
                get => 
                    ((ParticleSystemTrailTextureMode) GetTextureMode(this.m_ParticleSystem));
                set
                {
                    SetTextureMode(this.m_ParticleSystem, (float) value);
                }
            }
            /// <summary>
            /// <para>Drop new trail points in world space, regardless of Particle System Simulation Space.</para>
            /// </summary>
            public bool worldSpace
            {
                get => 
                    GetWorldSpace(this.m_ParticleSystem);
                set
                {
                    SetWorldSpace(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>If enabled, Trails will disappear immediately when their owning particle dies. Otherwise, the trail will persist until all its points have naturally expired, based on its lifetime.</para>
            /// </summary>
            public bool dieWithParticles
            {
                get => 
                    GetDieWithParticles(this.m_ParticleSystem);
                set
                {
                    SetDieWithParticles(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Set whether the particle size will act as a multiplier on top of the trail width.</para>
            /// </summary>
            public bool sizeAffectsWidth
            {
                get => 
                    GetSizeAffectsWidth(this.m_ParticleSystem);
                set
                {
                    SetSizeAffectsWidth(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Set whether the particle size will act as a multiplier on top of the trail lifetime.</para>
            /// </summary>
            public bool sizeAffectsLifetime
            {
                get => 
                    GetSizeAffectsLifetime(this.m_ParticleSystem);
                set
                {
                    SetSizeAffectsLifetime(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Toggle whether the trail will inherit the particle color as its starting color.</para>
            /// </summary>
            public bool inheritParticleColor
            {
                get => 
                    GetInheritParticleColor(this.m_ParticleSystem);
                set
                {
                    SetInheritParticleColor(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>The gradient controlling the trail colors during the lifetime of the attached particle.</para>
            /// </summary>
            public ParticleSystem.MinMaxGradient colorOverLifetime
            {
                get
                {
                    ParticleSystem.MinMaxGradient gradient = new ParticleSystem.MinMaxGradient();
                    GetColorOverLifetime(this.m_ParticleSystem, ref gradient);
                    return gradient;
                }
                set
                {
                    SetColorOverLifetime(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>The curve describing the width, of each trail point.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve widthOverTrail
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetWidthOverTrail(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetWidthOverTrail(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Change the width multiplier.</para>
            /// </summary>
            public float widthOverTrailMultiplier
            {
                get => 
                    GetWidthOverTrailMultiplier(this.m_ParticleSystem);
                set
                {
                    SetWidthOverTrailMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>The gradient controlling the trail colors over the length of the trail.</para>
            /// </summary>
            public ParticleSystem.MinMaxGradient colorOverTrail
            {
                get
                {
                    ParticleSystem.MinMaxGradient gradient = new ParticleSystem.MinMaxGradient();
                    GetColorOverTrail(this.m_ParticleSystem, ref gradient);
                    return gradient;
                }
                set
                {
                    SetColorOverTrail(this.m_ParticleSystem, ref value);
                }
            }
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetEnabled(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetEnabled(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetRatio(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetRatio(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetLifetime(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetLifetime(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetLifetimeMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetLifetimeMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetMinVertexDistance(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetMinVertexDistance(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetTextureMode(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern int GetTextureMode(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetWorldSpace(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetWorldSpace(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetDieWithParticles(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetDieWithParticles(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetSizeAffectsWidth(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetSizeAffectsWidth(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetSizeAffectsLifetime(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetSizeAffectsLifetime(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetInheritParticleColor(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetInheritParticleColor(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetColorOverLifetime(ParticleSystem system, ref ParticleSystem.MinMaxGradient gradient);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetColorOverLifetime(ParticleSystem system, ref ParticleSystem.MinMaxGradient gradient);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetWidthOverTrail(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetWidthOverTrail(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetWidthOverTrailMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetWidthOverTrailMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetColorOverTrail(ParticleSystem system, ref ParticleSystem.MinMaxGradient gradient);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetColorOverTrail(ParticleSystem system, ref ParticleSystem.MinMaxGradient gradient);
        }

        /// <summary>
        /// <para>Script interface for the Trigger module.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct TriggerModule
        {
            private ParticleSystem m_ParticleSystem;
            internal TriggerModule(ParticleSystem particleSystem)
            {
                this.m_ParticleSystem = particleSystem;
            }

            /// <summary>
            /// <para>Enable/disable the Trigger module.</para>
            /// </summary>
            public bool enabled
            {
                get => 
                    GetEnabled(this.m_ParticleSystem);
                set
                {
                    SetEnabled(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Choose what action to perform when particles are inside the trigger volume.</para>
            /// </summary>
            public ParticleSystemOverlapAction inside
            {
                get => 
                    ((ParticleSystemOverlapAction) GetInside(this.m_ParticleSystem));
                set
                {
                    SetInside(this.m_ParticleSystem, (int) value);
                }
            }
            /// <summary>
            /// <para>Choose what action to perform when particles are outside the trigger volume.</para>
            /// </summary>
            public ParticleSystemOverlapAction outside
            {
                get => 
                    ((ParticleSystemOverlapAction) GetOutside(this.m_ParticleSystem));
                set
                {
                    SetOutside(this.m_ParticleSystem, (int) value);
                }
            }
            /// <summary>
            /// <para>Choose what action to perform when particles enter the trigger volume.</para>
            /// </summary>
            public ParticleSystemOverlapAction enter
            {
                get => 
                    ((ParticleSystemOverlapAction) GetEnter(this.m_ParticleSystem));
                set
                {
                    SetEnter(this.m_ParticleSystem, (int) value);
                }
            }
            /// <summary>
            /// <para>Choose what action to perform when particles leave the trigger volume.</para>
            /// </summary>
            public ParticleSystemOverlapAction exit
            {
                get => 
                    ((ParticleSystemOverlapAction) GetExit(this.m_ParticleSystem));
                set
                {
                    SetExit(this.m_ParticleSystem, (int) value);
                }
            }
            /// <summary>
            /// <para>A multiplier applied to the size of each particle before overlaps are processed.</para>
            /// </summary>
            public float radiusScale
            {
                get => 
                    GetRadiusScale(this.m_ParticleSystem);
                set
                {
                    SetRadiusScale(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Set a collision shape associated with this particle system trigger.</para>
            /// </summary>
            /// <param name="index">Which collider to set.</param>
            /// <param name="collider">The collider to associate with this trigger.</param>
            public void SetCollider(int index, Component collider)
            {
                SetCollider(this.m_ParticleSystem, index, collider);
            }

            /// <summary>
            /// <para>Get a collision shape associated with this particle system trigger.</para>
            /// </summary>
            /// <param name="index">Which collider to return.</param>
            /// <returns>
            /// <para>The collider at the given index.</para>
            /// </returns>
            public Component GetCollider(int index) => 
                GetCollider(this.m_ParticleSystem, index);

            /// <summary>
            /// <para>The maximum number of collision shapes that can be attached to this particle system trigger.</para>
            /// </summary>
            public int maxColliderCount =>
                GetMaxColliderCount(this.m_ParticleSystem);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetEnabled(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetEnabled(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetInside(ParticleSystem system, int value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern int GetInside(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetOutside(ParticleSystem system, int value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern int GetOutside(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetEnter(ParticleSystem system, int value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern int GetEnter(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetExit(ParticleSystem system, int value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern int GetExit(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetRadiusScale(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetRadiusScale(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetCollider(ParticleSystem system, int index, Component collider);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern Component GetCollider(ParticleSystem system, int index);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern int GetMaxColliderCount(ParticleSystem system);
        }

        /// <summary>
        /// <para>Script interface for the Velocity Over Lifetime module.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct VelocityOverLifetimeModule
        {
            private ParticleSystem m_ParticleSystem;
            internal VelocityOverLifetimeModule(ParticleSystem particleSystem)
            {
                this.m_ParticleSystem = particleSystem;
            }

            /// <summary>
            /// <para>Enable/disable the Velocity Over Lifetime module.</para>
            /// </summary>
            public bool enabled
            {
                get => 
                    GetEnabled(this.m_ParticleSystem);
                set
                {
                    SetEnabled(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Curve to control particle speed based on lifetime, on the X axis.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve x
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetX(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetX(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Curve to control particle speed based on lifetime, on the Y axis.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve y
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetY(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetY(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>Curve to control particle speed based on lifetime, on the Z axis.</para>
            /// </summary>
            public ParticleSystem.MinMaxCurve z
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetZ(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetZ(this.m_ParticleSystem, ref value);
                }
            }
            /// <summary>
            /// <para>X axis speed multiplier.</para>
            /// </summary>
            public float xMultiplier
            {
                get => 
                    GetXMultiplier(this.m_ParticleSystem);
                set
                {
                    SetXMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Y axis speed multiplier.</para>
            /// </summary>
            public float yMultiplier
            {
                get => 
                    GetYMultiplier(this.m_ParticleSystem);
                set
                {
                    SetYMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Z axis speed multiplier.</para>
            /// </summary>
            public float zMultiplier
            {
                get => 
                    GetZMultiplier(this.m_ParticleSystem);
                set
                {
                    SetZMultiplier(this.m_ParticleSystem, value);
                }
            }
            /// <summary>
            /// <para>Specifies if the velocities are in local space (rotated with the transform) or world space.</para>
            /// </summary>
            public ParticleSystemSimulationSpace space
            {
                get => 
                    (!GetWorldSpace(this.m_ParticleSystem) ? ParticleSystemSimulationSpace.Local : ParticleSystemSimulationSpace.World);
                set
                {
                    SetWorldSpace(this.m_ParticleSystem, value == ParticleSystemSimulationSpace.World);
                }
            }
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetEnabled(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetEnabled(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void GetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetXMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetXMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetYMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetYMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetZMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern float GetZMultiplier(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern void SetWorldSpace(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
            private static extern bool GetWorldSpace(ParticleSystem system);
        }
    }
}

