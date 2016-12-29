namespace UnityEditor.Animations
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Structure that represents a state in the context of its parent state machine.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public struct ChildAnimatorState
    {
        private AnimatorState m_State;
        private Vector3 m_Position;
        /// <summary>
        /// <para>The state.</para>
        /// </summary>
        public AnimatorState state
        {
            get => 
                this.m_State;
            set
            {
                this.m_State = value;
            }
        }
        /// <summary>
        /// <para>The position the the state node in the context of its parent state machine.</para>
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
    }
}

