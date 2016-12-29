namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Information about the current or next state.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public struct AnimatorStateInfo
    {
        private int m_Name;
        private int m_Path;
        private int m_FullPath;
        private float m_NormalizedTime;
        private float m_Length;
        private float m_Speed;
        private float m_SpeedMultiplier;
        private int m_Tag;
        private int m_Loop;
        /// <summary>
        /// <para>Does name match the name of the active state in the statemachine?</para>
        /// </summary>
        /// <param name="name"></param>
        public bool IsName(string name)
        {
            int num = Animator.StringToHash(name);
            return (((num == this.m_FullPath) || (num == this.m_Name)) || (num == this.m_Path));
        }

        /// <summary>
        /// <para>The full path hash for this state.</para>
        /// </summary>
        public int fullPathHash =>
            this.m_FullPath;
        /// <summary>
        /// <para>The hashed name of the State.</para>
        /// </summary>
        [Obsolete("Use AnimatorStateInfo.fullPathHash instead.")]
        public int nameHash =>
            this.m_Path;
        /// <summary>
        /// <para>The hash is generated using Animator::StringToHash. The string to pass doest not include the parent layer's name.</para>
        /// </summary>
        public int shortNameHash =>
            this.m_Name;
        /// <summary>
        /// <para>Normalized time of the State.</para>
        /// </summary>
        public float normalizedTime =>
            this.m_NormalizedTime;
        /// <summary>
        /// <para>Current duration of the state.</para>
        /// </summary>
        public float length =>
            this.m_Length;
        /// <summary>
        /// <para>The playback speed of the animation. 1 is the normal playback speed.</para>
        /// </summary>
        public float speed =>
            this.m_Speed;
        /// <summary>
        /// <para>The speed multiplier for this state.</para>
        /// </summary>
        public float speedMultiplier =>
            this.m_SpeedMultiplier;
        /// <summary>
        /// <para>The Tag of the State.</para>
        /// </summary>
        public int tagHash =>
            this.m_Tag;
        /// <summary>
        /// <para>Does tag match the tag of the active state in the statemachine.</para>
        /// </summary>
        /// <param name="tag"></param>
        public bool IsTag(string tag) => 
            (Animator.StringToHash(tag) == this.m_Tag);

        /// <summary>
        /// <para>Is the state looping.</para>
        /// </summary>
        public bool loop =>
            (this.m_Loop != 0);
    }
}

