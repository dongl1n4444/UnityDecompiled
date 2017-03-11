namespace UnityEngine.Playables
{
    using System;

    public interface IScriptPlayable
    {
        /// <summary>
        /// <para>Called when the PlayableGraph this ScriptPlayable is owned by starts playing.</para>
        /// </summary>
        void OnGraphStart();
        /// <summary>
        /// <para>Called when the PlayableGraph this ScriptPlayable is owned by is stopped.</para>
        /// </summary>
        void OnGraphStop();
        /// <summary>
        /// <para>Override this method to perform custom operations when the PlayState changes.</para>
        /// </summary>
        /// <param name="info">The current frame information.</param>
        /// <param name="newState">The new PlayState.</param>
        void OnPlayStateChanged(FrameData info, PlayState newState);
        /// <summary>
        /// <para>Called during evaluation of the PlayableGraph.</para>
        /// </summary>
        /// <param name="info">Information about the current frame.</param>
        void PrepareFrame(FrameData info);
        /// <summary>
        /// <para>The ProcessFrame is the stage at which your Playable should do its work.</para>
        /// </summary>
        /// <param name="info">Information about the current frame.</param>
        /// <param name="playerData">Data that is set on the playable output userData.</param>
        void ProcessFrame(FrameData info, object playerData);
    }
}

