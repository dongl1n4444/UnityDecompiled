namespace UnityEditor.Animations
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Structure that represents a state machine in the context of its parent state machine.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public struct ChildAnimatorStateMachine
    {
        private AnimatorStateMachine m_StateMachine;
        private Vector3 m_Position;
        /// <summary>
        /// <para>The state machine.</para>
        /// </summary>
        public AnimatorStateMachine stateMachine
        {
            get
            {
                return this.m_StateMachine;
            }
            set
            {
                this.m_StateMachine = value;
            }
        }
        /// <summary>
        /// <para>The position the the state machine node in the context of its parent state machine.</para>
        /// </summary>
        public Vector3 position
        {
            get
            {
                return this.m_Position;
            }
            set
            {
                this.m_Position = value;
            }
        }
    }
}

