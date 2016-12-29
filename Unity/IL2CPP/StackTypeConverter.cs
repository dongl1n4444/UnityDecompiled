namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    public class StackTypeConverter
    {
        [Inject]
        public static INamingService Naming;
        [Inject]
        public static ITypeProviderService TypeProvider;

        public static string CppStackTypeFor(TypeReference type)
        {
            TypeReference a = StackTypeFor(type);
            if (a.IsSameType(TypeProvider.NativeIntTypeReference) || a.IsByReference)
            {
                return Naming.ForIntPtrT;
            }
            switch (a.MetadataType)
            {
                case MetadataType.Int32:
                    return "int32_t";

                case MetadataType.Int64:
                    return "int64_t";

                case MetadataType.Single:
                    return "float";

                case MetadataType.Double:
                    return "double";
            }
            throw new ArgumentException("Unexpected StackTypeFor: " + a);
        }

        public static TypeReference StackTypeFor(TypeReference type)
        {
            PinnedType type2 = type as PinnedType;
            if (type2 != null)
            {
                type = type2.ElementType;
            }
            ByReferenceType type3 = type as ByReferenceType;
            if (type3 != null)
            {
                return type3;
            }
            RequiredModifierType type4 = type as RequiredModifierType;
            if (type4 != null)
            {
                return StackTypeFor(type4.ElementType);
            }
            if ((type.IsSameType(TypeProvider.NativeIntTypeReference) || type.IsSameType(TypeProvider.NativeUIntTypeReference)) || type.IsPointer)
            {
                return TypeProvider.NativeIntTypeReference;
            }
            if (!type.IsValueType())
            {
                return TypeProvider.ObjectTypeReference;
            }
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
                    return TypeProvider.Int32TypeReference;

                case MetadataType.Int64:
                case MetadataType.UInt64:
                    return TypeProvider.Int64TypeReference;

                case MetadataType.Single:
                    return TypeProvider.SingleTypeReference;

                case MetadataType.Double:
                    return TypeProvider.DoubleTypeReference;

                case MetadataType.IntPtr:
                case MetadataType.UIntPtr:
                    return TypeProvider.NativeIntTypeReference;
            }
            throw new ArgumentException($"Cannot get stack type for {type.Name}");
        }

        public static TypeReference StackTypeForBinaryOperation(TypeReference type)
        {
            TypeReference reference = StackTypeFor(type);
            if (reference is ByReferenceType)
            {
                return TypeProvider.NativeIntTypeReference;
            }
            return reference;
        }
    }
}

