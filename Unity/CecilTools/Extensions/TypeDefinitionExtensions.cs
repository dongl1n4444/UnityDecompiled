namespace Unity.CecilTools.Extensions
{
    using Mono.Cecil;
    using System;
    using System.Runtime.CompilerServices;

    public static class TypeDefinitionExtensions
    {
        public static bool IsSubclassOf(this TypeDefinition type, string baseTypeName)
        {
            TypeReference baseType = type.BaseType;
            return ((baseType?.FullName == baseTypeName) || baseType.CheckedResolve().IsSubclassOf(baseTypeName));
        }
    }
}

