namespace Unity.Options
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal class AssemblyNameComparer : IEqualityComparer<AssemblyName>
    {
        public bool Equals(AssemblyName x, AssemblyName y) => 
            (x.FullName == y.FullName);

        public int GetHashCode(AssemblyName obj) => 
            obj.FullName.GetHashCode();
    }
}

