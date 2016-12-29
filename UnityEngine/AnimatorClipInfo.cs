namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Information about clip being played and blended by the Animator.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct AnimatorClipInfo
    {
        private int m_ClipInstanceID;
        private float m_Weight;
        /// <summary>
        /// <para>Returns the animation clip played by the Animator.</para>
        /// </summary>
        public AnimationClip clip =>
            ((this.m_ClipInstanceID == 0) ? null : ClipInstanceToScriptingObject(this.m_ClipInstanceID));
        /// <summary>
        /// <para>Returns the blending weight used by the Animator to blend this clip.</para>
        /// </summary>
        public float weight =>
            this.m_Weight;
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern AnimationClip ClipInstanceToScriptingObject(int instanceID);
    }
}

