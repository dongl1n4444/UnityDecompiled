namespace Unity.CecilTools
{
    using Mono.Cecil;
    using System;

    public static class ElementType
    {
        public static TypeReference For(TypeReference byRefType)
        {
            TypeSpecification specification = byRefType as TypeSpecification;
            if (specification == null)
            {
                throw new ArgumentException(string.Format("TypeReference isn't a TypeSpecification {0} ", byRefType));
            }
            return specification.ElementType;
        }
    }
}

