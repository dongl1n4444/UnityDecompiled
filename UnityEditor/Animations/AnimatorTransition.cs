namespace UnityEditor.Animations
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Transitions define when and how the state machine switch from on state to another. AnimatorTransition always originate from a StateMachine or a StateMachine entry. They do not define timing parameters.</para>
    /// </summary>
    public sealed class AnimatorTransition : AnimatorTransitionBase
    {
        /// <summary>
        /// <para>Creates a new animator transition.</para>
        /// </summary>
        public AnimatorTransition()
        {
            Internal_Create(this);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_Create(AnimatorTransition mono);
    }
}

