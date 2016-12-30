namespace UnityEngine
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>Information about what animation clips is played and its weight.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size=1), EditorBrowsable(EditorBrowsableState.Never), Obsolete("Use AnimatorClipInfo instead (UnityUpgradable) -> AnimatorClipInfo", true)]
    public struct AnimationInfo
    {
        /// <summary>
        /// <para>Animation clip that is played.</para>
        /// </summary>
        public AnimationClip clip =>
            null;
        /// <summary>
        /// <para>The weight of the animation clip.</para>
        /// </summary>
        public float weight =>
            0f;
    }
}

