using Mono.Cecil;
using System;

namespace Unity.IL2CPP
{
	public class NullMethodCollector : IMethodCollector
	{
		public void AddMethod(MethodReference method)
		{
		}

		public void AddReversePInvokeWrapper(MethodReference method)
		{
		}

		public void AddWrapperForDelegateFromManagedToNative(MethodReference method)
		{
		}

		public void AddTypeMarshallingFunctions(TypeDefinition type)
		{
		}

		public void AddCCWMarshallingFunction(TypeDefinition type)
		{
		}
	}
}
