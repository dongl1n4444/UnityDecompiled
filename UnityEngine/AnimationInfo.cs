namespace UnityEngine
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>Information about what animation clips is played and its weight.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size=1), Obsolete("Use AnimatorClipInfo instead (UnityUpgradable) -> AnimatorClipInfo", true), EditorBrowsable(EditorBrowsableState.Never)]
    public struct AnimationInfo
    {
        /// <summary>
        /// <para>Animation clip that is played.</para>
        /// </summary>
        public AnimationClip clip
        {
            get
            {
                return null;
            }
        }
        /// <summary>
        /// <para>The weight of the animation clip.</para>
        /// </summary>
        public float weight
        {
            get
            {
                return 0f;
            }
        }
    }
}

