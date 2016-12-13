using Mono.Cecil;
using System;
using System.Collections.ObjectModel;

namespace Unity.IL2CPP
{
	public interface IMethodCollectorResults
	{
		ReadOnlyCollection<MethodReference> GetMethods();

		int GetMethodIndex(MethodReference method);

		ReadOnlyCollection<MethodReference> GetReversePInvokeWrappers();

		int GetReversePInvokeWrapperIndex(MethodReference method);

		ReadOnlyCollection<MethodReference> GetWrappersForDelegateFromManagedToNative();

		int GetWrapperForDelegateFromManagedToNativedIndex(MethodReference method);

		ReadOnlyCollection<TypeDefinition> GetTypeMarshalingFunctions();

		int GetTypeMarshalingFunctionsIndex(TypeDefinition type);

		ReadOnlyCollection<TypeDefinition> GetCCWMarshalingFunctions();

		int GetCCWMarshalingFunctionIndex(TypeDefinition type);
	}
}
