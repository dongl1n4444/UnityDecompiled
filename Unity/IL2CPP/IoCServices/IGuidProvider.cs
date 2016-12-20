namespace Unity.IL2CPP.IoCServices
{
    using Mono.Cecil;
    using System;

    public interface IGuidProvider
    {
        Guid GuidFor(TypeReference type);
    }
}

