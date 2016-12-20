namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;

    public interface IRuntimeMetadataAccess
    {
        string ArrayInfo(TypeReference elementType);
        string FieldInfo(FieldReference field);
        string HiddenMethodInfo(MethodReference method);
        string Il2CppTypeFor(TypeReference type);
        string Method(MethodReference genericMethod);
        string MethodInfo(MethodReference method);
        bool NeedsBoxingForValueTypeThis(MethodReference method);
        string Newobj(MethodReference ctor);
        string SizeOf(TypeReference type);
        string StaticData(TypeReference type);
        string StringLiteral(string literal);
        string TypeInfoFor(TypeReference type);
    }
}

