namespace UnityEngine.Playables
{
    using System;

    public interface IPlayable
    {
        /// <summary>
        /// <para>The PlayableHandle for this playable.</para>
        /// </summary>
        PlayableHandle playableHandle { get; set; }
    }
}

