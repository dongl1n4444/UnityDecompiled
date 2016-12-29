namespace Unity.CecilTools.Extensions
{
    using Mono.Cecil;
    using System;
    using System.Runtime.CompilerServices;

    internal static class MethodDefinitionExtensions
    {
        public static bool IsConversionOperator(this MethodDefinition method)
        {
            if (!method.IsSpecialName)
            {
                return false;
            }
            return ((method.Name == "op_Implicit") || (method.Name == "op_Explicit"));
        }

        public static bool IsDefaultConstructor(MethodDefinition m) => 
            ((m.IsConstructor && !m.IsStatic) && (m.Parameters.Count == 0));

        public static bool IsSimpleGetter(this MethodDefinition original) => 
            (original.IsGetter && (original.Parameters.Count == 0));

        public static bool IsSimplePropertyAccessor(this MethodDefinition method) => 
            (method.IsSimpleGetter() || method.IsSimpleSetter());

        public static bool IsSimpleSetter(this MethodDefinition original) => 
            (original.IsSetter && (original.Parameters.Count == 1));

        public static string PropertyName(this MethodDefinition self) => 
            self.Name.Substring(4);

        public static bool SameAs(this MethodDefinition self, MethodDefinition other) => 
            (self.FullName == other.FullName);
    }
}

