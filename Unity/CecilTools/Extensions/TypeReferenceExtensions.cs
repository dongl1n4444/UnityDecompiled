namespace Unity.CecilTools.Extensions
{
    using Mono.Cecil;
    using System;
    using System.Runtime.CompilerServices;
    using Unity.CecilTools;

    [Extension]
    public static class TypeReferenceExtensions
    {
        [Extension]
        public static bool IsAssignableTo(TypeReference typeRef, string typeName)
        {
            try
            {
                if (typeRef.IsGenericInstance)
                {
                    return IsAssignableTo(ElementType.For(typeRef), typeName);
                }
                return ((typeRef.FullName == typeName) || TypeDefinitionExtensions.IsSubclassOf(ResolutionExtensions.CheckedResolve(typeRef), typeName));
            }
            catch (AssemblyResolutionException)
            {
                return false;
            }
        }

        [Extension]
        public static bool IsEnum(TypeReference type)
        {
            return ((type.IsValueType && !type.IsPrimitive) && ResolutionExtensions.CheckedResolve(type).IsEnum);
        }

        [Extension]
        public static bool IsStruct(TypeReference type)
        {
            return (((type.IsValueType && !type.IsPrimitive) && !IsEnum(type)) && !IsSystemDecimal(type));
        }

        private static bool IsSystemDecimal(TypeReference type)
        {
            return (type.FullName == "System.Decimal");
        }

        [Extension]
        public static string SafeNamespace(TypeReference type)
        {
            if (type.IsGenericInstance)
            {
                return SafeNamespace(((GenericInstanceType) type).ElementType);
            }
            if (type.IsNested)
            {
                return SafeNamespace(type.DeclaringType);
            }
            return type.Namespace;
        }
    }
}

