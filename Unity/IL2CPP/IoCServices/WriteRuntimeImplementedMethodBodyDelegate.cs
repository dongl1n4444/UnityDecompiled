namespace Unity.IL2CPP.IoCServices
{
    using Mono.Cecil;
    using System;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP;

    public delegate void WriteRuntimeImplementedMethodBodyDelegate(CppCodeWriter writer, MethodReference method, IRuntimeMetadataAccess metadataAccess);
}

