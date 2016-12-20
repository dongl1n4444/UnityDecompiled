namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    [Extension]
    public static class DelegateExtensions
    {
        [Extension]
        public static MethodInfo GetMethodInfoPortable(Delegate del)
        {
            return del.Method;
        }
    }
}

