namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using Unity.IL2CPP.Common;

    internal class RuntimeInvokerComparer : EqualityComparer<TypeReference[]>
    {
        public override bool Equals(TypeReference[] x, TypeReference[] y)
        {
            if (x.Length != y.Length)
            {
                return false;
            }
            for (int i = 0; i < x.Length; i++)
            {
                if (!Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(x[i], y[i], TypeComparisonMode.Exact))
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode(TypeReference[] obj)
        {
            int num = 0x1f * obj.Length;
            for (int i = 0; i < obj.Length; i++)
            {
                num += 7 * Unity.IL2CPP.Common.TypeReferenceEqualityComparer.GetHashCodeFor(obj[i]);
            }
            return num;
        }
    }
}

