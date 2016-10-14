using Mono.Cecil;
using System;

namespace Unity.IL2CPP.Marshaling.BodyWriters.ManagedToNative
{
	internal class ComStaticMethodBodyWriter : ComMethodBodyWriter
	{
		public ComStaticMethodBodyWriter(MethodReference actualMethod) : base(actualMethod, ComStaticMethodBodyWriter.GetInterfaceMethod(actualMethod))
		{
		}

		private static MethodReference GetInterfaceMethod(MethodReference method)
		{
			TypeDefinition typeDefinition = method.DeclaringType.Resolve();
			if (!typeDefinition.IsWindowsRuntime)
			{
				throw new InvalidOperationException("Calling static methods is not supported on COM classes!");
			}
			if (typeDefinition.HasGenericParameters)
			{
				throw new InvalidOperationException("Calling static methods is not supported on types with generic parameters!");
			}
			if (typeDefinition.IsInterface)
			{
				throw new InvalidOperationException("Calling static methods is not supported on interfaces!");
			}
			MethodReference overridenInterfaceMethod = method.GetOverridenInterfaceMethod(typeDefinition.GetStaticFactoryTypes());
			if (overridenInterfaceMethod == null)
			{
				throw new InvalidOperationException(string.Format("Could not find overriden method for {0}", method.FullName));
			}
			return overridenInterfaceMethod;
		}
	}
}
