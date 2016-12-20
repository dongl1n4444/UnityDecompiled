namespace UnityEngine.Experimental.Director
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Playables are customizable runtime objects that can be connected together in a tree to create complex behaviours.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct Playable
    {
        internal IntPtr m_Handle;
        internal int m_Version;
        /// <summary>
        /// <para>Call this method to release the resources associated to this Playable.</para>
        /// </summary>
        public void Destroy()
        {
            Playables.InternalDestroy(ref this);
        }

        /// <summary>
        /// <para>Returns true if the Playable is valid. A playable can be invalid if it was disposed. This is different from a Null playable.</para>
        /// </summary>
        public bool IsValid()
        {
            return IsValidInternal(ref this);
        }

        private static bool IsValidInternal(ref Playable playable)
        {
            return INTERNAL_CALL_IsValidInternal(ref playable);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool INTERNAL_CALL_IsValidInternal(ref Playable playable);
        public T CastTo<T>() where T: struct
        {
            return (T) Playables.CastToInternal(typeof(T), this.m_Handle, this.m_Version);
        }

        /// <summary>
        /// <para>Use GetTypeOf to get the Type of Playable.</para>
        /// </summary>
        /// <param name="playable">Playable you wish to know the type.</param>
        /// <returns>
        /// <para>The Type of Playable.</para>
        /// </returns>
        public static Type GetTypeOf(Playable playable)
        {
            return Playables.GetTypeOfInternal(playable.m_Handle, playable.m_Version);
        }

        /// <summary>
        /// <para>A Null Playable used to create empty input connections.</para>
        /// </summary>
        public static Playable Null
        {
            get
            {
                return new Playable { m_Version = 10 };
            }
        }
        public static bool Connect(Playable source, Playable target)
        {
            return Connect(source, target, -1, -1);
        }

        /// <summary>
        /// <para>Connects two Playables together.</para>
        /// </summary>
        /// <param name="source">Playable to be used as input.</param>
        /// <param name="target">Playable on which the input will be connected.</param>
        /// <param name="sourceOutputPort">Optional index of the output on the source Playable.</param>
        /// <param name="targetInputPort">Optional index of the input on the target Playable.</param>
        /// <returns>
        /// <para>Returns false if the operation could not be completed.</para>
        /// </returns>
        public static bool Connect(Playable source, Playable target, int sourceOutputPort, int targetInputPort)
        {
            return Playables.ConnectInternal(ref source, ref target, sourceOutputPort, targetInputPort);
        }

        /// <summary>
        /// <para>Disconnects an input from a Playable.</para>
        /// </summary>
        /// <param name="right">Playable from which the input will be disconnected.</param>
        /// <param name="inputPort">Index of the input to disconnect.</param>
        /// <param name="target"></param>
        public static void Disconnect(Playable target, int inputPort)
        {
            if (target.CheckInputBounds(inputPort))
            {
                Playables.DisconnectInternal(ref target, inputPort);
            }
        }

        public static T Create<T>() where T: CustomAnimationPlayable, new()
        {
            return (InternalCreate(typeof(T)) as T);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern object InternalCreate(Type type);
        /// <summary>
        /// <para>The count of inputs on the Playable. This count includes slots that aren't connected to anything. This is equivalent to, but much faster than calling GetInputs().Length.</para>
        /// </summary>
        public int inputCount
        {
            get
            {
                return GetInputCountInternal(ref this);
            }
        }
        /// <summary>
        /// <para>The count of ouputs on the Playable.  Currently only 1 output is supported.</para>
        /// </summary>
        public int outputCount
        {
            get
            {
                return GetOutputCountInternal(ref this);
            }
        }
        /// <summary>
        /// <para>Current Experimental.Director.PlayState of this playable. This indicates whether the Playable is currently playing or paused.</para>
        /// </summary>
        public PlayState state
        {
            get
            {
                return GetPlayStateInternal(ref this);
            }
            set
            {
                SetPlayStateInternal(ref this, value);
            }
        }
        /// <summary>
        /// <para>Current local time for this Playable.</para>
        /// </summary>
        public double time
        {
            get
            {
                return GetTimeInternal(ref this);
            }
            set
            {
                SetTimeInternal(ref this, value);
            }
        }
        internal bool canChangeInputs
        {
            get
            {
                return CanChangeInputsInternal(ref this);
            }
        }
        internal bool canSetWeights
        {
            get
            {
                return CanSetWeightsInternal(ref this);
            }
        }
        internal bool canDestroy
        {
            get
            {
                return CanDestroyInternal(ref this);
            }
        }
        private static bool CanChangeInputsInternal(ref Playable playable)
        {
            return INTERNAL_CALL_CanChangeInputsInternal(ref playable);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool INTERNAL_CALL_CanChangeInputsInternal(ref Playable playable);
        private static bool CanSetWeightsInternal(ref Playable playable)
        {
            return INTERNAL_CALL_CanSetWeightsInternal(ref playable);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool INTERNAL_CALL_CanSetWeightsInternal(ref Playable playable);
        private static bool CanDestroyInternal(ref Playable playable)
        {
            return INTERNAL_CALL_CanDestroyInternal(ref playable);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool INTERNAL_CALL_CanDestroyInternal(ref Playable playable);
        private static PlayState GetPlayStateInternal(ref Playable playable)
        {
            return INTERNAL_CALL_GetPlayStateInternal(ref playable);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern PlayState INTERNAL_CALL_GetPlayStateInternal(ref Playable playable);
        private static void SetPlayStateInternal(ref Playable playable, PlayState playState)
        {
            INTERNAL_CALL_SetPlayStateInternal(ref playable, playState);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_SetPlayStateInternal(ref Playable playable, PlayState playState);
        private static double GetTimeInternal(ref Playable playable)
        {
            return INTERNAL_CALL_GetTimeInternal(ref playable);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern double INTERNAL_CALL_GetTimeInternal(ref Playable playable);
        private static void SetTimeInternal(ref Playable playable, double time)
        {
            INTERNAL_CALL_SetTimeInternal(ref playable, time);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_SetTimeInternal(ref Playable playable, double time);
        /// <summary>
        /// <para>Duration in seconds.</para>
        /// </summary>
        public double duration
        {
            get
            {
                return GetDurationInternal(ref this);
            }
            set
            {
                SetDurationInternal(ref this, value);
            }
        }
        private static double GetDurationInternal(ref Playable playable)
        {
            return INTERNAL_CALL_GetDurationInternal(ref playable);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern double INTERNAL_CALL_GetDurationInternal(ref Playable playable);
        private static void SetDurationInternal(ref Playable playable, double duration)
        {
            INTERNAL_CALL_SetDurationInternal(ref playable, duration);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_SetDurationInternal(ref Playable playable, double duration);
        private static int GetInputCountInternal(ref Playable playable)
        {
            return INTERNAL_CALL_GetInputCountInternal(ref playable);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int INTERNAL_CALL_GetInputCountInternal(ref Playable playable);
        private static int GetOutputCountInternal(ref Playable playable)
        {
            return INTERNAL_CALL_GetOutputCountInternal(ref playable);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int INTERNAL_CALL_GetOutputCountInternal(ref Playable playable);
        /// <summary>
        /// <para>Returns a lists of the input Playables.</para>
        /// </summary>
        /// <param name="inputList">List of Playables connected. This list can include nulls if Playables were disconnected from this Playable via Playable.Disconnect.</param>
        public Playable[] GetInputs()
        {
            List<Playable> list = new List<Playable>();
            int inputCount = this.inputCount;
            for (int i = 0; i < inputCount; i++)
            {
                list.Add(this.GetInput(i));
            }
            return list.ToArray();
        }

        /// <summary>
        /// <para>Returns the Playable connected at the specified index.</para>
        /// </summary>
        /// <param name="inputPort">Index of the input.</param>
        /// <returns>
        /// <para>Playable connected at the index specified, or null if the index is valid but is not connected to anything. This happens if there was once a Playable connected at the index, but was disconnected via Playable.Disconnect.</para>
        /// </returns>
        public Playable GetInput(int inputPort)
        {
            return GetInputInternal(ref this, inputPort);
        }

        private static Playable GetInputInternal(ref Playable playable, int index)
        {
            Playable playable2;
            INTERNAL_CALL_GetInputInternal(ref playable, index, out playable2);
            return playable2;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetInputInternal(ref Playable playable, int index, out Playable value);
        /// <summary>
        /// <para>Get the list of ouputs connected on this Playable.</para>
        /// </summary>
        /// <param name="outputList">List of output Playables.</param>
        public Playable[] GetOutputs()
        {
            List<Playable> list = new List<Playable>();
            int outputCount = this.outputCount;
            for (int i = 0; i < outputCount; i++)
            {
                list.Add(this.GetOutput(i));
            }
            return list.ToArray();
        }

        /// <summary>
        /// <para>Returns the Playable connected at the specified output index.</para>
        /// </summary>
        /// <param name="outputPort">Index of the output.</param>
        /// <returns>
        /// <para>Playable connected at the output index specified, or null if the index is valid but is not connected to anything. This happens if there was once a Playable connected at the index, but was disconnected via Playable.Disconnect.</para>
        /// </returns>
        public Playable GetOutput(int outputPort)
        {
            return GetOutputInternal(ref this, outputPort);
        }

        private static Playable GetOutputInternal(ref Playable playable, int index)
        {
            Playable playable2;
            INTERNAL_CALL_GetOutputInternal(ref playable, index, out playable2);
            return playable2;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetOutputInternal(ref Playable playable, int index, out Playable value);
        private static void SetInputWeightFromIndexInternal(ref Playable playable, int index, float weight)
        {
            INTERNAL_CALL_SetInputWeightFromIndexInternal(ref playable, index, weight);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_SetInputWeightFromIndexInternal(ref Playable playable, int index, float weight);
        private static void SetInputWeightInternal(ref Playable playable, Playable input, float weight)
        {
            INTERNAL_CALL_SetInputWeightInternal(ref playable, ref input, weight);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_SetInputWeightInternal(ref Playable playable, ref Playable input, float weight);
        public void SetInputWeight(Playable input, float weight)
        {
            SetInputWeightInternal(ref this, input, weight);
        }

        /// <summary>
        /// <para>Sets the weight of an input.</para>
        /// </summary>
        /// <param name="inputIndex">Index of the input.</param>
        /// <param name="weight">Weight of the input.</param>
        /// <returns>
        /// <para>Returns false if there is no input Playable connected at that index.</para>
        /// </returns>
        public bool SetInputWeight(int inputIndex, float weight)
        {
            if (this.CheckInputBounds(inputIndex))
            {
                SetInputWeightFromIndexInternal(ref this, inputIndex, weight);
                return true;
            }
            return false;
        }

        /// <summary>
        /// <para>Get the weight of the Playable at a specified index.</para>
        /// </summary>
        /// <param name="inputIndex">Index of the Playable.</param>
        /// <param name="index"></param>
        /// <returns>
        /// <para>Weight of the input Playable. Returns -1 if there is no input connected at this input index.</para>
        /// </returns>
        public float GetInputWeight(int index)
        {
            return GetInputWeightInternal(ref this, index);
        }

        private static float GetInputWeightInternal(ref Playable playable, int index)
        {
            return INTERNAL_CALL_GetInputWeightInternal(ref playable, index);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern float INTERNAL_CALL_GetInputWeightInternal(ref Playable playable, int index);
        public static bool operator ==(Playable x, Playable y)
        {
            return CompareVersion(x, y);
        }

        public static bool operator !=(Playable x, Playable y)
        {
            return !CompareVersion(x, y);
        }

        public override bool Equals(object p)
        {
            return ((p != null) && (p.GetHashCode() == this.GetHashCode()));
        }

        public override int GetHashCode()
        {
            return (((int) this.m_Handle) ^ this.m_Version);
        }

        internal static bool CompareVersion(Playable lhs, Playable rhs)
        {
            return ((lhs.m_Handle == rhs.m_Handle) && (lhs.m_Version == rhs.m_Version));
        }

        internal bool CheckInputBounds(int inputIndex)
        {
            return this.CheckInputBounds(inputIndex, false);
        }

        internal bool CheckInputBounds(int inputIndex, bool acceptAny)
        {
            if ((inputIndex != -1) || !acceptAny)
            {
                if (inputIndex < 0)
                {
                    throw new IndexOutOfRangeException("Index must be greater than 0");
                }
                Playable[] inputs = this.GetInputs();
                if (inputs.Length <= inputIndex)
                {
                    object[] objArray1 = new object[] { "inputIndex ", inputIndex, " is greater than the number of available inputs (", inputs.Length, ")." };
                    throw new IndexOutOfRangeException(string.Concat(objArray1));
                }
            }
            return true;
        }
    }
}

