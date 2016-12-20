namespace Unity.IL2CPP.Common
{
    using Mono.Cecil;
    using System;

    public interface IAssemblyLoader
    {
        AssemblyDefinition Load(string name);
    }
}

