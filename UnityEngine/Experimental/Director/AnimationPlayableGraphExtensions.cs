namespace UnityEngine.Experimental.Director
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Extends PlayableGraph for Animation.</para>
    /// </summary>
    public static class AnimationPlayableGraphExtensions
    {
        /// <summary>
        /// <para>Creates an AnimationClipPlayable in the PlayableGraph.</para>
        /// </summary>
        /// <param name="graph">The PlayableGraph object.</param>
        /// <param name="clip">The AnimationClip that will be added in the graph.</param>
        /// <returns>
        /// <para>A PlayableHandle on the created Playable.</para>
        /// </returns>
        public static PlayableHandle CreateAnimationClipPlayable(this PlayableGraph graph, AnimationClip clip)
        {
            PlayableHandle @null = PlayableHandle.Null;
            if (!InternalCreateAnimationClipPlayable(ref graph, clip, ref @null))
            {
                return PlayableHandle.Null;
            }
            return @null;
        }

        [ExcludeFromDocs]
        internal static PlayableHandle CreateAnimationLayerMixerPlayable(this PlayableGraph graph)
        {
            int inputCount = 0;
            return graph.CreateAnimationLayerMixerPlayable(inputCount);
        }

        internal static PlayableHandle CreateAnimationLayerMixerPlayable(this PlayableGraph graph, [DefaultValue("0")] int inputCount)
        {
            PlayableHandle @null = PlayableHandle.Null;
            if (!InternalCreateAnimationLayerMixerPlayable(ref graph, ref @null))
            {
                return PlayableHandle.Null;
            }
            @null.inputCount = inputCount;
            return @null;
        }

        [ExcludeFromDocs]
        public static PlayableHandle CreateAnimationMixerPlayable(this PlayableGraph graph)
        {
            bool normalizeWeights = false;
            int inputCount = 0;
            return graph.CreateAnimationMixerPlayable(inputCount, normalizeWeights);
        }

        [ExcludeFromDocs]
        public static PlayableHandle CreateAnimationMixerPlayable(this PlayableGraph graph, int inputCount)
        {
            bool normalizeWeights = false;
            return graph.CreateAnimationMixerPlayable(inputCount, normalizeWeights);
        }

        /// <summary>
        /// <para>Creates an AnimationMixerPlayable in the PlayableGraph.</para>
        /// </summary>
        /// <param name="inputCount">The number of inputs that the mixer will update.</param>
        /// <param name="normalizeWeights">Set to true if you want the system to force a weight normalization of the inputs. If true, the sum of all the inputs weights will always be 1.0.</param>
        /// <param name="graph">The PlayableGraph object.</param>
        /// <returns>
        /// <para>A PlayableHandle on the created Playable.</para>
        /// </returns>
        public static PlayableHandle CreateAnimationMixerPlayable(this PlayableGraph graph, [DefaultValue("0")] int inputCount, [DefaultValue("false")] bool normalizeWeights)
        {
            PlayableHandle @null = PlayableHandle.Null;
            if (!InternalCreateAnimationMixerPlayable(ref graph, inputCount, normalizeWeights, ref @null))
            {
                return PlayableHandle.Null;
            }
            @null.inputCount = inputCount;
            return @null;
        }

        internal static PlayableHandle CreateAnimationMotionXToDeltaPlayable(this PlayableGraph graph)
        {
            PlayableHandle @null = PlayableHandle.Null;
            if (!InternalCreateAnimationMotionXToDeltaPlayable(ref graph, ref @null))
            {
                return PlayableHandle.Null;
            }
            @null.inputCount = 1;
            return @null;
        }

        internal static PlayableHandle CreateAnimationOffsetPlayable(this PlayableGraph graph, Vector3 position, Quaternion rotation, int inputCount)
        {
            PlayableHandle @null = PlayableHandle.Null;
            if (!InternalCreateAnimationOffsetPlayable(ref graph, position, rotation, ref @null))
            {
                return PlayableHandle.Null;
            }
            @null.inputCount = inputCount;
            return @null;
        }

        /// <summary>
        /// <para>Creates an AnimationPlayableOutput in the PlayableGraph. When the AnimationPlayableOutput.sourcePlayable is set, the Animator will be playing the Playable.</para>
        /// </summary>
        /// <param name="name">The name of output.</param>
        /// <param name="target">The target that will Play the AnimationPlayableOutput.sourcePlayable.</param>
        /// <param name="graph">The PlayableGraph object.</param>
        /// <returns>
        /// <para>A PlayableHandle on the created Playable.</para>
        /// </returns>
        public static AnimationPlayableOutput CreateAnimationOutput(this PlayableGraph graph, string name, Animator target)
        {
            AnimationPlayableOutput output = new AnimationPlayableOutput();
            if (!InternalCreateAnimationOutput(ref graph, name, out output.m_Output))
            {
                return AnimationPlayableOutput.Null;
            }
            output.target = target;
            return output;
        }

        /// <summary>
        /// <para>Creates an AnimatorControllerPlayable in the PlayableGraph.</para>
        /// </summary>
        /// <param name="controller">The RuntimeAnimatorController that will be added in the graph.</param>
        /// <param name="graph">The PlayableGraph object.</param>
        /// <returns>
        /// <para>A PlayableHandle on the created Playable.</para>
        /// </returns>
        public static PlayableHandle CreateAnimatorControllerPlayable(this PlayableGraph graph, RuntimeAnimatorController controller)
        {
            PlayableHandle @null = PlayableHandle.Null;
            if (!InternalCreateAnimatorControllerPlayable(ref graph, controller, ref @null))
            {
                return PlayableHandle.Null;
            }
            return @null;
        }

        /// <summary>
        /// <para>Destroys the PlayableOutput.</para>
        /// </summary>
        /// <param name="output">The output that will be destroyed.</param>
        /// <param name="graph">The PlayableGraph object.</param>
        public static void DestroyOutput(this PlayableGraph graph, AnimationPlayableOutput output)
        {
            InternalDestroyOutput(ref graph, ref output.m_Output);
        }

        /// <summary>
        /// <para>Returns the AnimationPlayableOutput at the given index.</para>
        /// </summary>
        /// <param name="index">The index of the AnimationPlayableOutput.</param>
        /// <param name="graph"></param>
        public static AnimationPlayableOutput GetAnimationOutput(this PlayableGraph graph, int index)
        {
            AnimationPlayableOutput output = new AnimationPlayableOutput();
            if (!InternalGetAnimationOutput(ref graph, index, out output.m_Output))
            {
                return AnimationPlayableOutput.Null;
            }
            return output;
        }

        /// <summary>
        /// <para>Gets the number of AnimationPlayableOutput in the PlayableGraph.</para>
        /// </summary>
        /// <param name="graph"></param>
        public static int GetAnimationOutputCount(this PlayableGraph graph) => 
            InternalAnimationOutputCount(ref graph);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_InternalCreateAnimationClipPlayable(ref PlayableGraph graph, AnimationClip clip, ref PlayableHandle handle);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_InternalCreateAnimationLayerMixerPlayable(ref PlayableGraph graph, ref PlayableHandle handle);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_InternalCreateAnimationMixerPlayable(ref PlayableGraph graph, int inputCount, bool normalizeWeights, ref PlayableHandle handle);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_InternalCreateAnimationMotionXToDeltaPlayable(ref PlayableGraph graph, ref PlayableHandle handle);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_InternalCreateAnimationOffsetPlayable(ref PlayableGraph graph, ref Vector3 position, ref Quaternion rotation, ref PlayableHandle handle);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_InternalCreateAnimatorControllerPlayable(ref PlayableGraph graph, RuntimeAnimatorController controller, ref PlayableHandle handle);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern int InternalAnimationOutputCount(ref PlayableGraph graph);
        private static bool InternalCreateAnimationClipPlayable(ref PlayableGraph graph, AnimationClip clip, ref PlayableHandle handle) => 
            INTERNAL_CALL_InternalCreateAnimationClipPlayable(ref graph, clip, ref handle);

        private static bool InternalCreateAnimationLayerMixerPlayable(ref PlayableGraph graph, ref PlayableHandle handle) => 
            INTERNAL_CALL_InternalCreateAnimationLayerMixerPlayable(ref graph, ref handle);

        private static bool InternalCreateAnimationMixerPlayable(ref PlayableGraph graph, int inputCount, bool normalizeWeights, ref PlayableHandle handle) => 
            INTERNAL_CALL_InternalCreateAnimationMixerPlayable(ref graph, inputCount, normalizeWeights, ref handle);

        private static bool InternalCreateAnimationMotionXToDeltaPlayable(ref PlayableGraph graph, ref PlayableHandle handle) => 
            INTERNAL_CALL_InternalCreateAnimationMotionXToDeltaPlayable(ref graph, ref handle);

        private static bool InternalCreateAnimationOffsetPlayable(ref PlayableGraph graph, Vector3 position, Quaternion rotation, ref PlayableHandle handle) => 
            INTERNAL_CALL_InternalCreateAnimationOffsetPlayable(ref graph, ref position, ref rotation, ref handle);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool InternalCreateAnimationOutput(ref PlayableGraph graph, string name, out PlayableOutput output);
        private static bool InternalCreateAnimatorControllerPlayable(ref PlayableGraph graph, RuntimeAnimatorController controller, ref PlayableHandle handle) => 
            INTERNAL_CALL_InternalCreateAnimatorControllerPlayable(ref graph, controller, ref handle);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void InternalDestroyOutput(ref PlayableGraph graph, ref PlayableOutput output);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool InternalGetAnimationOutput(ref PlayableGraph graph, int index, out PlayableOutput output);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void InternalSyncUpdateAndTimeMode(ref PlayableGraph graph, Animator animator);
        internal static void SyncUpdateAndTimeMode(this PlayableGraph graph, Animator animator)
        {
            InternalSyncUpdateAndTimeMode(ref graph, animator);
        }
    }
}

