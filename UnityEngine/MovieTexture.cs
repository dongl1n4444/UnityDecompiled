namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>Movie Textures are textures onto which movies are played back.</para>
    /// </summary>
    public sealed class MovieTexture : Texture
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Pause(MovieTexture self);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Play(MovieTexture self);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Stop(MovieTexture self);
        /// <summary>
        /// <para>Pauses playing the movie.</para>
        /// </summary>
        public void Pause()
        {
            INTERNAL_CALL_Pause(this);
        }

        /// <summary>
        /// <para>Starts playing the movie.</para>
        /// </summary>
        public void Play()
        {
            INTERNAL_CALL_Play(this);
        }

        /// <summary>
        /// <para>Stops playing the movie, and rewinds it to the beginning.</para>
        /// </summary>
        public void Stop()
        {
            INTERNAL_CALL_Stop(this);
        }

        /// <summary>
        /// <para>Returns the AudioClip belonging to the MovieTexture.</para>
        /// </summary>
        public AudioClip audioClip { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The time, in seconds, that the movie takes to play back completely.</para>
        /// </summary>
        public float duration { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Returns whether the movie is playing or not.</para>
        /// </summary>
        public bool isPlaying { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>If the movie is downloading from a web site, this returns if enough data has been downloaded so playback should be able to start without interruptions.</para>
        /// </summary>
        public bool isReadyToPlay { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Set this to true to make the movie loop.</para>
        /// </summary>
        public bool loop { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

