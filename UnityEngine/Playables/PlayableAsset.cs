namespace UnityEngine.Playables
{
    using System;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>An base class for assets that can be used to instatiate a Playable at runtime.</para>
    /// </summary>
    [Serializable, RequiredByNativeCode]
    public abstract class PlayableAsset : ScriptableObject, IPlayableAsset
    {
        protected PlayableAsset()
        {
        }

        /// <summary>
        /// <para>Implement this method to have your asset inject playables into the given graph.</para>
        /// </summary>
        /// <param name="graph">The graph to inject playables into.</param>
        /// <param name="owner">The game object which initiated the build.</param>
        /// <returns>
        /// <para>The playable injected into the graph, or the root playable if multiple playables are injected.</para>
        /// </returns>
        public abstract PlayableHandle CreatePlayable(PlayableGraph graph, GameObject owner);
        internal unsafe void InternalGetDuration(IntPtr ptrToDouble)
        {
            double duration = this.duration;
            *((double*) ptrToDouble.ToPointer()) = duration;
        }

        /// <summary>
        /// <para>The playback duration in seconds of the instantiated Playable.</para>
        /// </summary>
        public virtual double duration =>
            PlayableBinding.DefaultDuration;
    }
}

