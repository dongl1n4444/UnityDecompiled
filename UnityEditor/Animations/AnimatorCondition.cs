namespace UnityEditor.Animations
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>Condition that is used to determine if a transition must be taken.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct AnimatorCondition
    {
        private AnimatorConditionMode m_ConditionMode;
        private string m_ConditionEvent;
        private float m_EventTreshold;
        /// <summary>
        /// <para>The mode of the condition.</para>
        /// </summary>
        public AnimatorConditionMode mode
        {
            get => 
                this.m_ConditionMode;
            set
            {
                this.m_ConditionMode = value;
            }
        }
        /// <summary>
        /// <para>The name of the parameter used in the condition.</para>
        /// </summary>
        public string parameter
        {
            get => 
                this.m_ConditionEvent;
            set
            {
                this.m_ConditionEvent = value;
            }
        }
        /// <summary>
        /// <para>The AnimatorParameter's threshold value for the condition to be true.</para>
        /// </summary>
        public float threshold
        {
            get => 
                this.m_EventTreshold;
            set
            {
                this.m_EventTreshold = value;
            }
        }
    }
}

