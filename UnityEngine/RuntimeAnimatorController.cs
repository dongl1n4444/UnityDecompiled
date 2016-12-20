namespace UnityEngine
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>Runtime representation of the AnimatorController. It can be used to change the Animator's controller during runtime.</para>
    /// </summary>
    public class RuntimeAnimatorController : Object
    {
        /// <summary>
        /// <para>Retrieves all AnimationClip used by the controller.</para>
        /// </summary>
        public AnimationClip[] animationClips { [MethodImpl(MethodImplOptions.InternalCall)] get; }
    }
}

