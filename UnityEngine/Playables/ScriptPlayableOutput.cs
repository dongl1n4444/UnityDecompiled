namespace UnityEngine.Playables
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Script output for the Graph. ScriptPlayable can be used to write custom Playable that implement their own PrepareFrame callback.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct ScriptPlayableOutput
    {
        internal PlayableOutput m_Output;
        /// <summary>
        /// <para>Used to compare ScriptPlayableOutput.</para>
        /// </summary>
        public static ScriptPlayableOutput Null =>
            new ScriptPlayableOutput { m_Output=PlayableOutput.Null };
        internal UnityEngine.Object referenceObject
        {
            get => 
                PlayableOutput.GetInternalReferenceObject(ref this.m_Output);
            set
            {
                PlayableOutput.SetInternalReferenceObject(ref this.m_Output, value);
            }
        }
        /// <summary>
        /// <para>Used to pass custom data to ScriptPlayable.ProcessFrame.</para>
        /// </summary>
        public UnityEngine.Object userData
        {
            get => 
                PlayableOutput.GetInternalUserData(ref this.m_Output);
            set
            {
                PlayableOutput.SetInternalUserData(ref this.m_Output, value);
            }
        }
        /// <summary>
        /// <para>Returns true if the PlayableOutput is properly constructed by the PlayableGraph and has not been destroyed.</para>
        /// </summary>
        public bool IsValid() => 
            PlayableOutput.IsValidInternal(ref this.m_Output);

        /// <summary>
        /// <para>The Playable that is bound to the output.</para>
        /// </summary>
        public PlayableHandle sourcePlayable
        {
            get => 
                PlayableOutput.InternalGetSourcePlayable(ref this.m_Output);
            set
            {
                PlayableOutput.InternalSetSourcePlayable(ref this.m_Output, ref value);
            }
        }
        /// <summary>
        /// <para>Specifies which input port of the SourcePlayable will be used to evaluate the graph.</para>
        /// </summary>
        public int sourceInputPort
        {
            get => 
                PlayableOutput.InternalGetSourceInputPort(ref this.m_Output);
            set
            {
                PlayableOutput.InternalSetSourceInputPort(ref this.m_Output, value);
            }
        }
    }
}

