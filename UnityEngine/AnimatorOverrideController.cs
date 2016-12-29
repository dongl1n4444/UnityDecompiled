namespace UnityEngine
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Interface to control AnimatorOverrideController.</para>
    /// </summary>
    public sealed class AnimatorOverrideController : RuntimeAnimatorController
    {
        internal OnOverrideControllerDirtyCallback OnOverrideControllerDirty;

        public AnimatorOverrideController()
        {
            Internal_CreateAnimationSet(this);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern AnimationClip[] GetOriginalClips();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern AnimationClip[] GetOverrideClips();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Internal_CreateAnimationSet([Writable] AnimatorOverrideController self);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern AnimationClip Internal_GetClip(AnimationClip originalClip, bool returnEffectiveClip);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern AnimationClip Internal_GetClipByName(string name, bool returnEffectiveClip);
        [ExcludeFromDocs]
        private void Internal_SetClip(AnimationClip originalClip, AnimationClip overrideClip)
        {
            bool notify = true;
            this.Internal_SetClip(originalClip, overrideClip, notify);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void Internal_SetClip(AnimationClip originalClip, AnimationClip overrideClip, [DefaultValue("true")] bool notify);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void Internal_SetClipByName(string name, AnimationClip clip);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void Internal_SetDirty();
        [RequiredByNativeCode]
        internal static void OnInvalidateOverrideController(AnimatorOverrideController controller)
        {
            if (controller.OnOverrideControllerDirty != null)
            {
                controller.OnOverrideControllerDirty();
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void PerformOverrideClipListCleanup();

        /// <summary>
        /// <para>Returns the list of orignal clip from the controller and their override clip.</para>
        /// </summary>
        public AnimationClipPair[] clips
        {
            get
            {
                AnimationClip[] originalClips = this.GetOriginalClips();
                Dictionary<AnimationClip, bool> dictionary = new Dictionary<AnimationClip, bool>(originalClips.Length);
                foreach (AnimationClip clip in originalClips)
                {
                    dictionary[clip] = true;
                }
                originalClips = new AnimationClip[dictionary.Count];
                dictionary.Keys.CopyTo(originalClips, 0);
                AnimationClipPair[] pairArray = new AnimationClipPair[originalClips.Length];
                for (int i = 0; i < originalClips.Length; i++)
                {
                    pairArray[i] = new AnimationClipPair();
                    pairArray[i].originalClip = originalClips[i];
                    pairArray[i].overrideClip = this.Internal_GetClip(originalClips[i], false);
                }
                return pairArray;
            }
            set
            {
                for (int i = 0; i < value.Length; i++)
                {
                    this.Internal_SetClip(value[i].originalClip, value[i].overrideClip, false);
                }
                this.Internal_SetDirty();
            }
        }

        public AnimationClip this[string name]
        {
            get => 
                this.Internal_GetClipByName(name, true);
            set
            {
                this.Internal_SetClipByName(name, value);
            }
        }

        public AnimationClip this[AnimationClip clip]
        {
            get => 
                this.Internal_GetClip(clip, true);
            set
            {
                this.Internal_SetClip(clip, value);
            }
        }

        /// <summary>
        /// <para>The Controller that the AnimatorOverrideController overrides.</para>
        /// </summary>
        public RuntimeAnimatorController runtimeAnimatorController { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        internal delegate void OnOverrideControllerDirtyCallback();
    }
}

