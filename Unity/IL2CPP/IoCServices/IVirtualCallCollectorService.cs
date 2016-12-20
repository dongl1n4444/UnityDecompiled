namespace Unity.IL2CPP.IoCServices
{
    using Mono.Cecil;
    using NiceIO;
    using System;

    public interface IVirtualCallCollectorService
    {
        void AddMethod(MethodReference method);
        UnresolvedVirtualsTablesInfo WriteUnresolvedStubs(NPath outputDir);

        int Count { get; }
    }
}

