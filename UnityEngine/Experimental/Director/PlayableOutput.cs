namespace UnityEngine.Experimental.Director
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
    }
}

