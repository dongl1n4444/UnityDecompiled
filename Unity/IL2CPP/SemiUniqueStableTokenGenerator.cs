namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using Unity.IL2CPP.Common;

    internal static class SemiUniqueStableTokenGenerator
    {
        internal static uint GenerateFor(MethodReference method)
        {
            return (uint) (method.Module + "_" + method.FullName).GetHashCode();
        }

        internal static uint GenerateFor(TypeReference type)
        {
            return (uint) Unity.IL2CPP.Common.TypeReferenceEqualityComparer.GetHashCodeFor(type);
        }

        internal static uint GenerateFor(string literal)
        {
            return (uint) literal.GetHashCode();
        }
    }
}

