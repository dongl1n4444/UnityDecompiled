namespace UnityEngine.Experimental.Director
{
    using System;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Playables are customizable runtime objects that can be connected together in a tree to create complex behaviours.</para>
    /// </summary>
    [RequiredByNativeCode]
    public class Playable
    {
        /// <summary>
        /// <para>Returns the PlayableHandle for this playable.</para>
        /// </summary>
        public PlayableHandle handle;

        /// <summary>
        /// <para>Returns true if the Playable is valid. A playable can be invalid if it was disposed. This is different from a Null playable.</para>
        /// </summary>
        public bool IsValid() => 
            this.handle.IsValid();

        public static implicit operator PlayableHandle(Playable b) => 
            b.handle;
    }
}

