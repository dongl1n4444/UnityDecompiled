namespace Unity.IL2CPP.IoCServices
{
    using Mono.Cecil;
    using System;
    using Unity.IL2CPP.Metadata;

    public interface IIl2CppMethodReferenceCollectorWriterService
    {
        uint GetOrCreateIndex(MethodReference method, IMetadataCollection metadataCollection);
    }
}

