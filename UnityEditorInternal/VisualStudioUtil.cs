namespace UnityEditorInternal
{
    using System;
    using System.Runtime.CompilerServices;

    internal static class VisualStudioUtil
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool CanVS2017BuildCppCode();
    }
}

