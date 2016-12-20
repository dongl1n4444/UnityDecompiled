namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;

    public interface IMethodCollector
    {
        void AddCCWMarshallingFunction(TypeDefinition type);
        void AddMethod(MethodReference method);
        void AddReversePInvokeWrapper(MethodReference method);
        void AddTypeMarshallingFunctions(TypeDefinition type);
        void AddWrapperForDelegateFromManagedToNative(MethodReference method);
    }
}

