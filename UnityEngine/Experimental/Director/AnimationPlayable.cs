namespace UnityEngine.Experimental.Director
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Base class for all animation related Playable classes.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct AnimationPlayable
    {
        internal Playable handle;
        internal Playable node
        {
            get
            {
                return this.handle;
            }
        }
        /// <summary>
        /// <para>Call this method to release the resources allocated by the Playable.</para>
        /// </summary>
        public void Destroy()
        {
            this.node.Destroy();
        }

        /// <summary>
        /// <para>Adds an Playable as an input.</para>
        /// </summary>
        /// <param name="input">The [[Playable] to connect.</param>
        /// <returns>
        /// <para>Returns the index of the port the playable was connected to.</para>
        /// </returns>
        public unsafe int AddInput(Playable input)
        {
            if (!Playable.Connect(input, *((Playable*) this), -1, -1))
            {
                throw new InvalidOperationException("AddInput Failed. Either the connected playable is incompatible or this AnimationPlayable type doesn't support adding inputs");
            }
            return (this.inputCount - 1);
        }

        /// <summary>
        /// <para>Sets an Playable as an input.</para>
        /// </summary>
        /// <param name="source">Playable to be used as input.</param>
        /// <param name="index">Index of the input.</param>
        /// <returns>
        /// <para>Returns false if the operation could not be completed.</para>
        /// </returns>
        public unsafe bool SetInput(Playable source, int index)
        {
            if (!this.node.CheckInputBounds(index))
            {
                return false;
            }
            if (this.GetInput(index).IsValid())
            {
                Playable.Disconnect(*((Playable*) this), index);
            }
            return Playable.Connect(source, *((Playable*) this), -1, index);
        }

        public unsafe bool SetInputs(IEnumerable<Playable> sources)
        {
            for (int i = 0; i < this.inputCount; i++)
            {
                Playable.Disconnect(*((Playable*) this), i);
            }
            bool flag = false;
            int targetInputPort = 0;
            foreach (Playable playable in sources)
            {
                if (targetInputPort < this.inputCount)
                {
                    flag |= Playable.Connect(playable, *((Playable*) this), -1, targetInputPort);
                }
                else
                {
                    flag |= Playable.Connect(playable, *((Playable*) this), -1, -1);
                }
                this.node.SetInputWeight(targetInputPort, 1f);
                targetInputPort++;
            }
            for (int j = targetInputPort; j < this.inputCount; j++)
            {
                this.node.SetInputWeight(j, 0f);
            }
            return flag;
        }

        /// <summary>
        /// <para>Removes a playable from the list of inputs.</para>
        /// </summary>
        /// <param name="index">Index of the playable to remove.</param>
        /// <returns>
        /// <para>Returns false if the removal could not be removed because it wasn't found.</para>
        /// </returns>
        public unsafe bool RemoveInput(int index)
        {
            if (!Playables.CheckInputBounds(*((Playable*) this), index))
            {
                return false;
            }
            Playable.Disconnect(*((Playable*) this), index);
            return true;
        }

        /// <summary>
        /// <para>Removes a playable from the list of inputs.</para>
        /// </summary>
        /// <param name="playable">The Playable to remove.</param>
        /// <returns>
        /// <para>Returns false if the removal could not be removed because it wasn't found.</para>
        /// </returns>
        public unsafe bool RemoveInput(Playable playable)
        {
            for (int i = 0; i < this.inputCount; i++)
            {
                if (this.GetInput(i) == playable)
                {
                    Playable.Disconnect(*((Playable*) this), i);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// <para>Disconnects all input playables.</para>
        /// </summary>
        /// <returns>
        /// <para>Returns false if the removal fails.</para>
        /// </returns>
        public bool RemoveAllInputs()
        {
            int inputCount = this.node.inputCount;
            for (int i = 0; i < inputCount; i++)
            {
                this.RemoveInput(i);
            }
            return true;
        }

        /// <summary>
        /// <para>A Null AnimationPlayable used to create empty input connections.</para>
        /// </summary>
        public static AnimationPlayable Null
        {
            get
            {
                return new AnimationPlayable { handle = { m_Version = 10 } };
            }
        }
        public static bool operator ==(AnimationPlayable x, Playable y)
        {
            return Playables.Equals((Playable) x, y);
        }

        public static bool operator !=(AnimationPlayable x, Playable y)
        {
            return !Playables.Equals((Playable) x, y);
        }

        public override unsafe bool Equals(object p)
        {
            return Playables.Equals(*((Playable*) this), p);
        }

        public override int GetHashCode()
        {
            return this.node.GetHashCode();
        }

        public static implicit operator Playable(AnimationPlayable b)
        {
            return b.node;
        }

        /// <summary>
        /// <para>Returns true if the Playable is valid. A playable can be invalid if it was disposed. This is different from a Null playable..</para>
        /// </summary>
        public unsafe bool IsValid()
        {
            return Playables.IsValid(*((Playable*) this));
        }

        public T CastTo<T>() where T: struct
        {
            return this.handle.CastTo<T>();
        }

        /// <summary>
        /// <para>The count of inputs on the Playable. This count includes slots that aren't connected to anything.</para>
        /// </summary>
        public int inputCount
        {
            get
            {
                return Playables.GetInputCountValidated(*((Playable*) this), base.GetType());
            }
        }
        /// <summary>
        /// <para>Returns the Playable connected at the specified index.</para>
        /// </summary>
        /// <param name="inputPort">Index of the input.</param>
        /// <returns>
        /// <para>Playable connected at the index specified, or null if the index is valid but is not connected to anything. This happens if there was once a Playable connected at the index, but was disconnected.</para>
        /// </returns>
        public unsafe Playable GetInput(int inputPort)
        {
            return Playables.GetInputValidated(*((Playable*) this), inputPort, base.GetType());
        }

        /// <summary>
        /// <para>The count of ouputs on the Playable.  Currently only 1 output is supported.</para>
        /// </summary>
        public int outputCount
        {
            get
            {
                return Playables.GetOutputCountValidated(*((Playable*) this), base.GetType());
            }
        }
        /// <summary>
        /// <para>Returns the Playable connected at the specified output index.</para>
        /// </summary>
        /// <param name="outputPort">Index of the output.</param>
        /// <returns>
        /// <para>Playable connected at the output index specified, or null if the index is valid but is not connected to anything. This happens if there was once a Playable connected at the index, but was disconnected.</para>
        /// </returns>
        public unsafe Playable GetOutput(int outputPort)
        {
            return Playables.GetOutputValidated(*((Playable*) this), outputPort, base.GetType());
        }

        /// <summary>
        /// <para>Get the weight of the Playable at a specified index.</para>
        /// </summary>
        /// <param name="index">Index of the input.</param>
        /// <returns>
        /// <para>Weight of the input Playable. Returns -1 if there is no input connected at this input index.</para>
        /// </returns>
        public unsafe float GetInputWeight(int index)
        {
            return Playables.GetInputWeightValidated(*((Playable*) this), index, base.GetType());
        }

        /// <summary>
        /// <para>Set the weight of an input.</para>
        /// </summary>
        /// <param name="inputIndex"></param>
        /// <param name="weight"></param>
        public unsafe void SetInputWeight(int inputIndex, float weight)
        {
            Playables.SetInputWeightValidated(*((Playable*) this), inputIndex, weight, base.GetType());
        }

        /// <summary>
        /// <para>Current Experimental.Director.PlayState of this playable. This indicates whether the Playable is currently playing or paused.</para>
        /// </summary>
        public PlayState state
        {
            get
            {
                return Playables.GetPlayStateValidated(*((Playable*) this), base.GetType());
            }
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
            get
            {
                return Playables.GetTimeValidated(*((Playable*) this), base.GetType());
            }
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
            get
            {
                return Playables.GetDurationValidated(*((Playable*) this), base.GetType());
            }
            set
            {
                Playables.SetDurationValidated(*((Playable*) this), value, base.GetType());
            }
        }
    }
}

