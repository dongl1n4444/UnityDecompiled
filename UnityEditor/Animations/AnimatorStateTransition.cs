namespace UnityEditor.Animations
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>Transitions define when and how the state machine switch from one state to another. AnimatorStateTransition always originate from an Animator State (or AnyState) and have timing parameters.</para>
    /// </summary>
    public sealed class AnimatorStateTransition : AnimatorTransitionBase
    {
        /// <summary>
        /// <para>Creates a new animator state transition.</para>
        /// </summary>
        public AnimatorStateTransition()
        {
            Internal_Create(this);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Internal_Create(AnimatorStateTransition mono);

        /// <summary>
        /// <para>Set to true to allow or disallow transition to self during AnyState transition.</para>
        /// </summary>
        public bool canTransitionToSelf { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The duration of the transition.</para>
        /// </summary>
        public float duration { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The normalized time of the source state when the condition is true.</para>
        /// </summary>
        public float exitTime { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>When active the transition will have an exit time condition.</para>
        /// </summary>
        public bool hasExitTime { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>When active the transition duration will have a fixed duration.</para>
        /// </summary>
        public bool hasFixedDuration { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Which AnimatorState transitions can interrupt the Transition.</para>
        /// </summary>
        public TransitionInterruptionSource interruptionSource { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The time at which the destination state will start.</para>
        /// </summary>
        public float offset { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The Transition can be interrupted by a transition that has a higher priority.</para>
        /// </summary>
        public bool orderedInterruption { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

