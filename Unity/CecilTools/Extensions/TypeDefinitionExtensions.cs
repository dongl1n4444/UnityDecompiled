namespace Unity.CecilTools.Extensions
{
    using Mono.Cecil;
    using System;
    using System.Runtime.CompilerServices;

    [Extension]
    public static class TypeDefinitionExtensions
    {
        [Extension]
        public static bool IsSubclassOf(TypeDefinition type, string baseTypeName)
        {
            TypeReference baseType = type.BaseType;
            if (baseType == null)
            {
                return false;
            }
            return ((baseType.FullName == baseTypeName) || IsSubclassOf(ResolutionExtensions.CheckedResolve(baseType), baseTypeName));
        }
    }
}

