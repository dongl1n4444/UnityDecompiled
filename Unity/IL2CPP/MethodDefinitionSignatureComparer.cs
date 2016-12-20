namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using Unity.IL2CPP.Common;

    internal class MethodDefinitionSignatureComparer : IEqualityComparer<MethodDefinition>
    {
        public bool Equals(MethodDefinition x, MethodDefinition y)
        {
            if (x.HasGenericParameters != y.HasGenericParameters)
            {
                return false;
            }
            if (x.FullName != y.FullName)
            {
                return false;
            }
            if (x.Module == y.Module)
            {
                return true;
            }
            if ((x.Module == null) || (y.Module == null))
            {
                return false;
            }
            if (x.Module.Assembly == y.Module.Assembly)
            {
                return true;
            }
            AssemblyDefinitionComparer comparer = new AssemblyDefinitionComparer();
            return comparer.Equals(x.Module.Assembly, y.Module.Assembly);
        }

        public int GetHashCode(MethodDefinition obj)
        {
            return obj.FullName.GetHashCode();
        }
    }
}

