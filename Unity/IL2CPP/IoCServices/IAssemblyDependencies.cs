namespace Unity.IL2CPP.IoCServices
{
    using Mono.Cecil;
    using System.Collections.Generic;

    public interface IAssemblyDependencies
    {
        IEnumerable<AssemblyDefinition> GetReferencedAssembliesFor(AssemblyDefinition assembly);
    }
}

