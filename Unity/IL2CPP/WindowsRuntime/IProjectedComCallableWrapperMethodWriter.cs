namespace Unity.IL2CPP.WindowsRuntime
{
    using Mono.Cecil;
    using System;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Marshaling.BodyWriters.NativeToManaged;

    public interface IProjectedComCallableWrapperMethodWriter
    {
        ComCallableWrapperMethodBodyWriter GetBodyWriter(MethodReference method);
        void WriteDependenciesFor(CppCodeWriter writer, TypeReference interfaceType);
    }
}

