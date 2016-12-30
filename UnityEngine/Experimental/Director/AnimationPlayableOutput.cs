namespace UnityEngine.Experimental.Director
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Animation output for the PlayableGraph.  Defines how a Playable is connected to an Animator.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct AnimationPlayableOutput
    {
        internal PlayableOutput m_Output;
        /// <summary>
        /// <para>Used to compare against AnimationPlayableOutput instances to check their validity.</para>
        /// </summary>
        public static AnimationPlayableOutput Null
        {
            get
            {
                AnimationPlayableOutput output = new AnimationPlayableOutput();
                PlayableOutput output2 = new PlayableOutput {
                    m_Version = 0x45
                };
                output.m_Output = output2;
                return output;
            }
        }
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
        /// <para>Returns true if the PlayableOutput has been properly constructed by the PlayableGraph and has not been destroyed.</para>
        /// </summary>
        public bool IsValid() => 
            PlayableOutput.IsValidInternal(ref this.m_Output);

        /// <summary>
        /// <para>The Animator component that is bound to this output.</para>
        /// </summary>
        public Animator target
        {
            get => 
                InternalGetTarget(ref this.m_Output);
            set
            {
                InternalSetTarget(ref this.m_Output, value);
            }
        }
        /// <summary>
        /// <para>The blend weight of the sourcePlayable to the animator.</para>
        /// </summary>
        public float weight
        {
            get => 
                PlayableOutput.InternalGetWeight(ref this.m_Output);
            set
            {
                PlayableOutput.InternalSetWeight(ref this.m_Output, value);
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern Animator InternalGetTarget(ref PlayableOutput output);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void InternalSetTarget(ref PlayableOutput output, Animator target);
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
    }
}

