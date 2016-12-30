namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential)]
    internal struct RenderBufferHelper
    {
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern int GetLoadAction(out RenderBuffer b);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void SetLoadAction(out RenderBuffer b, int a);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern int GetStoreAction(out RenderBuffer b);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void SetStoreAction(out RenderBuffer b, int a);
        internal static IntPtr GetNativeRenderBufferPtr(IntPtr rb)
        {
            IntPtr ptr;
            INTERNAL_CALL_GetNativeRenderBufferPtr(rb, out ptr);
            return ptr;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetNativeRenderBufferPtr(IntPtr rb, out IntPtr value);
    }
}

