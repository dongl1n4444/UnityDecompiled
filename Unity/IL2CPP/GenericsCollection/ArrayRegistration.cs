namespace Unity.IL2CPP.GenericsCollection
{
    using Mono.Cecil;
    using System;

    internal static class ArrayRegistration
    {
        public static bool ShouldForce2DArrayFor(TypeDefinition type)
        {
            return (type.MetadataType == MetadataType.Single);
        }
    }
}

