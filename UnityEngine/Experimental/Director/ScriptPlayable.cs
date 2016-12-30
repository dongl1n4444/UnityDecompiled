namespace UnityEngine.Experimental.Director
{
    using System;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Base class for all user-defined playables.</para>
    /// </summary>
    [RequiredByNativeCode]
    public abstract class ScriptPlayable : Playable
    {
        protected ScriptPlayable()
        {
        }

        /// <summary>
        /// <para>Called when the Playable is destroyed.</para>
        /// </summary>
        public virtual void OnDestroy()
        {
        }

        /// <summary>
        /// <para>Called when the PlayableGraph this Playable is owned by starts playing.</para>
        /// </summary>
        public virtual void OnGraphStart()
        {
        }

        /// <summary>
        /// <para>Called when the PlayableGraph this Playable is owned by is stopped.</para>
        /// </summary>
        public virtual void OnGraphStop()
        {
        }

        /// <summary>
        /// <para>Override this method to perform custom operations when the PlayState changes.</para>
        /// </summary>
        /// <param name="info">The current frame information.</param>
        /// <param name="newState">The new PlayState.</param>
        public virtual void OnPlayStateChanged(FrameData info, PlayState newState)
        {
        }

        /// <summary>
        /// <para>Called during evaluation of the PlayableGraph.</para>
        /// </summary>
        /// <param name="info">Information about the current frame.</param>
        public virtual void PrepareFrame(FrameData info)
        {
        }

        /// <summary>
        /// <para>The ProcessFrame is the stage at which your Playable should do its work.</para>
        /// </summary>
        /// <param name="info">Information about the current frame.</param>
        /// <param name="playerData">Data that is set on the playable output userData.</param>
        public virtual void ProcessFrame(FrameData info, object playerData)
        {
        }
    }
}

