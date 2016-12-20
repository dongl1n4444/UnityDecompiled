namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [Extension]
    public static class SafeHandleExtensions
    {
        [Extension]
        public static void ClosePortable(SafeHandle handle)
        {
            handle.Close();
        }
    }
}

