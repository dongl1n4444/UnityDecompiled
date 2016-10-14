using Mono.Cecil;
using System;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP
{
	public class StackTypeConverter
	{
		[Inject]
		public static INamingService Naming;

		[Inject]
		public static ITypeProviderService TypeProvider;

		public static TypeReference StackTypeFor(TypeReference type)
		{
			ByReferenceType byReferenceType = type as ByReferenceType;
			TypeReference result;
			if (byReferenceType != null)
			{
				result = byReferenceType;
			}
			else
			{
				PinnedType pinnedType = type as PinnedType;
				if (pinnedType != null)
				{
					type = pinnedType.ElementType;
				}
				RequiredModifierType requiredModifierType = type as RequiredModifierType;
				if (requiredModifierType != null)
				{
					result = StackTypeConverter.StackTypeFor(requiredModifierType.ElementType);
				}
				else if (type.IsSameType(StackTypeConverter.TypeProvider.NativeIntTypeReference) || type.IsSameType(StackTypeConverter.TypeProvider.NativeUIntTypeReference) || type.IsPointer)
				{
					result = StackTypeConverter.TypeProvider.NativeIntTypeReference;
				}
				else
				{
					if (type.IsValueType())
					{
						MetadataType metadataType = type.MetadataType;
						if (type.IsValueType() && type.IsEnum())
						{
							metadataType = type.GetUnderlyingEnumType().MetadataType;
						}
						switch (metadataType)
						{
						case MetadataType.Boolean:
						case MetadataType.Char:
						case MetadataType.SByte:
						case MetadataType.Byte:
						case MetadataType.Int16:
						case MetadataType.UInt16:
						case MetadataType.Int32:
						case MetadataType.UInt32:
							result = StackTypeConverter.TypeProvider.Int32TypeReference;
							return result;
						case MetadataType.Int64:
						case MetadataType.UInt64:
							result = StackTypeConverter.TypeProvider.Int64TypeReference;
							return result;
						case MetadataType.Single:
							result = StackTypeConverter.TypeProvider.SingleTypeReference;
							return result;
						case MetadataType.Double:
							result = StackTypeConverter.TypeProvider.DoubleTypeReference;
							return result;
						case MetadataType.IntPtr:
						case MetadataType.UIntPtr:
							result = StackTypeConverter.TypeProvider.NativeIntTypeReference;
							return result;
						}
						throw new ArgumentException(string.Format("Cannot get stack type for {0}", type.Name));
					}
					result = StackTypeConverter.TypeProvider.ObjectTypeReference;
				}
			}
			return result;
		}

		public static string CppStackTypeFor(TypeReference type)
		{
			TypeReference typeReference = StackTypeConverter.StackTypeFor(type);
			string result;
			if (!typeReference.IsSameType(StackTypeConverter.TypeProvider.NativeIntTypeReference) && !typeReference.IsByReference)
			{
				switch (typeReference.MetadataType)
				{
				case MetadataType.Int32:
					result = "int32_t";
					return result;
				case MetadataType.Int64:
					result = "int64_t";
					return result;
				case MetadataType.Single:
					result = "float";
					return result;
				case MetadataType.Double:
					result = "double";
					return result;
				}
				throw new ArgumentException("Unexpected StackTypeFor: " + typeReference);
			}
			result = StackTypeConverter.Naming.ForIntPtrT;
			return result;
		}

		public static TypeReference StackTypeForBinaryOperation(TypeReference type)
		{
			TypeReference typeReference = StackTypeConverter.StackTypeFor(type);
			TypeReference result;
			if (typeReference is ByReferenceType)
			{
				result = StackTypeConverter.TypeProvider.NativeIntTypeReference;
			}
			else
			{
				result = typeReference;
			}
			return result;
		}
	}
}
