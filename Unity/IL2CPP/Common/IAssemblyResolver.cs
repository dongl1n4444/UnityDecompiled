namespace Unity.IL2CPP.Common
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;

    public interface IAssemblyResolver : Mono.Cecil.IAssemblyResolver
    {
        IEnumerable<NPath> GetSearchDirectories();
        bool IsAssemblyCached(AssemblyNameReference assemblyName);
    }
}

