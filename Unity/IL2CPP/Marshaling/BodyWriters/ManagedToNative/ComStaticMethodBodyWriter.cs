namespace Unity.IL2CPP.Marshaling.BodyWriters.ManagedToNative
{
    using Mono.Cecil;
    using System;
    using Unity.IL2CPP;

    internal class ComStaticMethodBodyWriter : ComMethodBodyWriter
    {
        public ComStaticMethodBodyWriter(MethodReference actualMethod) : base(actualMethod, GetInterfaceMethod(actualMethod))
        {
        }

        private static MethodReference GetInterfaceMethod(MethodReference method)
        {
            TypeDefinition type = method.DeclaringType.Resolve();
            if (!type.IsWindowsRuntime)
            {
                throw new InvalidOperationException("Calling static methods is not supported on COM classes!");
            }
            if (type.HasGenericParameters)
            {
                throw new InvalidOperationException("Calling static methods is not supported on types with generic parameters!");
            }
            if (type.IsInterface)
            {
                throw new InvalidOperationException("Calling static methods is not supported on interfaces!");
            }
            MethodReference overridenInterfaceMethod = method.GetOverridenInterfaceMethod(type.GetStaticFactoryTypes());
            if (overridenInterfaceMethod == null)
            {
                throw new InvalidOperationException($"Could not find overriden method for {method.FullName}");
            }
            return overridenInterfaceMethod;
        }
    }
}

