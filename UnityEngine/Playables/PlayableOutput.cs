namespace UnityEngine.Playables
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    internal struct PlayableOutput
    {
        internal IntPtr m_Handle;
        internal int m_Version;
        private const int m_NullVersion = 0x7fffffff;
        private static readonly PlayableOutput m_NullPlayableOutput;
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool IsValidInternal(ref PlayableOutput output);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern UnityEngine.Object GetInternalReferenceObject(ref PlayableOutput output);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void SetInternalReferenceObject(ref PlayableOutput output, UnityEngine.Object target);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern UnityEngine.Object GetInternalUserData(ref PlayableOutput output);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void SetInternalUserData(ref PlayableOutput output, [Writable] UnityEngine.Object target);
        internal static PlayableHandle InternalGetSourcePlayable(ref PlayableOutput output)
        {
            PlayableHandle handle;
            INTERNAL_CALL_InternalGetSourcePlayable(ref output, out handle);
            return handle;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_InternalGetSourcePlayable(ref PlayableOutput output, out PlayableHandle value);
        internal static void InternalSetSourcePlayable(ref PlayableOutput output, ref PlayableHandle target)
        {
            INTERNAL_CALL_InternalSetSourcePlayable(ref output, ref target);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_InternalSetSourcePlayable(ref PlayableOutput output, ref PlayableHandle target);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern int InternalGetSourceInputPort(ref PlayableOutput output);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void InternalSetSourceInputPort(ref PlayableOutput output, int port);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void InternalSetWeight(ref PlayableOutput output, float weight);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern float InternalGetWeight(ref PlayableOutput output);
        internal static PlayableOutput Null =>
            m_NullPlayableOutput;
        static PlayableOutput()
        {
            PlayableOutput output = new PlayableOutput {
                m_Version = 0x7fffffff
            };
            m_NullPlayableOutput = output;
        }
    }
}

