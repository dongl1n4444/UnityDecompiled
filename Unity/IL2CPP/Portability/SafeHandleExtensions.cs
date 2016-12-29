namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static class SafeHandleExtensions
    {
        public static void ClosePortable(this SafeHandle handle)
        {
            handle.Close();
        }
    }
}

