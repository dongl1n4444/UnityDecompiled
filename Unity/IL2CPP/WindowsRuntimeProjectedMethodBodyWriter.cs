namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using System.Runtime.CompilerServices;

    public delegate void WindowsRuntimeProjectedMethodBodyWriter(MethodDefinition method, CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess);
}

