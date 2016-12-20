namespace Unity.IL2CPP.Metadata
{
    using Mono.Cecil;
    using System;
    using Unity.IL2CPP.Common;
    using Unity.IL2CPP.ILPreProcessor;

    internal class VirtualMethodResolution
    {
        internal static bool MethodSignaturesMatch(MethodReference candidate, MethodReference method)
        {
            if (candidate.HasThis != method.HasThis)
            {
                return false;
            }
            return MethodSignaturesMatchIgnoreStaticness(candidate, method);
        }

        internal static bool MethodSignaturesMatchIgnoreStaticness(MethodReference candidate, MethodReference method)
        {
            if (candidate.Parameters.Count != method.Parameters.Count)
            {
                return false;
            }
            if (candidate.GenericParameters.Count != method.GenericParameters.Count)
            {
                return false;
            }
            Unity.IL2CPP.ILPreProcessor.TypeResolver resolver = new Unity.IL2CPP.ILPreProcessor.TypeResolver(candidate.DeclaringType as GenericInstanceType, candidate as GenericInstanceMethod);
            Unity.IL2CPP.ILPreProcessor.TypeResolver resolver2 = new Unity.IL2CPP.ILPreProcessor.TypeResolver(method.DeclaringType as GenericInstanceType, method as GenericInstanceMethod);
            if (!Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(resolver.ResolveReturnType(candidate), resolver2.ResolveReturnType(method), TypeComparisonMode.SignatureOnly))
            {
                return false;
            }
            for (int i = 0; i < candidate.Parameters.Count; i++)
            {
                if (!Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(resolver.ResolveParameterType(candidate, candidate.Parameters[i]), resolver2.ResolveParameterType(method, method.Parameters[i]), TypeComparisonMode.SignatureOnly))
                {
                    return false;
                }
            }
            return true;
        }
    }
}

