namespace UnityEngine.Experimental.Director
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Handle representing a Playable created in a PlayableGraph. The PlayableHandle implements all general usage Playable methods.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct PlayableHandle
    {
        internal IntPtr m_Handle;
        internal int m_Version;
        public T GetObject<T>() where T: Playable
        {
            if (!this.IsValid())
            {
                return null;
            }
            Playable scriptInstance = GetScriptInstance(ref this) as Playable;
            if (scriptInstance != null)
            {
                return (scriptInstance as T);
            }
            T local2 = (T) Activator.CreateInstance(GetPlayableTypeOf(ref this));
            local2.handle = this;
            SetScriptInstance(ref this, local2);
            return local2;
        }

        /// <summary>
        /// <para>Returns a Playable representation of the handle.</para>
        /// </summary>
        /// <returns>
        /// <para>The Playable represented by this handle.</para>
        /// </returns>
        public Playable GetObject() => 
            this.GetObject<Playable>();

        private static object GetScriptInstance(ref PlayableHandle playable) => 
            INTERNAL_CALL_GetScriptInstance(ref playable);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern object INTERNAL_CALL_GetScriptInstance(ref PlayableHandle playable);
        private static void SetScriptInstance(ref PlayableHandle playable, object scriptInstance)
        {
            INTERNAL_CALL_SetScriptInstance(ref playable, scriptInstance);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetScriptInstance(ref PlayableHandle playable, object scriptInstance);
        /// <summary>
        /// <para>Returns true if the Playable is properly constructed by the PlayableGraph and has not been destroyed.</para>
        /// </summary>
        public bool IsValid() => 
            IsValidInternal(ref this);

        private static bool IsValidInternal(ref PlayableHandle playable) => 
            INTERNAL_CALL_IsValidInternal(ref playable);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_IsValidInternal(ref PlayableHandle playable);
        internal static Type GetPlayableTypeOf(ref PlayableHandle playable) => 
            INTERNAL_CALL_GetPlayableTypeOf(ref playable);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern Type INTERNAL_CALL_GetPlayableTypeOf(ref PlayableHandle playable);
        /// <summary>
        /// <para>Used to compare PlayableHandles.</para>
        /// </summary>
        public static PlayableHandle Null =>
            new PlayableHandle { m_Version=10 };
        /// <summary>
        /// <para>The PlayableGraph that created the playable.</para>
        /// </summary>
        public PlayableGraph graph
        {
            get
            {
                PlayableGraph graph = new PlayableGraph();
                GetGraphInternal(ref this, ref graph);
                return graph;
            }
        }
        /// <summary>
        /// <para>Gets and Sets the number of inputs for the  Playable.</para>
        /// </summary>
        public int inputCount
        {
            get => 
                GetInputCountInternal(ref this);
            set
            {
                SetInputCountInternal(ref this, value);
            }
        }
        /// <summary>
        /// <para>Gets and Sets the number of outputs for the  Playable.</para>
        /// </summary>
        public int outputCount
        {
            get => 
                GetOutputCountInternal(ref this);
            set
            {
                SetOutputCountInternal(ref this, value);
            }
        }
        /// <summary>
        /// <para>When playing, the time will advance in the Playable during evaluation of the graph.</para>
        /// </summary>
        public PlayState playState
        {
            get => 
                GetPlayStateInternal(ref this);
            set
            {
                SetPlayStateInternal(ref this, value);
            }
        }
        /// <summary>
        /// <para>Modulates how time is incremented when the Playable is playing.</para>
        /// </summary>
        public double speed
        {
            get => 
                GetSpeedInternal(ref this);
            set
            {
                SetSpeedInternal(ref this, value);
            }
        }
        /// <summary>
        /// <para>The current  time of the Playable.</para>
        /// </summary>
        public double time
        {
            get => 
                GetTimeInternal(ref this);
            set
            {
                SetTimeInternal(ref this, value);
            }
        }
        /// <summary>
        /// <para>A flag indicating that a playable has completed its operation.</para>
        /// </summary>
        public bool isDone
        {
            get => 
                InternalGetDone(ref this);
            set
            {
                InternalSetDone(ref this, value);
            }
        }
        internal bool canChangeInputs =>
            CanChangeInputsInternal(ref this);
        internal bool canSetWeights =>
            CanSetWeightsInternal(ref this);
        internal bool canDestroy =>
            CanDestroyInternal(ref this);
        private static bool CanChangeInputsInternal(ref PlayableHandle playable) => 
            INTERNAL_CALL_CanChangeInputsInternal(ref playable);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_CanChangeInputsInternal(ref PlayableHandle playable);
        private static bool CanSetWeightsInternal(ref PlayableHandle playable) => 
            INTERNAL_CALL_CanSetWeightsInternal(ref playable);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_CanSetWeightsInternal(ref PlayableHandle playable);
        private static bool CanDestroyInternal(ref PlayableHandle playable) => 
            INTERNAL_CALL_CanDestroyInternal(ref playable);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_CanDestroyInternal(ref PlayableHandle playable);
        private static PlayState GetPlayStateInternal(ref PlayableHandle playable) => 
            INTERNAL_CALL_GetPlayStateInternal(ref playable);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern PlayState INTERNAL_CALL_GetPlayStateInternal(ref PlayableHandle playable);
        private static void SetPlayStateInternal(ref PlayableHandle playable, PlayState playState)
        {
            INTERNAL_CALL_SetPlayStateInternal(ref playable, playState);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetPlayStateInternal(ref PlayableHandle playable, PlayState playState);
        private static double GetSpeedInternal(ref PlayableHandle playable) => 
            INTERNAL_CALL_GetSpeedInternal(ref playable);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern double INTERNAL_CALL_GetSpeedInternal(ref PlayableHandle playable);
        private static void SetSpeedInternal(ref PlayableHandle playable, double speed)
        {
            INTERNAL_CALL_SetSpeedInternal(ref playable, speed);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetSpeedInternal(ref PlayableHandle playable, double speed);
        private static double GetTimeInternal(ref PlayableHandle playable) => 
            INTERNAL_CALL_GetTimeInternal(ref playable);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern double INTERNAL_CALL_GetTimeInternal(ref PlayableHandle playable);
        private static void SetTimeInternal(ref PlayableHandle playable, double time)
        {
            INTERNAL_CALL_SetTimeInternal(ref playable, time);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetTimeInternal(ref PlayableHandle playable, double time);
        private static bool InternalGetDone(ref PlayableHandle playable) => 
            INTERNAL_CALL_InternalGetDone(ref playable);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_InternalGetDone(ref PlayableHandle playable);
        private static void InternalSetDone(ref PlayableHandle playable, bool isDone)
        {
            INTERNAL_CALL_InternalSetDone(ref playable, isDone);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_InternalSetDone(ref PlayableHandle playable, bool isDone);
        /// <summary>
        /// <para>The duration of the Playable in seconds.</para>
        /// </summary>
        public double duration
        {
            get => 
                GetDurationInternal(ref this);
            set
            {
                SetDurationInternal(ref this, value);
            }
        }
        private static double GetDurationInternal(ref PlayableHandle playable) => 
            INTERNAL_CALL_GetDurationInternal(ref playable);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern double INTERNAL_CALL_GetDurationInternal(ref PlayableHandle playable);
        private static void SetDurationInternal(ref PlayableHandle playable, double duration)
        {
            INTERNAL_CALL_SetDurationInternal(ref playable, duration);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetDurationInternal(ref PlayableHandle playable, double duration);
        private static void GetGraphInternal(ref PlayableHandle playable, ref PlayableGraph graph)
        {
            INTERNAL_CALL_GetGraphInternal(ref playable, ref graph);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetGraphInternal(ref PlayableHandle playable, ref PlayableGraph graph);
        private static int GetInputCountInternal(ref PlayableHandle playable) => 
            INTERNAL_CALL_GetInputCountInternal(ref playable);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern int INTERNAL_CALL_GetInputCountInternal(ref PlayableHandle playable);
        private static void SetInputCountInternal(ref PlayableHandle playable, int count)
        {
            INTERNAL_CALL_SetInputCountInternal(ref playable, count);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetInputCountInternal(ref PlayableHandle playable, int count);
        private static int GetOutputCountInternal(ref PlayableHandle playable) => 
            INTERNAL_CALL_GetOutputCountInternal(ref playable);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern int INTERNAL_CALL_GetOutputCountInternal(ref PlayableHandle playable);
        private static void SetOutputCountInternal(ref PlayableHandle playable, int count)
        {
            INTERNAL_CALL_SetOutputCountInternal(ref playable, count);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetOutputCountInternal(ref PlayableHandle playable, int count);
        /// <summary>
        /// <para>Returns the PlayableHandle connected at the given input port index.</para>
        /// </summary>
        /// <param name="inputPort">The port index.</param>
        public PlayableHandle GetInput(int inputPort) => 
            GetInputInternal(ref this, inputPort);

        private static PlayableHandle GetInputInternal(ref PlayableHandle playable, int index)
        {
            PlayableHandle handle;
            INTERNAL_CALL_GetInputInternal(ref playable, index, out handle);
            return handle;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetInputInternal(ref PlayableHandle playable, int index, out PlayableHandle value);
        /// <summary>
        /// <para>Returns the PlayableHandle connected at the given ouput port index.</para>
        /// </summary>
        /// <param name="outputPort">The port index.</param>
        public PlayableHandle GetOutput(int outputPort) => 
            GetOutputInternal(ref this, outputPort);

        private static PlayableHandle GetOutputInternal(ref PlayableHandle playable, int index)
        {
            PlayableHandle handle;
            INTERNAL_CALL_GetOutputInternal(ref playable, index, out handle);
            return handle;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetOutputInternal(ref PlayableHandle playable, int index, out PlayableHandle value);
        private static void SetInputWeightFromIndexInternal(ref PlayableHandle playable, int index, float weight)
        {
            INTERNAL_CALL_SetInputWeightFromIndexInternal(ref playable, index, weight);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetInputWeightFromIndexInternal(ref PlayableHandle playable, int index, float weight);
        /// <summary>
        /// <para>Sets the weight of the Playable connected at the given input port index.</para>
        /// </summary>
        /// <param name="inputIndex">The port index.</param>
        /// <param name="weight">The weight. Should be between 0 and 1.</param>
        public bool SetInputWeight(int inputIndex, float weight)
        {
            if (this.CheckInputBounds(inputIndex))
            {
                SetInputWeightFromIndexInternal(ref this, inputIndex, weight);
                return true;
            }
            return false;
        }

        private static float GetInputWeightFromIndexInternal(ref PlayableHandle playable, int index) => 
            INTERNAL_CALL_GetInputWeightFromIndexInternal(ref playable, index);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern float INTERNAL_CALL_GetInputWeightFromIndexInternal(ref PlayableHandle playable, int index);
        /// <summary>
        /// <para>Returns the weight of the Playable connected at the given input port index.</para>
        /// </summary>
        /// <param name="inputIndex">The port index.</param>
        public float GetInputWeight(int inputIndex)
        {
            if (this.CheckInputBounds(inputIndex))
            {
                return GetInputWeightFromIndexInternal(ref this, inputIndex);
            }
            return 0f;
        }

        /// <summary>
        /// <para>Destroys the Playable associated with this PlayableHandle.</para>
        /// </summary>
        public void Destroy()
        {
            this.graph.DestroyPlayable(this);
        }

        public static bool operator ==(PlayableHandle x, PlayableHandle y) => 
            CompareVersion(x, y);

        public static bool operator !=(PlayableHandle x, PlayableHandle y) => 
            !CompareVersion(x, y);

        public override bool Equals(object p) => 
            ((p is PlayableHandle) && CompareVersion(this, (PlayableHandle) p));

        public override int GetHashCode() => 
            (this.m_Handle.GetHashCode() ^ this.m_Version.GetHashCode());

        internal static bool CompareVersion(PlayableHandle lhs, PlayableHandle rhs) => 
            ((lhs.m_Handle == rhs.m_Handle) && (lhs.m_Version == rhs.m_Version));

        internal bool CheckInputBounds(int inputIndex) => 
            this.CheckInputBounds(inputIndex, false);

        internal bool CheckInputBounds(int inputIndex, bool acceptAny)
        {
            if ((inputIndex != -1) || !acceptAny)
            {
                if (inputIndex < 0)
                {
                    throw new IndexOutOfRangeException("Index must be greater than 0");
                }
                if (this.inputCount <= inputIndex)
                {
                    object[] objArray1 = new object[] { "inputIndex ", inputIndex, " is greater than the number of available inputs (", this.inputCount, ")." };
                    throw new IndexOutOfRangeException(string.Concat(objArray1));
                }
            }
            return true;
        }
    }
}

