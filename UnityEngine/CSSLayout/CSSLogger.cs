namespace UnityEngine.CSSLayout
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal static class CSSLogger
    {
        public static Func Logger = null;

        public static void Initialize()
        {
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Func(CSSLogLevel level, string message);
    }
}

