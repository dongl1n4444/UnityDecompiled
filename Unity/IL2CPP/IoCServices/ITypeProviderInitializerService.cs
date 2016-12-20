namespace Unity.IL2CPP.IoCServices
{
    using Mono.Cecil;
    using System;

    public interface ITypeProviderInitializerService
    {
        void Initialize(AssemblyDefinition mscorlib);
    }
}

