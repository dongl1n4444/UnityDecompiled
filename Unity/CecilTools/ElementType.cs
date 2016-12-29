namespace Unity.CecilTools
{
    using Mono.Cecil;
    using System;

    public static class ElementType
    {
        public static TypeReference For(TypeReference byRefType)
        {
            TypeSpecification specification = byRefType as TypeSpecification;
            return specification?.ElementType;
        }
    }
}

