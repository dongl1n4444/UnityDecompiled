namespace Unity.IL2CPP.IoCServices
{
    using Mono.Cecil;
    using System;

    public interface IIl2CppFieldReferenceCollectorWriterService
    {
        uint GetOrCreateIndex(FieldReference field);
    }
}

