namespace Unity.IL2CPP.Common
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;

    public class AssemblyDefinitionComparer : IEqualityComparer<AssemblyDefinition>
    {
        public bool Equals(AssemblyDefinition x, AssemblyDefinition y)
        {
            return (x.FullName == y.FullName);
        }

        public int GetHashCode(AssemblyDefinition obj)
        {
            return obj.FullName.GetHashCode();
        }
    }
}

