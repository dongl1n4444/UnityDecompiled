namespace UnityEngine.Playables
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// <para>Implements high-level utility methods to simplify use of the Playable API with Animations.</para>
    /// </summary>
    public class AnimationPlayableUtilities
    {
        /// <summary>
        /// <para>Plays the Playable on  the given Animator.</para>
        /// </summary>
        /// <param name="animator">Target Animator.</param>
        /// <param name="playable">The Playable that will be played.</param>
        /// <param name="graph">The Graph that owns the Playable.</param>
        public static void Play(Animator animator, PlayableHandle playable, PlayableGraph graph)
        {
            AnimationPlayableOutput output = graph.CreateAnimationOutput("AnimationClip", animator);
            output.sourcePlayable = playable;
            output.sourceInputPort = 0;
            graph.SyncUpdateAndTimeMode(animator);
            graph.Play();
        }

        public static PlayableHandle PlayAnimatorController(Animator animator, RuntimeAnimatorController controller, out PlayableGraph graph)
        {
            graph = PlayableGraph.CreateGraph();
            AnimationPlayableOutput output = graph.CreateAnimationOutput("AnimatorControllerPlayable", animator);
            PlayableHandle handle = graph.CreateAnimatorControllerPlayable(controller);
            output.sourcePlayable = handle;
            graph.SyncUpdateAndTimeMode(animator);
            graph.Play();
            return handle;
        }

        public static PlayableHandle PlayClip(Animator animator, AnimationClip clip, out PlayableGraph graph)
        {
            graph = PlayableGraph.CreateGraph();
            AnimationPlayableOutput output = graph.CreateAnimationOutput("AnimationClip", animator);
            PlayableHandle handle = graph.CreateAnimationClipPlayable(clip);
            output.sourcePlayable = handle;
            graph.SyncUpdateAndTimeMode(animator);
            graph.Play();
            return handle;
        }

        public static PlayableHandle PlayLayerMixer(Animator animator, int inputCount, out PlayableGraph graph)
        {
            graph = PlayableGraph.CreateGraph();
            AnimationPlayableOutput output = graph.CreateAnimationOutput("Mixer", animator);
            PlayableHandle handle = graph.CreateAnimationLayerMixerPlayable(inputCount);
            output.sourcePlayable = handle;
            graph.SyncUpdateAndTimeMode(animator);
            graph.Play();
            return handle;
        }

        public static PlayableHandle PlayMixer(Animator animator, int inputCount, out PlayableGraph graph)
        {
            graph = PlayableGraph.CreateGraph();
            AnimationPlayableOutput output = graph.CreateAnimationOutput("Mixer", animator);
            PlayableHandle handle = graph.CreateAnimationMixerPlayable(inputCount);
            output.sourcePlayable = handle;
            graph.SyncUpdateAndTimeMode(animator);
            graph.Play();
            return handle;
        }
    }
}

