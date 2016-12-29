namespace Unity.IL2CPP.GenericsCollection
{
    using Mono.Cecil;
    using System;

    internal static class ArrayRegistration
    {
        public static bool ShouldForce2DArrayFor(TypeDefinition type) => 
            (type.MetadataType == MetadataType.Single);
    }
}

