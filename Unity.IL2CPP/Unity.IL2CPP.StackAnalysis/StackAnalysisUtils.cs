using Mono.Cecil;
using System;
using System.Collections.Generic;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP.StackAnalysis
{
	public static class StackAnalysisUtils
	{
		public delegate TypeReference ResultTypeAnalysisMethod(TypeReference leftType, TypeReference rightType, ITypeProviderService typeProvider);

		private static readonly List<MetadataType> _orderedTypes = new List<MetadataType>
		{
			MetadataType.Void,
			MetadataType.Boolean,
			MetadataType.Char,
			MetadataType.SByte,
			MetadataType.Byte,
			MetadataType.Int16,
			MetadataType.UInt16,
			MetadataType.Int32,
			MetadataType.UInt32,
			MetadataType.Int64,
			MetadataType.UInt64,
			MetadataType.Single,
			MetadataType.Double,
			MetadataType.String
		};

		public static TypeReference GetWidestValueType(IEnumerable<TypeReference> types)
		{
			MetadataType item = StackAnalysisUtils._orderedTypes[0];
			TypeReference result = null;
			foreach (TypeReference current in types)
			{
				if (current.IsValueType() && !current.Resolve().IsEnum && StackAnalysisUtils._orderedTypes.IndexOf(current.MetadataType) > StackAnalysisUtils._orderedTypes.IndexOf(item))
				{
					item = current.MetadataType;
					result = current;
				}
			}
			return result;
		}

		public static TypeReference ResultTypeForAdd(TypeReference leftType, TypeReference rightType, ITypeProviderService typeProvider)
		{
			return StackAnalysisUtils.CorrectLargestTypeFor(leftType, rightType, typeProvider);
		}

		public static TypeReference ResultTypeForSub(TypeReference leftType, TypeReference rightType, ITypeProviderService typeProvider)
		{
			TypeReference result;
			if (leftType.MetadataType == MetadataType.Byte || leftType.MetadataType == MetadataType.UInt16)
			{
				result = typeProvider.Int32TypeReference;
			}
			else if (leftType.MetadataType == MetadataType.Char)
			{
				result = typeProvider.Int32TypeReference;
			}
			else
			{
				result = StackAnalysisUtils.CorrectLargestTypeFor(leftType, rightType, typeProvider);
			}
			return result;
		}

		public static TypeReference ResultTypeForMul(TypeReference leftType, TypeReference rightType, ITypeProviderService typeProvider)
		{
			return StackAnalysisUtils.CorrectLargestTypeFor(leftType, rightType, typeProvider);
		}

		public static TypeReference CorrectLargestTypeFor(TypeReference leftType, TypeReference rightType, ITypeProviderService typeProvider)
		{
			TypeReference typeReference = StackTypeConverter.StackTypeFor(leftType);
			TypeReference typeReference2 = StackTypeConverter.StackTypeFor(rightType);
			TypeReference result;
			if (leftType.IsByReference)
			{
				result = leftType;
			}
			else if (rightType.IsByReference)
			{
				result = rightType;
			}
			else if (leftType.IsPointer)
			{
				result = leftType;
			}
			else if (rightType.IsPointer)
			{
				result = rightType;
			}
			else if (typeReference.MetadataType == MetadataType.Int64 || typeReference2.MetadataType == MetadataType.Int64)
			{
				result = typeProvider.Int64TypeReference;
			}
			else if (typeReference.IsSameType(typeProvider.NativeIntTypeReference) || typeReference2.IsSameType(typeProvider.NativeIntTypeReference))
			{
				result = typeProvider.NativeIntTypeReference;
			}
			else if (typeReference.IsSameType(typeProvider.Int32TypeReference) && typeReference2.IsSameType(typeProvider.Int32TypeReference))
			{
				result = typeProvider.Int32TypeReference;
			}
			else
			{
				result = leftType;
			}
			return result;
		}
	}
}
