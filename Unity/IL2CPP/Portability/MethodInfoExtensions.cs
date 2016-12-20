namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    [Extension]
    public static class MethodInfoExtensions
    {
        [Extension]
        public static RuntimeMethodHandle GetMethodHandlePortable(MethodBase methodInfo)
        {
            return methodInfo.MethodHandle;
        }

        [Extension]
        public static MethodImplAttributes GetMethodImplementationFlagsPortable(MethodBase methodInfo)
        {
            return methodInfo.GetMethodImplementationFlags();
        }
    }
}

