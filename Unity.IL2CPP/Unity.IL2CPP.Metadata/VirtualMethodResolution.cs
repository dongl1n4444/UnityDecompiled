using Mono.Cecil;
using System;
using Unity.IL2CPP.Common;
using Unity.IL2CPP.ILPreProcessor;

namespace Unity.IL2CPP.Metadata
{
	internal class VirtualMethodResolution
	{
		internal static bool MethodSignaturesMatch(MethodReference candidate, MethodReference method)
		{
			return candidate.HasThis == method.HasThis && VirtualMethodResolution.MethodSignaturesMatchIgnoreStaticness(candidate, method);
		}

		internal static bool MethodSignaturesMatchIgnoreStaticness(MethodReference candidate, MethodReference method)
		{
			bool result;
			if (candidate.Parameters.Count != method.Parameters.Count)
			{
				result = false;
			}
			else if (candidate.GenericParameters.Count != method.GenericParameters.Count)
			{
				result = false;
			}
			else
			{
				TypeResolver typeResolver = new TypeResolver(candidate.DeclaringType as GenericInstanceType, candidate as GenericInstanceMethod);
				TypeResolver typeResolver2 = new TypeResolver(method.DeclaringType as GenericInstanceType, method as GenericInstanceMethod);
				if (!TypeReferenceEqualityComparer.AreEqual(typeResolver.ResolveReturnType(candidate), typeResolver2.ResolveReturnType(method), TypeComparisonMode.SignatureOnly))
				{
					result = false;
				}
				else
				{
					for (int i = 0; i < candidate.Parameters.Count; i++)
					{
						if (!TypeReferenceEqualityComparer.AreEqual(typeResolver.ResolveParameterType(candidate, candidate.Parameters[i]), typeResolver2.ResolveParameterType(method, method.Parameters[i]), TypeComparisonMode.SignatureOnly))
						{
							result = false;
							return result;
						}
					}
					result = true;
				}
			}
			return result;
		}
	}
}
