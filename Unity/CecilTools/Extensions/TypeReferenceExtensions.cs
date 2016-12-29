namespace Unity.CecilTools.Extensions
{
    using Mono.Cecil;
    using System;
    using System.Runtime.CompilerServices;
    using Unity.CecilTools;

    public static class TypeReferenceExtensions
    {
        public static bool IsAssignableTo(this TypeReference typeRef, string typeName)
        {
            try
            {
                if (typeRef.IsGenericInstance)
                {
                    return ElementType.For(typeRef).IsAssignableTo(typeName);
                }
                return ((typeRef.FullName == typeName) || typeRef.CheckedResolve().IsSubclassOf(typeName));
            }
            catch (AssemblyResolutionException)
            {
                return false;
            }
        }

        public static bool IsEnum(this TypeReference type) => 
            ((type.IsValueType && !type.IsPrimitive) && type.CheckedResolve().IsEnum);

        public static bool IsStruct(this TypeReference type) => 
            (((type.IsValueType && !type.IsPrimitive) && !type.IsEnum()) && !IsSystemDecimal(type));

        private static bool IsSystemDecimal(TypeReference type) => 
            (type.FullName == "System.Decimal");

        public static string SafeNamespace(this TypeReference type)
        {
            if (type.IsGenericInstance)
            {
                return ((GenericInstanceType) type).ElementType.SafeNamespace();
            }
            if (type.IsNested)
            {
                return type.DeclaringType.SafeNamespace();
            }
            return type.Namespace;
        }
    }
}

