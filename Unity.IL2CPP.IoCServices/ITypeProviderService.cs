using Mono.Cecil;
using System;

namespace Unity.IL2CPP.IoCServices
{
	public interface ITypeProviderService
	{
		AssemblyDefinition Corlib
		{
			get;
		}

		TypeDefinition SystemObject
		{
			get;
		}

		TypeDefinition SystemString
		{
			get;
		}

		TypeDefinition SystemArray
		{
			get;
		}

		TypeDefinition SystemException
		{
			get;
		}

		TypeDefinition SystemDelegate
		{
			get;
		}

		TypeDefinition SystemMulticastDelegate
		{
			get;
		}

		TypeDefinition SystemByte
		{
			get;
		}

		TypeDefinition SystemUInt16
		{
			get;
		}

		TypeDefinition SystemIntPtr
		{
			get;
		}

		TypeDefinition SystemUIntPtr
		{
			get;
		}

		TypeDefinition SystemNullable
		{
			get;
		}

		TypeReference NativeIntTypeReference
		{
			get;
		}

		TypeReference NativeUIntTypeReference
		{
			get;
		}

		TypeReference Int32TypeReference
		{
			get;
		}

		TypeReference Int16TypeReference
		{
			get;
		}

		TypeReference UInt16TypeReference
		{
			get;
		}

		TypeReference SByteTypeReference
		{
			get;
		}

		TypeReference ByteTypeReference
		{
			get;
		}

		TypeReference BoolTypeReference
		{
			get;
		}

		TypeReference CharTypeReference
		{
			get;
		}

		TypeReference IntPtrTypeReference
		{
			get;
		}

		TypeReference UIntPtrTypeReference
		{
			get;
		}

		TypeReference Int64TypeReference
		{
			get;
		}

		TypeReference UInt32TypeReference
		{
			get;
		}

		TypeReference UInt64TypeReference
		{
			get;
		}

		TypeReference SingleTypeReference
		{
			get;
		}

		TypeReference DoubleTypeReference
		{
			get;
		}

		TypeReference ObjectTypeReference
		{
			get;
		}

		TypeReference StringTypeReference
		{
			get;
		}

		TypeReference RuntimeTypeHandleTypeReference
		{
			get;
		}

		TypeReference RuntimeMethodHandleTypeReference
		{
			get;
		}

		TypeReference RuntimeFieldHandleTypeReference
		{
			get;
		}

		TypeReference IActivationFactoryTypeReference
		{
			get;
		}

		TypeReference Il2CppComObjectTypeReference
		{
			get;
		}

		TypeReference Il2CppComDelegateTypeReference
		{
			get;
		}
	}
}
