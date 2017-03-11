namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;

    public interface IInteropDataCollector
    {
        void AddCCWMarshallingFunction(TypeReference type);
        void AddGuid(TypeReference type);
        void AddReversePInvokeWrapper(MethodReference method);
        void AddTypeMarshallingFunctions(TypeDefinition type);
        void AddWindowsRuntimeTypeWithName(TypeReference type, string typeName);
        void AddWrapperForDelegateFromManagedToNative(MethodReference method);
    }
}

