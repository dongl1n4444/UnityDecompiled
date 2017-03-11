namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Stores keyframe based animations.</para>
    /// </summary>
    public sealed class AnimationClip : Motion
    {
        /// <summary>
        /// <para>Creates a new animation clip.</para>
        /// </summary>
        public AnimationClip()
        {
            Internal_CreateAnimationClip(this);
        }

        /// <summary>
        /// <para>Adds an animation event to the clip.</para>
        /// </summary>
        /// <param name="evt">AnimationEvent to add.</param>
        public void AddEvent(AnimationEvent evt)
        {
            if (evt == null)
            {
                throw new ArgumentNullException("evt");
            }
            this.AddEventInternal(evt);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void AddEventInternal(object evt);
        /// <summary>
        /// <para>Clears all curves from the clip.</para>
        /// </summary>
        public void ClearCurves()
        {
            INTERNAL_CALL_ClearCurves(this);
        }

        /// <summary>
        /// <para>In order to insure better interpolation of quaternions, call this function after you are finished setting animation curves.</para>
        /// </summary>
        public void EnsureQuaternionContinuity()
        {
            INTERNAL_CALL_EnsureQuaternionContinuity(this);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern Array GetEventsInternal();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_ClearCurves(AnimationClip self);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_EnsureQuaternionContinuity(AnimationClip self);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_CreateAnimationClip([Writable] AnimationClip self);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_localBounds(out Bounds value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_localBounds(ref Bounds value);
        /// <summary>
        /// <para>Samples an animation at a given time for any animated properties.</para>
        /// </summary>
        /// <param name="go">The animated game object.</param>
        /// <param name="time">The time to sample an animation.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void SampleAnimation(GameObject go, float time);
        /// <summary>
        /// <para>Assigns the curve to animate a specific property.</para>
        /// </summary>
        /// <param name="relativePath">Path to the game object this curve applies to. relativePath is formatted similar to a pathname, e.g. "rootspineleftArm".
        /// If relativePath is empty it refers to the game object the animation clip is attached to.</param>
        /// <param name="type">The class type of the component that is animated.</param>
        /// <param name="propertyName">The name or path to the property being animated.</param>
        /// <param name="curve">The animation curve.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void SetCurve(string relativePath, System.Type type, string propertyName, AnimationCurve curve);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void SetEventsInternal(Array value);

        /// <summary>
        /// <para>Returns true if the animation clip has no curves and no events.</para>
        /// </summary>
        public bool empty { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Animation Events for this animation clip.</para>
        /// </summary>
        public AnimationEvent[] events
        {
            get => 
                ((AnimationEvent[]) this.GetEventsInternal());
            set
            {
                this.SetEventsInternal(value);
            }
        }

        /// <summary>
        /// <para>Frame rate at which keyframes are sampled. (Read Only)</para>
        /// </summary>
        public float frameRate { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Returns true if the animation contains curve that drives a humanoid rig.</para>
        /// </summary>
        public bool humanMotion { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Set to true if the AnimationClip will be used with the Legacy Animation component ( instead of the Animator ).</para>
        /// </summary>
        public bool legacy { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Animation length in seconds. (Read Only)</para>
        /// </summary>
        public float length { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>AABB of this Animation Clip in local space of Animation component that it is attached too.</para>
        /// </summary>
        public Bounds localBounds
        {
            get
            {
                Bounds bounds;
                this.INTERNAL_get_localBounds(out bounds);
                return bounds;
            }
            set
            {
                this.INTERNAL_set_localBounds(ref value);
            }
        }

        internal float startTime { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        internal float stopTime { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Sets the default wrap mode used in the animation state.</para>
        /// </summary>
        public WrapMode wrapMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

