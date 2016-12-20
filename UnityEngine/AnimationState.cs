namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>The AnimationState gives full control over animation blending.</para>
    /// </summary>
    [UsedByNativeCode]
    public sealed class AnimationState : TrackedReference
    {
        /// <summary>
        /// <para>Adds a transform which should be animated. This allows you to reduce the number of animations you have to create.</para>
        /// </summary>
        /// <param name="mix">The transform to animate.</param>
        /// <param name="recursive">Whether to also animate all children of the specified transform.</param>
        [ExcludeFromDocs]
        public void AddMixingTransform(Transform mix)
        {
            bool recursive = true;
            this.AddMixingTransform(mix, recursive);
        }

        /// <summary>
        /// <para>Adds a transform which should be animated. This allows you to reduce the number of animations you have to create.</para>
        /// </summary>
        /// <param name="mix">The transform to animate.</param>
        /// <param name="recursive">Whether to also animate all children of the specified transform.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void AddMixingTransform(Transform mix, [DefaultValue("true")] bool recursive);
        /// <summary>
        /// <para>Removes a transform which should be animated.</para>
        /// </summary>
        /// <param name="mix"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void RemoveMixingTransform(Transform mix);

        /// <summary>
        /// <para>Which blend mode should be used?</para>
        /// </summary>
        public AnimationBlendMode blendMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The clip that is being played by this animation state.</para>
        /// </summary>
        public AnimationClip clip { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Enables / disables the animation.</para>
        /// </summary>
        public bool enabled { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        public int layer { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The length of the animation clip in seconds.</para>
        /// </summary>
        public float length { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The name of the animation.</para>
        /// </summary>
        public string name { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The normalized playback speed.</para>
        /// </summary>
        public float normalizedSpeed { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The normalized time of the animation.</para>
        /// </summary>
        public float normalizedTime { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The playback speed of the animation. 1 is normal playback speed.</para>
        /// </summary>
        public float speed { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The current time of the animation.</para>
        /// </summary>
        public float time { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The weight of animation.</para>
        /// </summary>
        public float weight { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Wrapping mode of the animation.</para>
        /// </summary>
        public WrapMode wrapMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

