namespace Unity.IL2CPP.Common
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class MethodReferenceComparer : EqualityComparer<MethodReference>
    {
        public static bool AreEqual(MethodReference x, MethodReference y)
        {
            if (!object.ReferenceEquals(x, y))
            {
                if (x.HasThis != y.HasThis)
                {
                    return false;
                }
                if (x.HasParameters != y.HasParameters)
                {
                    return false;
                }
                if (x.HasGenericParameters != y.HasGenericParameters)
                {
                    return false;
                }
                if (x.Parameters.Count != y.Parameters.Count)
                {
                    return false;
                }
                if (x.Name != y.Name)
                {
                    return false;
                }
                if (!Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(x.DeclaringType, y.DeclaringType, TypeComparisonMode.Exact))
                {
                    return false;
                }
                GenericInstanceMethod method = x as GenericInstanceMethod;
                GenericInstanceMethod method2 = y as GenericInstanceMethod;
                if ((method != null) || (method2 != null))
                {
                    if ((method == null) || (method2 == null))
                    {
                        return false;
                    }
                    if (method.GenericArguments.Count != method2.GenericArguments.Count)
                    {
                        return false;
                    }
                    for (int i = 0; i < method.GenericArguments.Count; i++)
                    {
                        if (!Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(method.GenericArguments[i], method2.GenericArguments[i], TypeComparisonMode.Exact))
                        {
                            return false;
                        }
                    }
                }
                if (x.Resolve() != y.Resolve())
                {
                    return false;
                }
            }
            return true;
        }

        public static bool AreSignaturesEqual(MethodReference x, MethodReference y, TypeComparisonMode comparisonMode = 0)
        {
            if (x.HasThis != y.HasThis)
            {
                return false;
            }
            if (x.Parameters.Count != y.Parameters.Count)
            {
                return false;
            }
            if (x.GenericParameters.Count != y.GenericParameters.Count)
            {
                return false;
            }
            for (int i = 0; i < x.Parameters.Count; i++)
            {
                if (!Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(x.Parameters[i].ParameterType, y.Parameters[i].ParameterType, comparisonMode))
                {
                    return false;
                }
            }
            if (!Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(x.ReturnType, y.ReturnType, comparisonMode))
            {
                return false;
            }
            return true;
        }

        public override bool Equals(MethodReference x, MethodReference y) => 
            AreEqual(x, y);

        public override int GetHashCode(MethodReference obj) => 
            GetHashCodeFor(obj);

        public static int GetHashCodeFor(MethodReference obj)
        {
            GenericInstanceMethod method = obj as GenericInstanceMethod;
            if (method != null)
            {
                int hashCodeFor = GetHashCodeFor(method.ElementMethod);
                for (int i = 0; i < method.GenericArguments.Count; i++)
                {
                    hashCodeFor = (hashCodeFor * 0x1cfaa2db) + Unity.IL2CPP.Common.TypeReferenceEqualityComparer.GetHashCodeFor(method.GenericArguments[i]);
                }
                return hashCodeFor;
            }
            return ((Unity.IL2CPP.Common.TypeReferenceEqualityComparer.GetHashCodeFor(obj.DeclaringType) * 0x1cfaa2db) + obj.Name.GetHashCode());
        }
    }
}

