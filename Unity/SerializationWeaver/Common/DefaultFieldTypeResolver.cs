namespace Unity.SerializationWeaver.Common
{
    using Mono.Cecil;
    using System;

    public class DefaultFieldTypeResolver : IFieldTypeResolver
    {
        public TypeReference TypeOf(FieldReference fieldDefinition)
        {
            return fieldDefinition.FieldType;
        }
    }
}

