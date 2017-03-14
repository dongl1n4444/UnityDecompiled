namespace Unity.IL2CPP.IoCServices
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using Unity.IL2CPP;

    public interface IRuntimeImplementedMethodWriter
    {
        IEnumerable<RuntimeGenericTypeData> GetGenericSharingDataFor(MethodDefinition method);
        bool IsRuntimeImplementedMethod(MethodDefinition method);
        void WriteMethodBody(CppCodeWriter writer, MethodReference method, IRuntimeMetadataAccess metadataAccess);
    }
}

