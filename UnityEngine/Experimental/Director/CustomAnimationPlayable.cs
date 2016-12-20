namespace UnityEngine.Experimental.Director
{
    using System;
    using System.Collections.Generic;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>To implement custom handling of AnimationPlayable, inherit from this class.</para>
    /// </summary>
    [RequiredByNativeCode]
    public class CustomAnimationPlayable : ScriptPlayable
    {
        internal AnimationPlayable handle;

        public CustomAnimationPlayable()
        {
            if (!this.handle.IsValid())
            {
                string str = base.GetType().ToString();
                string[] textArray1 = new string[] { str, " must be instantiated using the Playable.Create<", str, "> method instead of new ", str, "." };
                throw new InvalidOperationException(string.Concat(textArray1));
            }
        }

        /// <summary>
        /// <para>Adds an Playable as an input.</para>
        /// </summary>
        /// <param name="input">The [[Playable] to connect.</param>
        /// <returns>
        /// <para>Returns the index of the port the playable was connected to.</para>
        /// </returns>
        public int AddInput(Playable input)
        {
            return AnimationPlayableUtilities.AddInputValidated((AnimationPlayable) this, input, base.GetType());
        }

        public T CastTo<T>() where T: struct
        {
            return this.handle.CastTo<T>();
        }

        /// <summary>
        /// <para>Call this method to release the resources associated to this Playable.</para>
        /// </summary>
        public void Destroy()
        {
            this.node.Destroy();
        }

        /// <summary>
        /// <para>Returns the Playable connected at the specified index.</para>
        /// </summary>
        /// <param name="inputPort">Index of the input.</param>
        /// <returns>
        /// <para>Playable connected at the index specified, or null if the index is valid but is not connected to anything. This happens if there was once a Playable connected at the index, but was disconnected.</para>
        /// </returns>
        public Playable GetInput(int inputPort)
        {
            return Playables.GetInputValidated((Playable) this, inputPort, base.GetType());
        }

        /// <summary>
        /// <para>Get the weight of the Playable at a specified index.</para>
        /// </summary>
        /// <param name="index">Index of the input.</param>
        /// <returns>
        /// <para>Weight of the input Playable. Returns -1 if there is no input connected at this input index.</para>
        /// </returns>
        public float GetInputWeight(int index)
        {
            return Playables.GetInputWeightValidated((Playable) this, index, base.GetType());
        }

        /// <summary>
        /// <para>Returns the Playable connected at the specified output index.</para>
        /// </summary>
        /// <param name="outputPort">Index of the output.</param>
        /// <returns>
        /// <para>Playable connected at the output index specified, or null if the index is valid but is not connected to anything. This happens if there was once a Playable connected at the index, but was disconnected.</para>
        /// </returns>
        public Playable GetOutput(int outputPort)
        {
            return Playables.GetOutputValidated((Playable) this, outputPort, base.GetType());
        }

        /// <summary>
        /// <para>Override this method to perform custom operations when the PlayState changes.</para>
        /// </summary>
        /// <param name="newState"></param>
        public virtual void OnSetPlayState(PlayState newState)
        {
        }

        /// <summary>
        /// <para>Override this method to perform custom operations when the local time changes.</para>
        /// </summary>
        /// <param name="localTime"></param>
        public virtual void OnSetTime(float localTime)
        {
        }

        public static implicit operator AnimationPlayable(CustomAnimationPlayable s)
        {
            return s.handle;
        }

        public static implicit operator Playable(CustomAnimationPlayable s)
        {
            return new Playable { 
                m_Handle = s.node.m_Handle,
                m_Version = s.node.m_Version
            };
        }

        /// <summary>
        /// <para>Override this method to manage input connections and change weights on inputs.</para>
        /// </summary>
        /// <param name="info"></param>
        public virtual void PrepareFrame(FrameData info)
        {
        }

        /// <summary>
        /// <para>Disconnects all input playables.</para>
        /// </summary>
        /// <returns>
        /// <para>Returns false if the removal fails.</para>
        /// </returns>
        public bool RemoveAllInputs()
        {
            return AnimationPlayableUtilities.RemoveAllInputsValidated((AnimationPlayable) this, base.GetType());
        }

        /// <summary>
        /// <para>Removes a playable from the list of inputs.</para>
        /// </summary>
        /// <param name="index">Index of the playable to remove.</param>
        /// <returns>
        /// <para>Returns false if the removal could not be removed because it wasn't found.</para>
        /// </returns>
        public bool RemoveInput(int index)
        {
            return AnimationPlayableUtilities.RemoveInputValidated((AnimationPlayable) this, index, base.GetType());
        }

        internal void SetHandle(int version, IntPtr playableHandle)
        {
            this.handle.handle.m_Handle = playableHandle;
            this.handle.handle.m_Version = version;
        }

        /// <summary>
        /// <para>Sets an Playable as an input.</para>
        /// </summary>
        /// <param name="source">Playable to be used as input.</param>
        /// <param name="index">Index of the input.</param>
        /// <returns>
        /// <para>Returns false if the operation could not be completed.</para>
        /// </returns>
        public bool SetInput(Playable source, int index)
        {
            return AnimationPlayableUtilities.SetInputValidated((AnimationPlayable) this, source, index, base.GetType());
        }

        public bool SetInputs(IEnumerable<Playable> sources)
        {
            return AnimationPlayableUtilities.SetInputsValidated((AnimationPlayable) this, sources, base.GetType());
        }

        /// <summary>
        /// <para>Set the weight of an input.</para>
        /// </summary>
        /// <param name="inputIndex"></param>
        /// <param name="weight"></param>
        public void SetInputWeight(int inputIndex, float weight)
        {
            Playables.SetInputWeightValidated((Playable) this, inputIndex, weight, base.GetType());
        }

        /// <summary>
        /// <para>Duration in seconds.</para>
        /// </summary>
        public double duration
        {
            get
            {
                return Playables.GetDurationValidated((Playable) this, base.GetType());
            }
            set
            {
                Playables.SetDurationValidated((Playable) this, value, base.GetType());
            }
        }

        /// <summary>
        /// <para>The count of inputs on the Playable. This count includes slots that aren't connected to anything.</para>
        /// </summary>
        public int inputCount
        {
            get
            {
                return Playables.GetInputCountValidated((Playable) this, base.GetType());
            }
        }

        internal Playable node
        {
            get
            {
                return (Playable) this.handle;
            }
        }

        /// <summary>
        /// <para>The count of ouputs on the Playable.  Currently only 1 output is supported.</para>
        /// </summary>
        public int outputCount
        {
            get
            {
                return Playables.GetOutputCountValidated((Playable) this, base.GetType());
            }
        }

        /// <summary>
        /// <para>Current Experimental.Director.PlayState of this playable. This indicates whether the Playable is currently playing or paused.</para>
        /// </summary>
        public PlayState state
        {
            get
            {
                return Playables.GetPlayStateValidated((Playable) this, base.GetType());
            }
            set
            {
                Playables.SetPlayStateValidated((Playable) this, value, base.GetType());
            }
        }

        /// <summary>
        /// <para>Current time in seconds.</para>
        /// </summary>
        public double time
        {
            get
            {
                return Playables.GetTimeValidated((Playable) this, base.GetType());
            }
            set
            {
                Playables.SetTimeValidated((Playable) this, value, base.GetType());
            }
        }
    }
}

