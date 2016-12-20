namespace UnityEngine.Profiling
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    [UsedByNativeCode]
    internal sealed class Sampler
    {
        internal IntPtr m_Ptr;

        internal Sampler()
        {
        }

        public bool enabled { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        public string name { [MethodImpl(MethodImplOptions.InternalCall)] get; }
    }
}

