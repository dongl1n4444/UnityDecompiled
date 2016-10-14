using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.IL2CPP.Marshaling.BodyWriters.ManagedToNative
{
	internal class ComInstanceMethodBodyWriter : ComMethodBodyWriter
	{
		public ComInstanceMethodBodyWriter(MethodReference method) : base(method, ComInstanceMethodBodyWriter.GetInterfaceMethod(method))
		{
		}

		private static MethodReference GetInterfaceMethod(MethodReference method)
		{
			TypeReference declaringType = method.DeclaringType;
			MethodReference result;
			if (declaringType.IsInterface())
			{
				result = method;
			}
			else
			{
				TypeReference[] nonInstanceInterfaces = declaringType.GetActivationFactoryTypes().ToArray<TypeReference>();
				IEnumerable<TypeReference> candidateInterfaces = from iface in declaringType.GetInterfaces()
				where !nonInstanceInterfaces.Any((TypeReference nonInstanceInterface) => iface == nonInstanceInterface)
				select iface;
				MethodReference overridenInterfaceMethod = method.GetOverridenInterfaceMethod(candidateInterfaces);
				if (overridenInterfaceMethod == null)
				{
					throw new InvalidOperationException(string.Format("Could not find overriden method for {0}", method.FullName));
				}
				result = overridenInterfaceMethod;
			}
			return result;
		}
	}
}
