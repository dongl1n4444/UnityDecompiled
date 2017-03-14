namespace UnityEngine.Playables
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>The PlayableGraph is used to manage PlayableHandle creation, destruction and connections.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct PlayableGraph
    {
        internal IntPtr m_Handle;
        internal int m_Version;
        /// <summary>
        /// <para>Returns true if the PlayableGraph has been properly constructed using PlayableGraph.CreateGraph and is not deleted.</para>
        /// </summary>
        public bool IsValid() => 
            IsValidInternal(ref this);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool IsValidInternal(ref PlayableGraph graph);
        /// <summary>
        /// <para>Creates a PlayableGraph.</para>
        /// </summary>
        /// <returns>
        /// <para>The created graph.</para>
        /// </returns>
        public static PlayableGraph CreateGraph()
        {
            PlayableGraph graph = new PlayableGraph();
            InternalCreate(ref graph);
            return graph;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void InternalCreate(ref PlayableGraph graph);
        /// <summary>
        /// <para>Indicates that a graph has completed its operations.</para>
        /// </summary>
        public bool isDone =>
            InternalIsDone(ref this);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool InternalIsDone(ref PlayableGraph graph);
        /// <summary>
        /// <para>Plays the graph.</para>
        /// </summary>
        public void Play()
        {
            InternalPlay(ref this);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void InternalPlay(ref PlayableGraph graph);
        /// <summary>
        /// <para>Stops the graph, if it is playing.</para>
        /// </summary>
        public void Stop()
        {
            InternalStop(ref this);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void InternalStop(ref PlayableGraph graph);
        /// <summary>
        /// <para>Returns the number of PlayableHandle owned by the Graph.</para>
        /// </summary>
        public int playableCount =>
            InternalPlayableCount(ref this);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern int InternalPlayableCount(ref PlayableGraph graph);
        /// <summary>
        /// <para>Creates a ScriptPlayableOutput in the [PlayableGraph]].</para>
        /// </summary>
        /// <param name="name">The name of the output.</param>
        public ScriptPlayableOutput CreateScriptOutput(string name)
        {
            ScriptPlayableOutput output = new ScriptPlayableOutput();
            if (!InternalCreateScriptOutput(ref this, name, out output.m_Output))
            {
                return ScriptPlayableOutput.Null;
            }
            return output;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool InternalCreateScriptOutput(ref PlayableGraph graph, string name, out PlayableOutput output);
        /// <summary>
        /// <para>This method allows you to create custom Playable instances.</para>
        /// </summary>
        /// <returns>
        /// <para>The created Playable.</para>
        /// </returns>
        public PlayableHandle CreatePlayable()
        {
            PlayableHandle @null = PlayableHandle.Null;
            if (!InternalCreatePlayable(ref this, ref @null))
            {
                return PlayableHandle.Null;
            }
            return @null;
        }

        [ExcludeFromDocs]
        public PlayableHandle CreateGenericMixerPlayable()
        {
            int inputCount = 0;
            return this.CreateGenericMixerPlayable(inputCount);
        }

        /// <summary>
        /// <para>Creates a generic ScriptPlayable mixer.</para>
        /// </summary>
        /// <param name="inputCount">The number of input.</param>
        /// <returns>
        /// <para>The created Playable.</para>
        /// </returns>
        public PlayableHandle CreateGenericMixerPlayable([DefaultValue("0")] int inputCount)
        {
            PlayableHandle @null = PlayableHandle.Null;
            if (!InternalCreatePlayable(ref this, ref @null))
            {
                return PlayableHandle.Null;
            }
            @null.inputCount = inputCount;
            return @null;
        }

        private static bool InternalCreatePlayable(ref PlayableGraph graph, ref PlayableHandle handle) => 
            INTERNAL_CALL_InternalCreatePlayable(ref graph, ref handle);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_InternalCreatePlayable(ref PlayableGraph graph, ref PlayableHandle handle);
        /// <summary>
        /// <para>Destroys the graph.</para>
        /// </summary>
        public void Destroy()
        {
            DestroyInternal(ref this);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void DestroyInternal(ref PlayableGraph graph);
        /// <summary>
        /// <para>Connects two Playable instances, either by referencing the Playable instances themselves or by their PlayableHandles.</para>
        /// </summary>
        /// <param name="source">The source playable or its handle.</param>
        /// <param name="sourceOutputPort">The port used in the source playable.</param>
        /// <param name="destination">The destination playable or its handle.</param>
        /// <param name="destinationInputPort">The port used in the destination playable.</param>
        /// <returns>
        /// <para>Returns true if connection is successful.</para>
        /// </returns>
        public bool Connect(PlayableHandle source, int sourceOutputPort, PlayableHandle destination, int destinationInputPort) => 
            ConnectInternal(ref this, source, sourceOutputPort, destination, destinationInputPort);

        /// <summary>
        /// <para>Connects two Playable instances, either by referencing the Playable instances themselves or by their PlayableHandles.</para>
        /// </summary>
        /// <param name="source">The source playable or its handle.</param>
        /// <param name="sourceOutputPort">The port used in the source playable.</param>
        /// <param name="destination">The destination playable or its handle.</param>
        /// <param name="destinationInputPort">The port used in the destination playable.</param>
        /// <returns>
        /// <para>Returns true if connection is successful.</para>
        /// </returns>
        public bool Connect(Playable source, int sourceOutputPort, Playable destination, int destinationInputPort) => 
            ConnectInternal(ref this, source.handle, sourceOutputPort, destination.handle, destinationInputPort);

        private static bool ConnectInternal(ref PlayableGraph graph, PlayableHandle source, int sourceOutputPort, PlayableHandle destination, int destinationInputPort) => 
            INTERNAL_CALL_ConnectInternal(ref graph, ref source, sourceOutputPort, ref destination, destinationInputPort);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_ConnectInternal(ref PlayableGraph graph, ref PlayableHandle source, int sourceOutputPort, ref PlayableHandle destination, int destinationInputPort);
        /// <summary>
        /// <para>Disconnects PlayableHandle.  The connections determine the topology of the PlayableGraph and how its is evaluated.</para>
        /// </summary>
        /// <param name="playable">The source playabe or its handle.</param>
        /// <param name="inputPort">The port used in the source playable.</param>
        public void Disconnect(Playable playable, int inputPort)
        {
            PlayableHandle handle = playable.handle;
            DisconnectInternal(ref this, ref handle, inputPort);
        }

        /// <summary>
        /// <para>Disconnects PlayableHandle.  The connections determine the topology of the PlayableGraph and how its is evaluated.</para>
        /// </summary>
        /// <param name="playable">The source playabe or its handle.</param>
        /// <param name="inputPort">The port used in the source playable.</param>
        public void Disconnect(PlayableHandle playable, int inputPort)
        {
            DisconnectInternal(ref this, ref playable, inputPort);
        }

        private static void DisconnectInternal(ref PlayableGraph graph, ref PlayableHandle playable, int inputPort)
        {
            INTERNAL_CALL_DisconnectInternal(ref graph, ref playable, inputPort);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_DisconnectInternal(ref PlayableGraph graph, ref PlayableHandle playable, int inputPort);
        /// <summary>
        /// <para>Destroys the Playable associated with this PlayableHandle.</para>
        /// </summary>
        /// <param name="playable">The playable to destroy.</param>
        public void DestroyPlayable(PlayableHandle playable)
        {
            InternalDestroyPlayable(ref this, ref playable);
        }

        private static void InternalDestroyPlayable(ref PlayableGraph graph, ref PlayableHandle playable)
        {
            INTERNAL_CALL_InternalDestroyPlayable(ref graph, ref playable);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_InternalDestroyPlayable(ref PlayableGraph graph, ref PlayableHandle playable);
        /// <summary>
        /// <para>Destroys the PlayableOutput.</para>
        /// </summary>
        /// <param name="output">The output to destroy.</param>
        public void DestroyOutput(ScriptPlayableOutput output)
        {
            InternalDestroyOutput(ref this, ref output.m_Output);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void InternalDestroyOutput(ref PlayableGraph graph, ref PlayableOutput output);
        /// <summary>
        /// <para>Recursively destroys the given Playable and all children connected to its inputs.</para>
        /// </summary>
        /// <param name="playable">The playable to destroy.</param>
        public void DestroySubgraph(PlayableHandle playable)
        {
            InternalDestroySubgraph(ref this, playable);
        }

        private static void InternalDestroySubgraph(ref PlayableGraph graph, PlayableHandle playable)
        {
            INTERNAL_CALL_InternalDestroySubgraph(ref graph, ref playable);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_InternalDestroySubgraph(ref PlayableGraph graph, ref PlayableHandle playable);
        [ExcludeFromDocs]
        public void Evaluate()
        {
            float deltaTime = 0f;
            this.Evaluate(deltaTime);
        }

        /// <summary>
        /// <para>Evaluates all the PlayableOutputs in the graph, and updates all the connected Playables in the graph.</para>
        /// </summary>
        /// <param name="deltaTime">The time in seconds by which to advance each Playable in the graph.</param>
        public void Evaluate([DefaultValue("0")] float deltaTime)
        {
            InternalEvaluate(ref this, deltaTime);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void InternalEvaluate(ref PlayableGraph graph, float deltaTime);
        /// <summary>
        /// <para>Returns the number of PlayableHandle owned by the Graph that have no connected outputs.</para>
        /// </summary>
        public int rootPlayableCount =>
            InternalRootPlayableCount(ref this);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern int InternalRootPlayableCount(ref PlayableGraph graph);
        /// <summary>
        /// <para>Returns the PlayableHandle with no output connections at the given index.</para>
        /// </summary>
        /// <param name="index">The index of the root PlayableHandle.</param>
        public PlayableHandle GetRootPlayable(int index)
        {
            PlayableHandle @null = PlayableHandle.Null;
            InternalGetRootPlayable(index, ref this, ref @null);
            return @null;
        }

        internal static void InternalGetRootPlayable(int index, ref PlayableGraph graph, ref PlayableHandle handle)
        {
            INTERNAL_CALL_InternalGetRootPlayable(index, ref graph, ref handle);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_InternalGetRootPlayable(int index, ref PlayableGraph graph, ref PlayableHandle handle);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern int InternalScriptOutputCount(ref PlayableGraph graph);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool InternalGetScriptOutput(ref PlayableGraph graph, int index, out PlayableOutput output);
        private static void SetScriptInstance(ref PlayableHandle handle, object instance)
        {
            INTERNAL_CALL_SetScriptInstance(ref handle, instance);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetScriptInstance(ref PlayableHandle handle, object instance);
        /// <summary>
        /// <para>Get the number of ScriptOutputs in the graph.</para>
        /// </summary>
        public int GetScriptOutputCount() => 
            InternalScriptOutputCount(ref this);

        /// <summary>
        /// <para>Returns the ScriptPlayableOutput at the given index.</para>
        /// </summary>
        /// <param name="index">The index of the ScriptPlayableOutput.</param>
        public ScriptPlayableOutput GetScriptOutput(int index)
        {
            ScriptPlayableOutput output = new ScriptPlayableOutput();
            if (!InternalGetScriptOutput(ref this, index, out output.m_Output))
            {
                return ScriptPlayableOutput.Null;
            }
            return output;
        }

        public PlayableHandle CreateScriptPlayable<T>() where T: class, IScriptPlayable, IPlayable, new()
        {
            PlayableHandle handle = this.CreatePlayable();
            if (!handle.IsValid())
            {
                return PlayableHandle.Null;
            }
            IPlayable instance = null;
            if (typeof(ScriptableObject).IsAssignableFrom(typeof(T)))
            {
                instance = ScriptableObject.CreateInstance(typeof(T)) as T;
            }
            else
            {
                instance = Activator.CreateInstance<T>();
            }
            if (instance == null)
            {
                handle.Destroy();
                Debug.LogError("Could not create a ScriptPlayable of Type " + typeof(T).ToString());
                return PlayableHandle.Null;
            }
            SetScriptInstance(ref handle, instance);
            instance.playableHandle = handle;
            return handle;
        }

        public PlayableHandle CreateScriptMixerPlayable<T>(int inputCount) where T: class, IScriptPlayable, IPlayable, new()
        {
            PlayableHandle handle = this.CreateScriptPlayable<T>();
            if (handle.IsValid())
            {
                handle.inputCount = inputCount;
            }
            return handle;
        }

        public PlayableHandle CloneScriptPlayable(IScriptPlayable source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source parameter cannot be null");
            }
            ScriptableObject obj2 = source as ScriptableObject;
            UnityEngine.Object obj3 = source as UnityEngine.Object;
            if (obj2 != null)
            {
                return InternalCloneScriptableObjectPlayable(this, obj2, obj2.GetType());
            }
            if (obj3 != null)
            {
                return InternalCloneEngineObjectPlayable(this, obj3);
            }
            return InternalCloneObjectPlayable(this, source);
        }

        internal static PlayableHandle InternalCloneScriptableObjectPlayable(PlayableGraph graph, ScriptableObject source, System.Type type)
        {
            PlayableHandle handle = graph.CreatePlayable();
            if (!handle.IsValid())
            {
                return PlayableHandle.Null;
            }
            ScriptableObject instance = UnityEngine.Object.Instantiate<ScriptableObject>(source);
            if (instance == null)
            {
                Debug.LogError("Could not clone a ScriptPlayable of Type " + type.ToString());
                handle.Destroy();
                return PlayableHandle.Null;
            }
            SetScriptInstance(ref handle, instance);
            IPlayable playable = (IPlayable) instance;
            playable.playableHandle = handle;
            instance.hideFlags |= HideFlags.DontSave;
            return handle;
        }

        internal static PlayableHandle InternalCloneEngineObjectPlayable(PlayableGraph graph, UnityEngine.Object source)
        {
            PlayableHandle handle = graph.CreatePlayable();
            if (!handle.IsValid())
            {
                return PlayableHandle.Null;
            }
            UnityEngine.Object instance = UnityEngine.Object.Instantiate(source);
            if (instance == null)
            {
                Debug.LogError("Could not clone a ScriptPlayable of Type " + source.GetType().ToString());
                handle.Destroy();
                return PlayableHandle.Null;
            }
            SetScriptInstance(ref handle, instance);
            IPlayable playable = (IPlayable) instance;
            playable.playableHandle = handle;
            instance.hideFlags |= HideFlags.DontSave;
            return handle;
        }

        internal static PlayableHandle InternalCloneObjectPlayable(PlayableGraph graph, object source)
        {
            PlayableHandle handle = graph.CreatePlayable();
            if (!handle.IsValid())
            {
                return PlayableHandle.Null;
            }
            ICloneable cloneable = source as ICloneable;
            if (cloneable == null)
            {
                Debug.LogError("Could not clone a ScriptPlayable of Type " + source.GetType().ToString() + " as it does not implement ICloneable");
                handle.Destroy();
                return PlayableHandle.Null;
            }
            object instance = cloneable.Clone();
            if (instance == null)
            {
                Debug.LogError("Could not clone a ScriptPlayable of Type " + source.GetType().ToString());
                handle.Destroy();
                return PlayableHandle.Null;
            }
            SetScriptInstance(ref handle, instance);
            IPlayable playable = (IPlayable) instance;
            playable.playableHandle = handle;
            return handle;
        }
    }
}

