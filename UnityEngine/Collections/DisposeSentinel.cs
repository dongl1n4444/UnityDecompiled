namespace UnityEngine.Collections
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    internal class DisposeSentinel
    {
        private IntPtr m_Ptr;
        private DeallocateDelegate m_DeallocateDelegate;
        private Allocator m_Label;
        private string m_FileName;
        private int m_LineNumber;
        public static DisposeSentinel Create(IntPtr ptr, Allocator label, int callSiteStackDepth, DeallocateDelegate deallocateDelegate = null)
        {
            if (NativeLeakDetection.Mode == NativeLeakDetectionMode.Enabled)
            {
                DisposeSentinel sentinel = new DisposeSentinel();
                StackFrame frame = new StackFrame(callSiteStackDepth + 2, true);
                sentinel.m_FileName = frame.GetFileName();
                sentinel.m_LineNumber = frame.GetFileLineNumber();
                sentinel.m_Ptr = ptr;
                sentinel.m_Label = label;
                sentinel.m_DeallocateDelegate = deallocateDelegate;
                return sentinel;
            }
            return null;
        }

        ~DisposeSentinel()
        {
            if (this.m_Ptr != IntPtr.Zero)
            {
                UnsafeUtility.LogError($"A NativeArray created with Allocator.Persistent has not been disposed, resulting in a memory leak. It was allocated at {this.m_FileName}:{this.m_LineNumber}.", this.m_FileName, this.m_LineNumber);
                if (this.m_DeallocateDelegate != null)
                {
                    this.m_DeallocateDelegate(this.m_Ptr, this.m_Label);
                }
                else
                {
                    UnsafeUtility.Free(this.m_Ptr, this.m_Label);
                }
            }
        }

        public static void Clear(ref DisposeSentinel sentinel)
        {
            if (sentinel != null)
            {
                sentinel.m_Ptr = IntPtr.Zero;
                sentinel = null;
            }
        }
        public delegate void DeallocateDelegate(IntPtr buffer, Allocator allocator);
    }
}

