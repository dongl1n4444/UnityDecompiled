namespace UnityEngine.Playables
{
    using System;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Base class for all user-defined playables.</para>
    /// </summary>
    [Serializable, RequiredByNativeCode]
    public abstract class ScriptPlayable : IPlayable, IScriptPlayable, ICloneable
    {
        /// <summary>
        /// <para>Returns the PlayableHandle for this playable.</para>
        /// </summary>
        public PlayableHandle handle;

        protected ScriptPlayable()
        {
        }

        /// <summary>
        /// <para>Clones a ScriptPlayable.</para>
        /// </summary>
        /// <returns>
        /// <para>The cloned object.</para>
        /// </returns>
        public virtual object Clone()
        {
            ScriptPlayable playable = (ScriptPlayable) base.MemberwiseClone();
            playable.handle = PlayableHandle.Null;
            return playable;
        }

        /// <summary>
        /// <para>Is the playable a valid part of a valid PlayableGraph.</para>
        /// </summary>
        public bool IsValid() => 
            this.handle.IsValid();

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

        public static implicit operator PlayableHandle(ScriptPlayable b) => 
            b.handle;

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

        PlayableHandle IPlayable.playableHandle
        {
            get => 
                this.handle;
            set
            {
                this.handle = value;
            }
        }
    }
}

