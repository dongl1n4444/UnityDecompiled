namespace UnityEngine.Playables
{
    using System;
    using UnityEngine;

    public interface IPlayableAsset
    {
        /// <summary>
        /// <para>Implement this method to have your asset inject playables into the given graph.</para>
        /// </summary>
        /// <param name="graph">The graph to inject playables into.</param>
        /// <param name="owner">The game object which initiated the build.</param>
        /// <returns>
        /// <para>The playable injected into the graph, or the root playable if multiple playables are injected.</para>
        /// </returns>
        PlayableHandle CreatePlayable(PlayableGraph graph, GameObject owner);

        /// <summary>
        /// <para>Duration in seconds.</para>
        /// </summary>
        double duration { get; }
    }
}

