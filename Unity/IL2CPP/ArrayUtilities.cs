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
                throw new ArgumentException(string.Format("{0} is not an array type", typeReference.FullName), "typeReference");
            }
            return ArrayElementTypeOf(specification.ElementType);
        }

        internal static ModuleDefinition ModuleDefinitionForElementTypeOf(ArrayType arrayType)
        {
            return arrayType.ElementType.Resolve().Module;
        }
    }
}

