namespace UnityEngine.Experimental.Director
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Extends PlayableGraph for Audio.</para>
    /// </summary>
    public static class AudioPlayableGraphExtensions
    {
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern int internalAudioOutputCount(ref PlayableGraph graph);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool InternalGetAudioOutput(ref PlayableGraph graph, int index, out PlayableOutput output);
    }
}

