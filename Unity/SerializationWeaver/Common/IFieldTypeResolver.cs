namespace Unity.SerializationWeaver.Common
{
    using Mono.Cecil;

    public interface IFieldTypeResolver
    {
        TypeReference TypeOf(FieldReference fieldDefinition);
    }
}

