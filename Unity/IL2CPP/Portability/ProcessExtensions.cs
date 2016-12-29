namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public static class ProcessExtensions
    {
        public static IntPtr GetHandlePortable(this Process process) => 
            process.Handle;
    }
}

