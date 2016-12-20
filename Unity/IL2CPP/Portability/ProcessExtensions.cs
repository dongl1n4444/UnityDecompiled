namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [Extension]
    public static class ProcessExtensions
    {
        [Extension]
        public static IntPtr GetHandlePortable(Process process)
        {
            return process.Handle;
        }
    }
}

