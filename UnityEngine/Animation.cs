namespace UnityEngine
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;

    /// <summary>
    /// <para>The animation component is used to play back animations.</para>
    /// </summary>
    public sealed class Animation : Behaviour, IEnumerable
    {
        /// <summary>
        /// <para>Adds a clip to the animation with name newName.</para>
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="newName"></param>
        public void AddClip(AnimationClip clip, string newName)
        {
            this.AddClip(clip, newName, -2147483648, 0x7fffffff);
        }

        /// <summary>
        /// <para>Adds clip to the only play between firstFrame and lastFrame. The new clip will also be added to the animation with name newName.</para>
        /// </summary>
        /// <param name="addLoopFrame">Should an extra frame be inserted at the end that matches the first frame? Turn this on if you are making a looping animation.</param>
        /// <param name="clip"></param>
        /// <param name="newName"></param>
        /// <param name="firstFrame"></param>
        /// <param name="lastFrame"></param>
        [ExcludeFromDocs]
        public void AddClip(AnimationClip clip, string newName, int firstFrame, int lastFrame)
        {
            bool addLoopFrame = false;
            this.AddClip(clip, newName, firstFrame, lastFrame, addLoopFrame);
        }

        /// <summary>
        /// <para>Adds clip to the only play between firstFrame and lastFrame. The new clip will also be added to the animation with name newName.</para>
        /// </summary>
        /// <param name="addLoopFrame">Should an extra frame be inserted at the end that matches the first frame? Turn this on if you are making a looping animation.</param>
        /// <param name="clip"></param>
        /// <param name="newName"></param>
        /// <param name="firstFrame"></param>
        /// <param name="lastFrame"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void AddClip(AnimationClip clip, string newName, int firstFrame, int lastFrame, [DefaultValue("false")] bool addLoopFrame);
        /// <summary>
        /// <para>Blends the animation named animation towards targetWeight over the next time seconds.</para>
        /// </summary>
        /// <param name="animation"></param>
        /// <param name="targetWeight"></param>
        /// <param name="fadeLength"></param>
        [ExcludeFromDocs]
        public void Blend(string animation)
        {
            float fadeLength = 0.3f;
            float targetWeight = 1f;
            this.Blend(animation, targetWeight, fadeLength);
        }

        /// <summary>
        /// <para>Blends the animation named animation towards targetWeight over the next time seconds.</para>
        /// </summary>
        /// <param name="animation"></param>
        /// <param name="targetWeight"></param>
        /// <param name="fadeLength"></param>
        [ExcludeFromDocs]
        public void Blend(string animation, float targetWeight)
        {
            float fadeLength = 0.3f;
            this.Blend(animation, targetWeight, fadeLength);
        }

        /// <summary>
        /// <para>Blends the animation named animation towards targetWeight over the next time seconds.</para>
        /// </summary>
        /// <param name="animation"></param>
        /// <param name="targetWeight"></param>
        /// <param name="fadeLength"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void Blend(string animation, [DefaultValue("1.0F")] float targetWeight, [DefaultValue("0.3F")] float fadeLength);
        /// <summary>
        /// <para>Fades the animation with name animation in over a period of time seconds and fades other animations out.</para>
        /// </summary>
        /// <param name="animation"></param>
        /// <param name="fadeLength"></param>
        /// <param name="mode"></param>
        [ExcludeFromDocs]
        public void CrossFade(string animation)
        {
            PlayMode stopSameLayer = PlayMode.StopSameLayer;
            float fadeLength = 0.3f;
            this.CrossFade(animation, fadeLength, stopSameLayer);
        }

        /// <summary>
        /// <para>Fades the animation with name animation in over a period of time seconds and fades other animations out.</para>
        /// </summary>
        /// <param name="animation"></param>
        /// <param name="fadeLength"></param>
        /// <param name="mode"></param>
        [ExcludeFromDocs]
        public void CrossFade(string animation, float fadeLength)
        {
            PlayMode stopSameLayer = PlayMode.StopSameLayer;
            this.CrossFade(animation, fadeLength, stopSameLayer);
        }

        /// <summary>
        /// <para>Fades the animation with name animation in over a period of time seconds and fades other animations out.</para>
        /// </summary>
        /// <param name="animation"></param>
        /// <param name="fadeLength"></param>
        /// <param name="mode"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void CrossFade(string animation, [DefaultValue("0.3F")] float fadeLength, [DefaultValue("PlayMode.StopSameLayer")] PlayMode mode);
        /// <summary>
        /// <para>Cross fades an animation after previous animations has finished playing.</para>
        /// </summary>
        /// <param name="animation"></param>
        /// <param name="fadeLength"></param>
        /// <param name="queue"></param>
        /// <param name="mode"></param>
        [ExcludeFromDocs]
        public AnimationState CrossFadeQueued(string animation)
        {
            PlayMode stopSameLayer = PlayMode.StopSameLayer;
            QueueMode completeOthers = QueueMode.CompleteOthers;
            float fadeLength = 0.3f;
            return this.CrossFadeQueued(animation, fadeLength, completeOthers, stopSameLayer);
        }

        /// <summary>
        /// <para>Cross fades an animation after previous animations has finished playing.</para>
        /// </summary>
        /// <param name="animation"></param>
        /// <param name="fadeLength"></param>
        /// <param name="queue"></param>
        /// <param name="mode"></param>
        [ExcludeFromDocs]
        public AnimationState CrossFadeQueued(string animation, float fadeLength)
        {
            PlayMode stopSameLayer = PlayMode.StopSameLayer;
            QueueMode completeOthers = QueueMode.CompleteOthers;
            return this.CrossFadeQueued(animation, fadeLength, completeOthers, stopSameLayer);
        }

        /// <summary>
        /// <para>Cross fades an animation after previous animations has finished playing.</para>
        /// </summary>
        /// <param name="animation"></param>
        /// <param name="fadeLength"></param>
        /// <param name="queue"></param>
        /// <param name="mode"></param>
        [ExcludeFromDocs]
        public AnimationState CrossFadeQueued(string animation, float fadeLength, QueueMode queue)
        {
            PlayMode stopSameLayer = PlayMode.StopSameLayer;
            return this.CrossFadeQueued(animation, fadeLength, queue, stopSameLayer);
        }

        /// <summary>
        /// <para>Cross fades an animation after previous animations has finished playing.</para>
        /// </summary>
        /// <param name="animation"></param>
        /// <param name="fadeLength"></param>
        /// <param name="queue"></param>
        /// <param name="mode"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern AnimationState CrossFadeQueued(string animation, [DefaultValue("0.3F")] float fadeLength, [DefaultValue("QueueMode.CompleteOthers")] QueueMode queue, [DefaultValue("PlayMode.StopSameLayer")] PlayMode mode);
        public AnimationClip GetClip(string name)
        {
            AnimationState state = this.GetState(name);
            if (state != null)
            {
                return state.clip;
            }
            return null;
        }

        /// <summary>
        /// <para>Get the number of clips currently assigned to this animation.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern int GetClipCount();
        public IEnumerator GetEnumerator() => 
            new Enumerator(this);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern AnimationState GetState(string name);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern AnimationState GetStateAtIndex(int index);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern int GetStateCount();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Rewind(Animation self);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Sample(Animation self);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Stop(Animation self);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_SyncLayer(Animation self, int layer);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_localBounds(out Bounds value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void Internal_RewindByName(string name);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_localBounds(ref Bounds value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void Internal_StopByName(string name);
        /// <summary>
        /// <para>Is the animation named name playing?</para>
        /// </summary>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool IsPlaying(string name);
        [ExcludeFromDocs]
        public bool Play()
        {
            PlayMode stopSameLayer = PlayMode.StopSameLayer;
            return this.Play(stopSameLayer);
        }

        /// <summary>
        /// <para>Plays an animation without any blending.</para>
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="animation"></param>
        [ExcludeFromDocs]
        public bool Play(string animation)
        {
            PlayMode stopSameLayer = PlayMode.StopSameLayer;
            return this.Play(animation, stopSameLayer);
        }

        [Obsolete("use PlayMode instead of AnimationPlayMode.")]
        public bool Play(AnimationPlayMode mode) => 
            this.PlayDefaultAnimation((PlayMode) mode);

        /// <summary>
        /// <para>Plays an animation without any blending.</para>
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="animation"></param>
        public bool Play([DefaultValue("PlayMode.StopSameLayer")] PlayMode mode) => 
            this.PlayDefaultAnimation(mode);

        [Obsolete("use PlayMode instead of AnimationPlayMode.")]
        public bool Play(string animation, AnimationPlayMode mode) => 
            this.Play(animation, (PlayMode) mode);

        /// <summary>
        /// <para>Plays an animation without any blending.</para>
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="animation"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool Play(string animation, [DefaultValue("PlayMode.StopSameLayer")] PlayMode mode);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern bool PlayDefaultAnimation(PlayMode mode);
        /// <summary>
        /// <para>Plays an animation after previous animations has finished playing.</para>
        /// </summary>
        /// <param name="animation"></param>
        /// <param name="queue"></param>
        /// <param name="mode"></param>
        [ExcludeFromDocs]
        public AnimationState PlayQueued(string animation)
        {
            PlayMode stopSameLayer = PlayMode.StopSameLayer;
            QueueMode completeOthers = QueueMode.CompleteOthers;
            return this.PlayQueued(animation, completeOthers, stopSameLayer);
        }

        /// <summary>
        /// <para>Plays an animation after previous animations has finished playing.</para>
        /// </summary>
        /// <param name="animation"></param>
        /// <param name="queue"></param>
        /// <param name="mode"></param>
        [ExcludeFromDocs]
        public AnimationState PlayQueued(string animation, QueueMode queue)
        {
            PlayMode stopSameLayer = PlayMode.StopSameLayer;
            return this.PlayQueued(animation, queue, stopSameLayer);
        }

        /// <summary>
        /// <para>Plays an animation after previous animations has finished playing.</para>
        /// </summary>
        /// <param name="animation"></param>
        /// <param name="queue"></param>
        /// <param name="mode"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern AnimationState PlayQueued(string animation, [DefaultValue("QueueMode.CompleteOthers")] QueueMode queue, [DefaultValue("PlayMode.StopSameLayer")] PlayMode mode);
        /// <summary>
        /// <para>Remove clip from the animation list.</para>
        /// </summary>
        /// <param name="clipName"></param>
        public void RemoveClip(string clipName)
        {
            this.RemoveClip2(clipName);
        }

        /// <summary>
        /// <para>Remove clip from the animation list.</para>
        /// </summary>
        /// <param name="clip"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void RemoveClip(AnimationClip clip);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void RemoveClip2(string clipName);
        /// <summary>
        /// <para>Rewinds all animations.</para>
        /// </summary>
        public void Rewind()
        {
            INTERNAL_CALL_Rewind(this);
        }

        /// <summary>
        /// <para>Rewinds the animation named name.</para>
        /// </summary>
        /// <param name="name"></param>
        public void Rewind(string name)
        {
            this.Internal_RewindByName(name);
        }

        /// <summary>
        /// <para>Samples animations at the current state.</para>
        /// </summary>
        public void Sample()
        {
            INTERNAL_CALL_Sample(this);
        }

        /// <summary>
        /// <para>Stops all playing animations that were started with this Animation.</para>
        /// </summary>
        public void Stop()
        {
            INTERNAL_CALL_Stop(this);
        }

        /// <summary>
        /// <para>Stops an animation named name.</para>
        /// </summary>
        /// <param name="name"></param>
        public void Stop(string name)
        {
            this.Internal_StopByName(name);
        }

        public void SyncLayer(int layer)
        {
            INTERNAL_CALL_SyncLayer(this, layer);
        }

        /// <summary>
        /// <para>When turned on, Unity might stop animating if it thinks that the results of the animation won't be visible to the user.</para>
        /// </summary>
        [Obsolete("Use cullingType instead")]
        public bool animateOnlyIfVisible { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>When turned on, animations will be executed in the physics loop. This is only useful in conjunction with kinematic rigidbodies.</para>
        /// </summary>
        public bool animatePhysics { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The default animation.</para>
        /// </summary>
        public AnimationClip clip { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Controls culling of this Animation component.</para>
        /// </summary>
        public AnimationCullingType cullingType { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Are we playing any animations?</para>
        /// </summary>
        public bool isPlaying { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public AnimationState this[string name] =>
            this.GetState(name);

        /// <summary>
        /// <para>AABB of this Animation animation component in local space.</para>
        /// </summary>
        public Bounds localBounds
        {
            get
            {
                Bounds bounds;
                this.INTERNAL_get_localBounds(out bounds);
                return bounds;
            }
            set
            {
                this.INTERNAL_set_localBounds(ref value);
            }
        }

        /// <summary>
        /// <para>Should the default animation clip (the Animation.clip property) automatically start playing on startup?</para>
        /// </summary>
        public bool playAutomatically { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>How should time beyond the playback range of the clip be treated?</para>
        /// </summary>
        public WrapMode wrapMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        private sealed class Enumerator : IEnumerator
        {
            private int m_CurrentIndex = -1;
            private Animation m_Outer;

            internal Enumerator(Animation outer)
            {
                this.m_Outer = outer;
            }

            public bool MoveNext()
            {
                int stateCount = this.m_Outer.GetStateCount();
                this.m_CurrentIndex++;
                return (this.m_CurrentIndex < stateCount);
            }

            public void Reset()
            {
                this.m_CurrentIndex = -1;
            }

            public object Current =>
                this.m_Outer.GetStateAtIndex(this.m_CurrentIndex);
        }
    }
}

