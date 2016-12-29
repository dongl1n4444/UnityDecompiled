namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public static class MethodInfoExtensions
    {
        public static RuntimeMethodHandle GetMethodHandlePortable(this MethodBase methodInfo) => 
            methodInfo.MethodHandle;

        public static MethodImplAttributes GetMethodImplementationFlagsPortable(this MethodBase methodInfo) => 
            methodInfo.GetMethodImplementationFlags();
    }
}

