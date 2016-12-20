namespace Unity.CecilTools.Extensions
{
    using Mono.Cecil;
    using System;
    using System.Runtime.CompilerServices;

    [Extension]
    internal static class MethodDefinitionExtensions
    {
        [Extension]
        public static bool IsConversionOperator(MethodDefinition method)
        {
            if (!method.IsSpecialName)
            {
                return false;
            }
            return ((method.Name == "op_Implicit") || (method.Name == "op_Explicit"));
        }

        public static bool IsDefaultConstructor(MethodDefinition m)
        {
            return ((m.IsConstructor && !m.IsStatic) && (m.Parameters.Count == 0));
        }

        [Extension]
        public static bool IsSimpleGetter(MethodDefinition original)
        {
            return (original.IsGetter && (original.Parameters.Count == 0));
        }

        [Extension]
        public static bool IsSimplePropertyAccessor(MethodDefinition method)
        {
            return (IsSimpleGetter(method) || IsSimpleSetter(method));
        }

        [Extension]
        public static bool IsSimpleSetter(MethodDefinition original)
        {
            return (original.IsSetter && (original.Parameters.Count == 1));
        }

        [Extension]
        public static string PropertyName(MethodDefinition self)
        {
            return self.Name.Substring(4);
        }

        [Extension]
        public static bool SameAs(MethodDefinition self, MethodDefinition other)
        {
            return (self.FullName == other.FullName);
        }
    }
}

