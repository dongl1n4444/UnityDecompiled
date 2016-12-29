namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public static class DelegateExtensions
    {
        public static MethodInfo GetMethodInfoPortable(this Delegate del) => 
            del.Method;
    }
}

