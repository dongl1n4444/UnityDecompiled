namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using System.Collections.ObjectModel;

    public interface IMethodCollectorResults
    {
        int GetCCWMarshalingFunctionIndex(TypeDefinition type);
        ReadOnlyCollection<TypeDefinition> GetCCWMarshalingFunctions();
        int GetMethodIndex(MethodReference method);
        ReadOnlyCollection<MethodReference> GetMethods();
        int GetReversePInvokeWrapperIndex(MethodReference method);
        ReadOnlyCollection<MethodReference> GetReversePInvokeWrappers();
        ReadOnlyCollection<TypeDefinition> GetTypeMarshalingFunctions();
        int GetTypeMarshalingFunctionsIndex(TypeDefinition type);
        int GetWrapperForDelegateFromManagedToNativedIndex(MethodReference method);
        ReadOnlyCollection<MethodReference> GetWrappersForDelegateFromManagedToNative();
    }
}

