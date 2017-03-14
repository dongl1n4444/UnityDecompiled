namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using Unity.IL2CPP.Common;

    internal static class SemiUniqueStableTokenGenerator
    {
        internal static uint GenerateFor(MethodReference method) => 
            ((uint) (method.Module + "_" + method.GetFullName()).GetHashCode());

        internal static uint GenerateFor(TypeReference type) => 
            ((uint) Unity.IL2CPP.Common.TypeReferenceEqualityComparer.GetHashCodeFor(type));

        internal static uint GenerateFor(string literal) => 
            ((uint) literal.GetHashCode());
    }
}

