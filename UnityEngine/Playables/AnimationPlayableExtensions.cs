namespace UnityEngine.Playables
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Animation specific extensions to the Playable Handle type.</para>
    /// </summary>
    public static class AnimationPlayableExtensions
    {
        /// <summary>
        /// <para>Get the animated clip that animates the fields of the playable.</para>
        /// </summary>
        /// <param name="handle">The handle to retrieve the Animation Clip from.</param>
        public static AnimationClip GetAnimatedProperties(this PlayableHandle handle) => 
            GetAnimatedPropertiesInternal(ref handle);

        internal static AnimationClip GetAnimatedPropertiesInternal(ref PlayableHandle playable) => 
            INTERNAL_CALL_GetAnimatedPropertiesInternal(ref playable);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern AnimationClip INTERNAL_CALL_GetAnimatedPropertiesInternal(ref PlayableHandle playable);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetAnimatedPropertiesInternal(ref PlayableHandle playable, AnimationClip animatedProperties);
        /// <summary>
        /// <para>Sets an animation clip to animate properties on the playable.</para>
        /// </summary>
        /// <param name="handle">Handle of the playable to set.</param>
        /// <param name="clip">Animation clip containing animated properties.</param>
        public static void SetAnimatedProperties(this PlayableHandle handle, AnimationClip clip)
        {
            SetAnimatedPropertiesInternal(ref handle, clip);
        }

        internal static void SetAnimatedPropertiesInternal(ref PlayableHandle playable, AnimationClip animatedProperties)
        {
            INTERNAL_CALL_SetAnimatedPropertiesInternal(ref playable, animatedProperties);
        }
    }
}

