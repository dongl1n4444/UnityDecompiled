namespace UnityEngine.Experimental.Director
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Playable used to mix AnimationPlayables.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct AnimationMixerPlayable
    {
        internal AnimationPlayable handle;
        internal Playable node =>
            this.handle.node;
        /// <summary>
        /// <para>Creates an AnimationMixerPlayable.</para>
        /// </summary>
        public static AnimationMixerPlayable Create()
        {
            AnimationMixerPlayable that = new AnimationMixerPlayable();
            InternalCreate(ref that);
            return that;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void InternalCreate(ref AnimationMixerPlayable that);
        /// <summary>
        /// <para>Call this method to release the resources associated to this Playable.</para>
        /// </summary>
        public void Destroy()
        {
            this.node.Destroy();
        }

        /// <summary>
        /// <para>The count of inputs on the Playable. This count includes slots that aren't connected to anything.</para>
        /// </summary>
        public int inputCount =>
            Playables.GetInputCountValidated(*((Playable*) this), base.GetType());
        /// <summary>
        /// <para>Returns the Playable connected at the specified index.</para>
        /// </summary>
        /// <param name="inputPort">Index of the input.</param>
        /// <returns>
        /// <para>Playable connected at the index specified, or null if the index is valid but is not connected to anything. This happens if there was once a Playable connected at the index, but was disconnected.</para>
        /// </returns>
        public unsafe Playable GetInput(int inputPort) => 
            Playables.GetInputValidated(*((Playable*) this), inputPort, base.GetType());

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

        /// <summary>
        /// <para>Automatically creates an AnimationClipPlayable for each supplied AnimationClip, then sets them as inputs to the mixer.</para>
        /// </summary>
        /// <param name="clips">AnimationClips to be used as inputs.</param>
        /// <returns>
        /// <para>Returns false if the creation of the AnimationClipPlayables failed, or if the connection failed.</para>
        /// </returns>
        public bool SetInputs(AnimationClip[] clips) => 
            AnimationPlayableUtilities.SetInputs(this, clips);

        /// <summary>
        /// <para>Get the weight of the Playable at a specified index.</para>
        /// </summary>
        /// <param name="index">Index of the input.</param>
        /// <returns>
        /// <para>Weight of the input Playable. Returns -1 if there is no input connected at this input index.</para>
        /// </returns>
        public unsafe float GetInputWeight(int index) => 
            Playables.GetInputWeightValidated(*((Playable*) this), index, base.GetType());

        /// <summary>
        /// <para>Sets the weight of an input.</para>
        /// </summary>
        /// <param name="inputIndex">Index of the input.</param>
        /// <param name="weight">Weight of the input.</param>
        public unsafe void SetInputWeight(int inputIndex, float weight)
        {
            Playables.SetInputWeightValidated(*((Playable*) this), inputIndex, weight, base.GetType());
        }

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
        public static bool operator ==(AnimationMixerPlayable x, Playable y) => 
            Playables.Equals((Playable) x, y);

        public static bool operator !=(AnimationMixerPlayable x, Playable y) => 
            !Playables.Equals((Playable) x, y);

        public override unsafe bool Equals(object p) => 
            Playables.Equals(*((Playable*) this), p);

        public override int GetHashCode() => 
            this.node.GetHashCode();

        public static implicit operator Playable(AnimationMixerPlayable b) => 
            b.node;

        public static implicit operator AnimationPlayable(AnimationMixerPlayable b) => 
            b.handle;

        /// <summary>
        /// <para>Returns true if the Playable is valid. A playable can be invalid if it was disposed. This is different from a Null playable.</para>
        /// </summary>
        public unsafe bool IsValid() => 
            Playables.IsValid(*((Playable*) this));

        public T CastTo<T>() where T: struct => 
            this.handle.CastTo<T>();

        /// <summary>
        /// <para>Adds an Playable as an input.</para>
        /// </summary>
        /// <param name="input">The [[Playable] to connect.</param>
        /// <returns>
        /// <para>Returns the index of the port the playable was connected to.</para>
        /// </returns>
        public unsafe int AddInput(Playable input) => 
            AnimationPlayableUtilities.AddInputValidated(*((AnimationPlayable*) this), input, base.GetType());

        /// <summary>
        /// <para>Sets an Playable as an input.</para>
        /// </summary>
        /// <param name="source">Playable to be used as input.</param>
        /// <param name="index">Index of the input.</param>
        /// <returns>
        /// <para>Returns false if the operation could not be completed.</para>
        /// </returns>
        public unsafe bool SetInput(Playable source, int index) => 
            AnimationPlayableUtilities.SetInputValidated(*((AnimationPlayable*) this), source, index, base.GetType());

        public unsafe bool SetInputs(IEnumerable<Playable> sources) => 
            AnimationPlayableUtilities.SetInputsValidated(*((AnimationPlayable*) this), sources, base.GetType());

        /// <summary>
        /// <para>Removes a playable from the list of inputs.</para>
        /// </summary>
        /// <param name="index">Index of the playable to remove.</param>
        /// <returns>
        /// <para>Returns false if the removal could not be removed because it wasn't found.</para>
        /// </returns>
        public unsafe bool RemoveInput(int index) => 
            AnimationPlayableUtilities.RemoveInputValidated(*((AnimationPlayable*) this), index, base.GetType());

        /// <summary>
        /// <para>Disconnects all input playables.</para>
        /// </summary>
        /// <returns>
        /// <para>Returns false if the removal fails.</para>
        /// </returns>
        public unsafe bool RemoveAllInputs() => 
            AnimationPlayableUtilities.RemoveAllInputsValidated(*((AnimationPlayable*) this), base.GetType());
    }
}

