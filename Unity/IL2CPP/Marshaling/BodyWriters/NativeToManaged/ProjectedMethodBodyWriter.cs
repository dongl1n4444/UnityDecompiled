namespace Unity.IL2CPP.Marshaling.BodyWriters.NativeToManaged
{
    using Mono.Cecil;
    using System;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP.Marshaling.BodyWriters;

    internal class ProjectedMethodBodyWriter : ComCallableWrapperMethodBodyWriter
    {
        public ProjectedMethodBodyWriter(MethodReference managedInterfaceMethod, MethodReference nativeInterfaceMethod) : base(managedInterfaceMethod, nativeInterfaceMethod, MarshalType.WindowsRuntime)
        {
        }

        protected override string ManagedObjectExpression =>
            InteropMethodInfo.Naming.ThisParameterName;
    }
}

