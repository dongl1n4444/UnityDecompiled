namespace UnityEditor.Animations
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// <para>This class contains all the owner's information for this State Machine Behaviour.</para>
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public sealed class StateMachineBehaviourContext
    {
        /// <summary>
        /// <para>The Animations.AnimatorController that owns this state machine behaviour.</para>
        /// </summary>
        public AnimatorController animatorController;
        /// <summary>
        /// <para>The object that owns this state machine behaviour. Could be an Animations.AnimatorState or Animations.AnimatorStateMachine.</para>
        /// </summary>
        public Object animatorObject;
        /// <summary>
        /// <para>The animator's layer index that owns this state machine behaviour.</para>
        /// </summary>
        public int layerIndex;
    }
}

