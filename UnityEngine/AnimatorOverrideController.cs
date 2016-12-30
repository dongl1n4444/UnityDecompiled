namespace UnityEngine
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Interface to control Animator Override Controller.</para>
    /// </summary>
    public sealed class AnimatorOverrideController : RuntimeAnimatorController
    {
        internal OnOverrideControllerDirtyCallback OnOverrideControllerDirty;

        /// <summary>
        /// <para>Creates an empty Animator Override Controller.</para>
        /// </summary>
        public AnimatorOverrideController()
        {
            Internal_CreateAnimatorOverrideController(this, null);
        }

        /// <summary>
        /// <para>Creates an Animator Override Controller that overrides controller.</para>
        /// </summary>
        /// <param name="controller">Runtime Animator Controller to override.</param>
        public AnimatorOverrideController(RuntimeAnimatorController controller)
        {
            Internal_CreateAnimatorOverrideController(this, controller);
        }

        public void ApplyOverrides(IList<KeyValuePair<AnimationClip, AnimationClip>> overrides)
        {
            if (overrides == null)
            {
                throw new ArgumentNullException("overrides");
            }
            for (int i = 0; i < overrides.Count; i++)
            {
                KeyValuePair<AnimationClip, AnimationClip> pair = overrides[i];
                KeyValuePair<AnimationClip, AnimationClip> pair2 = overrides[i];
                this.Internal_SetClip(pair.Key, pair2.Value, false);
            }
            this.SendNotification();
        }

        public void GetOverrides(List<KeyValuePair<AnimationClip, AnimationClip>> overrides)
        {
            if (overrides == null)
            {
                throw new ArgumentNullException("overrides");
            }
            int overridesCount = this.overridesCount;
            if (overrides.Capacity < overridesCount)
            {
                overrides.Capacity = overridesCount;
            }
            overrides.Clear();
            for (int i = 0; i < overridesCount; i++)
            {
                AnimationClip key = this.Internal_GetOriginalClip(i);
                overrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(key, this.Internal_GetOverrideClip(key)));
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_CreateAnimatorOverrideController([Writable] AnimatorOverrideController self, RuntimeAnimatorController controller);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern AnimationClip Internal_GetClip(AnimationClip originalClip, bool returnEffectiveClip);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern AnimationClip Internal_GetClipByName(string name, bool returnEffectiveClip);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern AnimationClip Internal_GetOriginalClip(int index);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern AnimationClip Internal_GetOverrideClip(AnimationClip originalClip);
        [ExcludeFromDocs]
        private void Internal_SetClip(AnimationClip originalClip, AnimationClip overrideClip)
        {
            bool notify = true;
            this.Internal_SetClip(originalClip, overrideClip, notify);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void Internal_SetClip(AnimationClip originalClip, AnimationClip overrideClip, [DefaultValue("true")] bool notify);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void Internal_SetClipByName(string name, AnimationClip clip);
        [RequiredByNativeCode]
        internal static void OnInvalidateOverrideController(AnimatorOverrideController controller)
        {
            if (controller.OnOverrideControllerDirty != null)
            {
                controller.OnOverrideControllerDirty();
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void PerformOverrideClipListCleanup();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void SendNotification();

        /// <summary>
        /// <para>Returns the list of orignal Animation Clip from the controller and their override Animation Clip.</para>
        /// </summary>
        [Obsolete("clips property is deprecated. Use AnimatorOverrideController.GetOverrides and AnimatorOverrideController.ApplyOverrides instead.")]
        public AnimationClipPair[] clips
        {
            get
            {
                int overridesCount = this.overridesCount;
                AnimationClipPair[] pairArray = new AnimationClipPair[overridesCount];
                for (int i = 0; i < overridesCount; i++)
                {
                    pairArray[i] = new AnimationClipPair();
                    pairArray[i].originalClip = this.Internal_GetOriginalClip(i);
                    pairArray[i].overrideClip = this.Internal_GetOverrideClip(pairArray[i].originalClip);
                }
                return pairArray;
            }
            set
            {
                for (int i = 0; i < value.Length; i++)
                {
                    this.Internal_SetClip(value[i].originalClip, value[i].overrideClip, false);
                }
                this.SendNotification();
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
        /// <para>Returns the count of overrides.</para>
        /// </summary>
        public int overridesCount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The Runtime Animator Controller that the Animator Override Controller overrides.</para>
        /// </summary>
        public RuntimeAnimatorController runtimeAnimatorController { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        internal delegate void OnOverrideControllerDirtyCallback();
    }
}

