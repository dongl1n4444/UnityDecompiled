namespace Unity.IL2CPP.StackAnalysis
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.IoCServices;

    public static class StackAnalysisUtils
    {
        private static readonly List<MetadataType> _orderedTypes;

        static StackAnalysisUtils()
        {
            List<MetadataType> list = new List<MetadataType> {
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
            _orderedTypes = list;
        }

        public static TypeReference CorrectLargestTypeFor(TypeReference leftType, TypeReference rightType, ITypeProviderService typeProvider)
        {
            TypeReference a = StackTypeConverter.StackTypeFor(leftType);
            TypeReference reference2 = StackTypeConverter.StackTypeFor(rightType);
            if (!leftType.IsByReference)
            {
                if (rightType.IsByReference)
                {
                    return rightType;
                }
                if (leftType.IsPointer)
                {
                    return leftType;
                }
                if (rightType.IsPointer)
                {
                    return rightType;
                }
                if ((a.MetadataType == MetadataType.Int64) || (reference2.MetadataType == MetadataType.Int64))
                {
                    return typeProvider.Int64TypeReference;
                }
                if (a.IsSameType(typeProvider.NativeIntTypeReference) || reference2.IsSameType(typeProvider.NativeIntTypeReference))
                {
                    return typeProvider.NativeIntTypeReference;
                }
                if (a.IsSameType(typeProvider.Int32TypeReference) && reference2.IsSameType(typeProvider.Int32TypeReference))
                {
                    return typeProvider.Int32TypeReference;
                }
            }
            return leftType;
        }

        public static TypeReference GetWidestValueType(IEnumerable<TypeReference> types)
        {
            MetadataType item = _orderedTypes[0];
            TypeReference reference = null;
            foreach (TypeReference reference2 in types)
            {
                if ((reference2.IsValueType() && !reference2.Resolve().IsEnum) && (_orderedTypes.IndexOf(reference2.MetadataType) > _orderedTypes.IndexOf(item)))
                {
                    item = reference2.MetadataType;
                    reference = reference2;
                }
            }
            return reference;
        }

        public static TypeReference ResultTypeForAdd(TypeReference leftType, TypeReference rightType, ITypeProviderService typeProvider) => 
            CorrectLargestTypeFor(leftType, rightType, typeProvider);

        public static TypeReference ResultTypeForMul(TypeReference leftType, TypeReference rightType, ITypeProviderService typeProvider) => 
            CorrectLargestTypeFor(leftType, rightType, typeProvider);

        public static TypeReference ResultTypeForSub(TypeReference leftType, TypeReference rightType, ITypeProviderService typeProvider)
        {
            if ((leftType.MetadataType == MetadataType.Byte) || (leftType.MetadataType == MetadataType.UInt16))
            {
                return typeProvider.Int32TypeReference;
            }
            if (leftType.MetadataType == MetadataType.Char)
            {
                return typeProvider.Int32TypeReference;
            }
            return CorrectLargestTypeFor(leftType, rightType, typeProvider);
        }

        public delegate TypeReference ResultTypeAnalysisMethod(TypeReference leftType, TypeReference rightType, ITypeProviderService typeProvider);
    }
}

