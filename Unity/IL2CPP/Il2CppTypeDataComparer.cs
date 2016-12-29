namespace Unity.IL2CPP
{
    using System;
    using System.Collections.Generic;
    using Unity.IL2CPP.Common;

    internal class Il2CppTypeDataComparer : EqualityComparer<Il2CppTypeData>
    {
        public override bool Equals(Il2CppTypeData x, Il2CppTypeData y) => 
            ((x.Attrs == y.Attrs) && TypeReferenceEqualityComparer.AreEqual(x.Type, y.Type, TypeComparisonMode.Exact));

        public override int GetHashCode(Il2CppTypeData obj) => 
            (TypeReferenceEqualityComparer.GetHashCodeFor(obj.Type) + obj.Attrs);
    }
}

