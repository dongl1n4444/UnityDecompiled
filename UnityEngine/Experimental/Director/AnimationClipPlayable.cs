namespace UnityEngine.Experimental.Director
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Playable that plays an AnimationClip. Can be used as an input to an AnimationPlayable.</para>
    /// </summary>
    [RequiredByNativeCode]
    public sealed class AnimationClipPlayable : AnimationPlayable
    {
        private static AnimationClip GetAnimationClip(ref PlayableHandle handle) => 
            INTERNAL_CALL_GetAnimationClip(ref handle);

        private static bool GetApplyFootIK(ref PlayableHandle handle) => 
            INTERNAL_CALL_GetApplyFootIK(ref handle);

        private static bool GetRemoveStartOffset(ref PlayableHandle handle) => 
            INTERNAL_CALL_GetRemoveStartOffset(ref handle);

        private static float GetSpeed(ref PlayableHandle handle) => 
            INTERNAL_CALL_GetSpeed(ref handle);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern AnimationClip INTERNAL_CALL_GetAnimationClip(ref PlayableHandle handle);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_GetApplyFootIK(ref PlayableHandle handle);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_GetRemoveStartOffset(ref PlayableHandle handle);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern float INTERNAL_CALL_GetSpeed(ref PlayableHandle handle);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetApplyFootIK(ref PlayableHandle handle, bool value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetRemoveStartOffset(ref PlayableHandle handle, bool value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetSpeed(ref PlayableHandle handle, float value);
        private static void SetApplyFootIK(ref PlayableHandle handle, bool value)
        {
            INTERNAL_CALL_SetApplyFootIK(ref handle, value);
        }

        private static void SetRemoveStartOffset(ref PlayableHandle handle, bool value)
        {
            INTERNAL_CALL_SetRemoveStartOffset(ref handle, value);
        }

        private static void SetSpeed(ref PlayableHandle handle, float value)
        {
            INTERNAL_CALL_SetSpeed(ref handle, value);
        }

        /// <summary>
        /// <para>Applies Humanoid FootIK solver.</para>
        /// </summary>
        public bool applyFootIK
        {
            get => 
                GetApplyFootIK(ref this.handle);
            set
            {
                SetApplyFootIK(ref this.handle, value);
            }
        }

        /// <summary>
        /// <para>AnimationClip played by this Playable.</para>
        /// </summary>
        public AnimationClip clip =>
            GetAnimationClip(ref this.handle);

        internal bool removeStartOffset
        {
            get => 
                GetRemoveStartOffset(ref this.handle);
            set
            {
                SetRemoveStartOffset(ref this.handle, value);
            }
        }

        /// <summary>
        /// <para>The speed at which the AnimationClip is played.</para>
        /// </summary>
        public float speed
        {
            get => 
                GetSpeed(ref this.handle);
            set
            {
                SetSpeed(ref this.handle, value);
            }
        }
    }
}

