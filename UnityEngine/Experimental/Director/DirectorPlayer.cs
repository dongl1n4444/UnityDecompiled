namespace UnityEngine.Experimental.Director
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    /// <summary>
    /// <para>The DirectorPlayer is the base class for all components capable of playing a Experimental.Director.Playable tree.</para>
    /// </summary>
    public class DirectorPlayer : Behaviour
    {
        /// <summary>
        /// <para>Returns the Player's current local time.</para>
        /// </summary>
        /// <returns>
        /// <para>Current local time.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern double GetTime();
        /// <summary>
        /// <para>Returns the current Experimental.Director.DirectorUpdateMode.</para>
        /// </summary>
        /// <returns>
        /// <para>Current update mode for this player.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern DirectorUpdateMode GetTimeUpdateMode();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_PlayStructInternal(DirectorPlayer self, ref Playable pStruct);
        /// <summary>
        /// <para>Starts playing a Experimental.Director.Playable tree.</para>
        /// </summary>
        /// <param name="playable">The root Experimental.Director.Playable in the tree.</param>
        /// <param name="pStruct"></param>
        public void Play(Playable pStruct)
        {
            this.PlayStructInternal(pStruct);
        }

        private void PlayStructInternal(Playable pStruct)
        {
            INTERNAL_CALL_PlayStructInternal(this, ref pStruct);
        }

        /// <summary>
        /// <para>Sets the Player's local time.</para>
        /// </summary>
        /// <param name="time">The new local time.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetTime(double time);
        /// <summary>
        /// <para>Specifies the way the Player's will increment when it is playing.</para>
        /// </summary>
        /// <param name="mode"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetTimeUpdateMode(DirectorUpdateMode mode);
        /// <summary>
        /// <para>Stop the playback of the Player and Experimental.Director.Playable.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void Stop();
    }
}

