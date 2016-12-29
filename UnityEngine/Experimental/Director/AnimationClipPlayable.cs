namespace UnityEngine.Experimental.Director
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Playable that plays an AnimationClip. Can be used as an input to an AnimationPlayable.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct AnimationClipPlayable
    {
        internal AnimationPlayable handle;
        internal Playable node =>
            this.handle.node;
        /// <summary>
        /// <para>Creates an AnimationClipPlayable.</para>
        /// </summary>
        /// <param name="clip"></param>
        public static AnimationClipPlayable Create(AnimationClip clip)
        {
            AnimationClipPlayable that = new AnimationClipPlayable();
            InternalCreate(clip, ref that);
            return that;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void InternalCreate(AnimationClip clip, ref AnimationClipPlayable that);
        /// <summary>
        /// <para>Call this method to release the resources associated to this Playable.</para>
        /// </summary>
        public void Destroy()
        {
            this.node.Destroy();
        }

        public static bool operator ==(AnimationClipPlayable x, Playable y) => 
            Playables.Equals((Playable) x, y);

        public static bool operator !=(AnimationClipPlayable x, Playable y) => 
            !Playables.Equals((Playable) x, y);

        public override unsafe bool Equals(object p) => 
            Playables.Equals(*((Playable*) this), p);

        public override int GetHashCode() => 
            this.node.GetHashCode();

        public static implicit operator Playable(AnimationClipPlayable b) => 
            b.node;

        public static implicit operator AnimationPlayable(AnimationClipPlayable b) => 
            b.handle;

        /// <summary>
        /// <para>Returns true if the Playable is valid. A playable can be invalid if it was disposed. This is different from a Null playable.</para>
        /// </summary>
        public unsafe bool IsValid() => 
            Playables.IsValid(*((Playable*) this));

        /// <summary>
        /// <para>Current Experimental.Director.PlayState of this playable. This indicates whether the Playable is currently playing or paused.</para>
        /// </summary>
        public PlayState state
        {
            get => 
                Playables.GetPlayStateValidated(*((Playable*) this), base.GetType());
            set
            {
                Playables.SetPlayStateValidated(*((Playable*) this), value, base.GetType());
            }
        }
        /// <summary>
        /// <para>Current time in seconds.</para>
        /// </summary>
        public double time
        {
            get => 
                Playables.GetTimeValidated(*((Playable*) this), base.GetType());
            set
            {
                Playables.SetTimeValidated(*((Playable*) this), value, base.GetType());
            }
        }
        /// <summary>
        /// <para>Duration in seconds.</para>
        /// </summary>
        public double duration
        {
            get => 
                Playables.GetDurationValidated(*((Playable*) this), base.GetType());
            set
            {
                Playables.SetDurationValidated(*((Playable*) this), value, base.GetType());
            }
        }
        /// <summary>
        /// <para>The count of ouputs on the Playable.  Currently only 1 output is supported.</para>
        /// </summary>
        public int outputCount =>
            Playables.GetOutputCountValidated(*((Playable*) this), base.GetType());
        /// <summary>
        /// <para>Returns the Playable connected at the specified output index.</para>
        /// </summary>
        /// <param name="outputPort">Index of the output.</param>
        /// <returns>
        /// <para>Playable connected at the output index specified, or null if the index is valid but is not connected to anything. This happens if there was once a Playable connected at the index, but was disconnected.</para>
        /// </returns>
        public unsafe Playable GetOutput(int outputPort) => 
            Playables.GetOutputValidated(*((Playable*) this), outputPort, base.GetType());

        public T CastTo<T>() where T: struct => 
            this.handle.CastTo<T>();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern AnimationClip GetAnimationClip(ref AnimationClipPlayable that);
        /// <summary>
        /// <para>AnimationClip played by this playable.</para>
        /// </summary>
        public AnimationClip clip =>
            GetAnimationClip(ref this);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern float GetSpeed(ref AnimationClipPlayable that);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SetSpeed(ref AnimationClipPlayable that, float value);
        /// <summary>
        /// <para>The speed at which the AnimationClip is played.</para>
        /// </summary>
        public float speed
        {
            get => 
                GetSpeed(ref this);
            set
            {
                SetSpeed(ref this, value);
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool GetApplyFootIK(ref AnimationClipPlayable that);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SetApplyFootIK(ref AnimationClipPlayable that, bool value);
        /// <summary>
        /// <para>Applies Humanoid FootIK solver.</para>
        /// </summary>
        public bool applyFootIK
        {
            get => 
                GetApplyFootIK(ref this);
            set
            {
                SetApplyFootIK(ref this, value);
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool GetRemoveStartOffset(ref AnimationClipPlayable that);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SetRemoveStartOffset(ref AnimationClipPlayable that, bool value);
        internal bool removeStartOffset
        {
            get => 
                GetRemoveStartOffset(ref this);
            set
            {
                SetRemoveStartOffset(ref this, value);
            }
        }
    }
}

