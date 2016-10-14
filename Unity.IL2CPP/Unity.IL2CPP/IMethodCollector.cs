using Mono.Cecil;
using System;

namespace Unity.IL2CPP
{
	public interface IMethodCollector
	{
		void AddMethod(MethodReference method);

		void AddReversePInvokeWrapper(MethodReference method);

		void AddWrapperForDelegateFromManagedToNative(MethodReference method);

		void AddTypeMarshallingFunctions(TypeDefinition type);

		void AddCCWMarshallingFunction(TypeDefinition type);
	}
}
