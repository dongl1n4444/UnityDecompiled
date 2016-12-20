﻿namespace Unity.IL2CPP.Common
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;

    public class AssemblyNameReferenceComparer : IEqualityComparer<AssemblyNameReference>
    {
        public bool Equals(AssemblyNameReference x, AssemblyNameReference y)
        {
            return (x.FullName == y.FullName);
        }

        public int GetHashCode(AssemblyNameReference obj)
        {
            return obj.FullName.GetHashCode();
        }
    }
}

