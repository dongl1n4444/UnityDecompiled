namespace UnityEngineInternal
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    [Extension]
    internal static class NetFxCoreExtensions
    {
        [Extension]
        public static Delegate CreateDelegate(MethodInfo self, Type delegateType, object target)
        {
            return Delegate.CreateDelegate(delegateType, target, self);
        }

        [Extension]
        public static MethodInfo GetMethodInfo(Delegate self)
        {
            return self.Method;
        }
    }
}

