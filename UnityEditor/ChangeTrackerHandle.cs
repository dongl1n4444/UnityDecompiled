namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    internal struct ChangeTrackerHandle
    {
        private IntPtr m_Handle;
        internal static ChangeTrackerHandle AcquireTracker(UnityEngine.Object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("Not a valid unity engine object");
            }
            return Internal_AcquireTracker(obj);
        }

        private static ChangeTrackerHandle Internal_AcquireTracker(UnityEngine.Object o)
        {
            ChangeTrackerHandle handle;
            INTERNAL_CALL_Internal_AcquireTracker(o, out handle);
            return handle;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Internal_AcquireTracker(UnityEngine.Object o, out ChangeTrackerHandle value);
        internal void ReleaseTracker()
        {
            if (this.m_Handle == IntPtr.Zero)
            {
                throw new ArgumentNullException("Not a valid handle, has it been released already?");
            }
            Internal_ReleaseTracker(this);
            this.m_Handle = IntPtr.Zero;
        }

        private static void Internal_ReleaseTracker(ChangeTrackerHandle h)
        {
            INTERNAL_CALL_Internal_ReleaseTracker(ref h);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Internal_ReleaseTracker(ref ChangeTrackerHandle h);
        internal bool PollForChanges()
        {
            if (this.m_Handle == IntPtr.Zero)
            {
                throw new ArgumentNullException("Not a valid handle, has it been released already?");
            }
            return this.Internal_PollChanges();
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern bool Internal_PollChanges();
    }
}

