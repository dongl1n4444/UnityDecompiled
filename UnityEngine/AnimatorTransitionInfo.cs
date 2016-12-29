namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Information about the current transition.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public struct AnimatorTransitionInfo
    {
        private int m_FullPath;
        private int m_UserName;
        private int m_Name;
        private float m_NormalizedTime;
        private bool m_AnyState;
        private int m_TransitionType;
        /// <summary>
        /// <para>Does name match the name of the active Transition.</para>
        /// </summary>
        /// <param name="name"></param>
        public bool IsName(string name) => 
            ((Animator.StringToHash(name) == this.m_Name) || (Animator.StringToHash(name) == this.m_FullPath));

        /// <summary>
        /// <para>Does userName match the name of the active Transition.</para>
        /// </summary>
        /// <param name="name"></param>
        public bool IsUserName(string name) => 
            (Animator.StringToHash(name) == this.m_UserName);

        /// <summary>
        /// <para>The unique name of the Transition.</para>
        /// </summary>
        public int fullPathHash =>
            this.m_FullPath;
        /// <summary>
        /// <para>The simplified name of the Transition.</para>
        /// </summary>
        public int nameHash =>
            this.m_Name;
        /// <summary>
        /// <para>The user-specified name of the Transition.</para>
        /// </summary>
        public int userNameHash =>
            this.m_UserName;
        /// <summary>
        /// <para>Normalized time of the Transition.</para>
        /// </summary>
        public float normalizedTime =>
            this.m_NormalizedTime;
        /// <summary>
        /// <para>Returns true if the transition is from an AnyState node, or from Animator.CrossFade().</para>
        /// </summary>
        public bool anyState =>
            this.m_AnyState;
        internal bool entry =>
            ((this.m_TransitionType & 2) != 0);
        internal bool exit =>
            ((this.m_TransitionType & 4) != 0);
    }
}

