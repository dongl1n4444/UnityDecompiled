namespace Unity.IL2CPP.Common
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;

    public class FieldReferenceComparer : EqualityComparer<FieldReference>
    {
        public static bool AreEqual(FieldReference x, FieldReference y)
        {
            if (object.ReferenceEquals(x, y))
            {
                return true;
            }
            if (!Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(x.DeclaringType, y.DeclaringType, TypeComparisonMode.Exact))
            {
                return false;
            }
            if (!Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(x.FieldType, y.FieldType, TypeComparisonMode.Exact))
            {
                return false;
            }
            return (x.Name == y.Name);
        }

        public override bool Equals(FieldReference x, FieldReference y) => 
            AreEqual(x, y);

        public override int GetHashCode(FieldReference obj) => 
            obj.FullName.GetHashCode();
    }
}

