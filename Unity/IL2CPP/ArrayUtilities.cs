namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;

    public class ArrayUtilities
    {
        public static TypeReference ArrayElementTypeOf(TypeReference typeReference)
        {
            ArrayType type = typeReference as ArrayType;
            if (type != null)
            {
                return type.ElementType;
            }
            TypeSpecification specification = typeReference as TypeSpecification;
            if (specification == null)
            {
                throw new ArgumentException($"{typeReference.FullName} is not an array type", "typeReference");
            }
            return ArrayElementTypeOf(specification.ElementType);
        }

        internal static ModuleDefinition ModuleDefinitionForElementTypeOf(ArrayType arrayType) => 
            arrayType.ElementType.Resolve().Module;
    }
}

