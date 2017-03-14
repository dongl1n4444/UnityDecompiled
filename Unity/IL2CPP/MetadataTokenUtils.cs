namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;

    public static class MetadataTokenUtils
    {
        public static AssemblyDefinition AssemblyDefinitionFor(MethodDefinition methodDefinition) => 
            methodDefinition.Module.Assembly;

        public static AssemblyDefinition AssemblyDefinitionFor(MethodReference methodReference) => 
            ResolvedMethodFor(methodReference).Module.Assembly;

        public static AssemblyDefinition AssemblyDefinitionFor(TypeDefinition typeDefinition) => 
            typeDefinition.Module.Assembly;

        public static AssemblyDefinition AssemblyDefinitionFor(TypeReference typeReference) => 
            ResolvedTypeFor(typeReference).Module.Assembly;

        public static string FormatMonoMetadataTokenFor(MethodReference method) => 
            $"{{ "{AssemblyDefinitionFor(method).Name.Name}", {FormattedMetadataTokenFor(method)} }}";

        public static string FormatMonoMetadataTokenFor(TypeReference type) => 
            $"{{ "{AssemblyDefinitionFor(type).Name.Name}", {FormattedMetadataTokenFor(type)} }}";

        public static string FormatMonoMetadataTokenFor(StringMetadataToken stringMetadataToken) => 
            $"{{ "{stringMetadataToken.Assembly.Name.Name}", {stringMetadataToken.Token.ToUInt32()} }}";

        public static string FormattedMetadataTokenFor(FieldReference fieldRef) => 
            $"0x{MetadataTokenFor(fieldRef):X8} /* {fieldRef.FullName} */";

        public static string FormattedMetadataTokenFor(MethodReference methodRef) => 
            $"0x{MetadataTokenFor(methodRef):X8} /* {methodRef.FullName} */";

        public static string FormattedMetadataTokenFor(TypeReference typeRef) => 
            $"0x{MetadataTokenFor(typeRef):X8} /* {typeRef.FullName} */";

        public static string FormattedMetadataTokenFor(uint token, string fullName)
        {
            if (fullName != null)
            {
                return $"0x{token:X8} /* {fullName} */";
            }
            return $"0x{token:X8}";
        }

        public static uint MetadataTokenFor(FieldDefinition fieldDefinition) => 
            fieldDefinition.MetadataToken.ToUInt32();

        public static uint MetadataTokenFor(FieldReference fieldReference)
        {
            FieldDefinition fieldDefinition = fieldReference.Resolve();
            return ((fieldDefinition == null) ? fieldReference.MetadataToken.ToUInt32() : MetadataTokenFor(fieldDefinition));
        }

        public static uint MetadataTokenFor(MethodDefinition methodDefinition) => 
            methodDefinition.MetadataToken.ToUInt32();

        public static uint MetadataTokenFor(MethodReference methodReference) => 
            ResolvedMethodFor(methodReference).MetadataToken.ToUInt32();

        public static uint MetadataTokenFor(TypeDefinition typeDefinition) => 
            typeDefinition.MetadataToken.ToUInt32();

        public static uint MetadataTokenFor(TypeReference typeReference) => 
            ResolvedTypeFor(typeReference).MetadataToken.ToUInt32();

        private static MethodReference ResolvedMethodFor(MethodReference methodReference)
        {
            MethodReference reference = methodReference;
            if (!methodReference.IsGenericInstance && !methodReference.DeclaringType.IsGenericInstance)
            {
                MethodDefinition definition1 = methodReference.Resolve();
                if (definition1 != null)
                {
                    return definition1;
                }
            }
            return reference;
        }

        private static TypeReference ResolvedTypeFor(TypeReference typeReference)
        {
            TypeReference reference = typeReference;
            if (!reference.IsGenericInstance && !typeReference.IsArray)
            {
                TypeDefinition definition1 = typeReference.Resolve();
                if (definition1 != null)
                {
                    return definition1;
                }
            }
            return reference;
        }
    }
}

