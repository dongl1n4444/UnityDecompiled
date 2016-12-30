namespace Unity.IL2CPP.IoCServices
{
    using Mono.Cecil;

    public interface ITypeProviderService
    {
        TypeReference BindableIteratorToEnumeratorAdapterTypeReference { get; }

        TypeReference BoolTypeReference { get; }

        TypeReference ByteTypeReference { get; }

        TypeReference CharTypeReference { get; }

        AssemblyDefinition Corlib { get; }

        TypeReference DoubleTypeReference { get; }

        TypeReference IActivationFactoryTypeReference { get; }

        TypeReference IBindableIterableTypeReference { get; }

        TypeReference IBindableIteratorTypeReference { get; }

        TypeReference IIterableTypeReference { get; }

        TypeReference Il2CppComDelegateTypeReference { get; }

        TypeReference Il2CppComObjectTypeReference { get; }

        TypeReference Int16TypeReference { get; }

        TypeReference Int32TypeReference { get; }

        TypeReference Int64TypeReference { get; }

        TypeReference IntPtrTypeReference { get; }

        TypeReference IPropertyValueType { get; }

        TypeReference IReferenceType { get; }

        TypeDefinition IStringableType { get; }

        TypeReference NativeIntTypeReference { get; }

        TypeReference NativeUIntTypeReference { get; }

        TypeReference ObjectTypeReference { get; }

        TypeReference RuntimeFieldHandleTypeReference { get; }

        TypeReference RuntimeMethodHandleTypeReference { get; }

        TypeReference RuntimeTypeHandleTypeReference { get; }

        TypeReference SByteTypeReference { get; }

        TypeReference SingleTypeReference { get; }

        TypeReference StringTypeReference { get; }

        TypeDefinition SystemArray { get; }

        TypeDefinition SystemByte { get; }

        TypeDefinition SystemDelegate { get; }

        TypeDefinition SystemException { get; }

        TypeDefinition SystemIntPtr { get; }

        TypeDefinition SystemMulticastDelegate { get; }

        TypeDefinition SystemNullable { get; }

        TypeDefinition SystemObject { get; }

        TypeDefinition SystemString { get; }

        TypeDefinition SystemUInt16 { get; }

        TypeDefinition SystemUIntPtr { get; }

        TypeDefinition SystemVoid { get; }

        TypeReference UInt16TypeReference { get; }

        TypeReference UInt32TypeReference { get; }

        TypeReference UInt64TypeReference { get; }

        TypeReference UIntPtrTypeReference { get; }
    }
}

