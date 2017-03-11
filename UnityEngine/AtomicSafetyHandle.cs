namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    internal struct AtomicSafetyHandle
    {
        internal IntPtr versionNode;
        internal AtomicSafetyHandleVersionMask version;
        internal static AtomicSafetyHandle Create()
        {
            AtomicSafetyHandle handle;
            Create_Injected(out handle);
            return handle;
        }

        internal static void Release(AtomicSafetyHandle handle)
        {
            Release_Injected(ref handle);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void PrepareUndisposable(ref AtomicSafetyHandle handle);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void UseSecondaryVersion(ref AtomicSafetyHandle handle);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void BumpSecondaryVersion(ref AtomicSafetyHandle handle);
        internal static void EnforceAllBufferJobsHaveCompletedAndRelease(AtomicSafetyHandle handle)
        {
            EnforceAllBufferJobsHaveCompletedAndRelease_Injected(ref handle);
        }

        internal static void CheckReadAndThrowNoEarlyOut(AtomicSafetyHandle handle)
        {
            CheckReadAndThrowNoEarlyOut_Injected(ref handle);
        }

        internal static void CheckWriteAndThrowNoEarlyOut(AtomicSafetyHandle handle)
        {
            CheckWriteAndThrowNoEarlyOut_Injected(ref handle);
        }

        internal static void CheckDeallocateAndThrow(AtomicSafetyHandle handle)
        {
            CheckDeallocateAndThrow_Injected(ref handle);
        }

        internal static unsafe void CheckReadAndThrow(AtomicSafetyHandle handle)
        {
            AtomicSafetyHandleVersionMask* versionNode = (AtomicSafetyHandleVersionMask*) handle.versionNode;
            if (((handle.version & AtomicSafetyHandleVersionMask.Read) == ~(AtomicSafetyHandleVersionMask.ReadAndWrite | AtomicSafetyHandleVersionMask.ReadAndWriteInv)) && (handle.version != (*(((int*) versionNode)) & -2)))
            {
                CheckReadAndThrowNoEarlyOut(handle);
            }
        }

        internal static unsafe void CheckWriteAndThrow(AtomicSafetyHandle handle)
        {
            AtomicSafetyHandleVersionMask* versionNode = (AtomicSafetyHandleVersionMask*) handle.versionNode;
            if (((handle.version & AtomicSafetyHandleVersionMask.Write) == ~(AtomicSafetyHandleVersionMask.ReadAndWrite | AtomicSafetyHandleVersionMask.ReadAndWriteInv)) && (handle.version != (*(((int*) versionNode)) & -3)))
            {
                CheckWriteAndThrowNoEarlyOut(handle);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Create_Injected(out AtomicSafetyHandle ret);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Release_Injected(ref AtomicSafetyHandle handle);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void EnforceAllBufferJobsHaveCompletedAndRelease_Injected(ref AtomicSafetyHandle handle);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void CheckReadAndThrowNoEarlyOut_Injected(ref AtomicSafetyHandle handle);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void CheckWriteAndThrowNoEarlyOut_Injected(ref AtomicSafetyHandle handle);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void CheckDeallocateAndThrow_Injected(ref AtomicSafetyHandle handle);
    }
}

