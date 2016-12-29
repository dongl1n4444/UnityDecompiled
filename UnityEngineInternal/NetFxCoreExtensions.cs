namespace UnityEngineInternal
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    internal static class NetFxCoreExtensions
    {
        public static Delegate CreateDelegate(this MethodInfo self, Type delegateType, object target) => 
            Delegate.CreateDelegate(delegateType, target, self);

        public static MethodInfo GetMethodInfo(this Delegate self) => 
            self.Method;
    }
}

