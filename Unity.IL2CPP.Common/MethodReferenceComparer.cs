using Mono.Cecil;
using System;
using System.Collections.Generic;

namespace Unity.IL2CPP.Common
{
	public class MethodReferenceComparer : EqualityComparer<MethodReference>
	{
		public override bool Equals(MethodReference x, MethodReference y)
		{
			return MethodReferenceComparer.AreEqual(x, y);
		}

		public override int GetHashCode(MethodReference obj)
		{
			return MethodReferenceComparer.GetHashCodeFor(obj);
		}

		public static bool AreEqual(MethodReference x, MethodReference y)
		{
			bool result;
			if (object.ReferenceEquals(x, y))
			{
				result = true;
			}
			else if (x.HasThis != y.HasThis)
			{
				result = false;
			}
			else if (x.HasParameters != y.HasParameters)
			{
				result = false;
			}
			else if (x.HasGenericParameters != y.HasGenericParameters)
			{
				result = false;
			}
			else if (x.Parameters.Count != y.Parameters.Count)
			{
				result = false;
			}
			else if (x.Name != y.Name)
			{
				result = false;
			}
			else if (!TypeReferenceEqualityComparer.AreEqual(x.DeclaringType, y.DeclaringType, TypeComparisonMode.Exact))
			{
				result = false;
			}
			else
			{
				GenericInstanceMethod genericInstanceMethod = x as GenericInstanceMethod;
				GenericInstanceMethod genericInstanceMethod2 = y as GenericInstanceMethod;
				if (genericInstanceMethod != null || genericInstanceMethod2 != null)
				{
					if (genericInstanceMethod == null || genericInstanceMethod2 == null)
					{
						result = false;
						return result;
					}
					if (genericInstanceMethod.GenericArguments.Count != genericInstanceMethod2.GenericArguments.Count)
					{
						result = false;
						return result;
					}
					for (int i = 0; i < genericInstanceMethod.GenericArguments.Count; i++)
					{
						if (!TypeReferenceEqualityComparer.AreEqual(genericInstanceMethod.GenericArguments[i], genericInstanceMethod2.GenericArguments[i], TypeComparisonMode.Exact))
						{
							result = false;
							return result;
						}
					}
				}
				result = (x.Resolve() == y.Resolve());
			}
			return result;
		}

		public static bool AreSignaturesEqual(MethodReference x, MethodReference y, TypeComparisonMode comparisonMode = TypeComparisonMode.Exact)
		{
			bool result;
			if (x.HasThis != y.HasThis)
			{
				result = false;
			}
			else if (x.Parameters.Count != y.Parameters.Count)
			{
				result = false;
			}
			else if (x.GenericParameters.Count != y.GenericParameters.Count)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < x.Parameters.Count; i++)
				{
					if (!TypeReferenceEqualityComparer.AreEqual(x.Parameters[i].ParameterType, y.Parameters[i].ParameterType, comparisonMode))
					{
						result = false;
						return result;
					}
				}
				result = TypeReferenceEqualityComparer.AreEqual(x.ReturnType, y.ReturnType, comparisonMode);
			}
			return result;
		}

		public static int GetHashCodeFor(MethodReference obj)
		{
			GenericInstanceMethod genericInstanceMethod = obj as GenericInstanceMethod;
			int result;
			if (genericInstanceMethod != null)
			{
				int num = MethodReferenceComparer.GetHashCodeFor(genericInstanceMethod.ElementMethod);
				for (int i = 0; i < genericInstanceMethod.GenericArguments.Count; i++)
				{
					num = num * 486187739 + TypeReferenceEqualityComparer.GetHashCodeFor(genericInstanceMethod.GenericArguments[i]);
				}
				result = num;
			}
			else
			{
				result = TypeReferenceEqualityComparer.GetHashCodeFor(obj.DeclaringType) * 486187739 + obj.Name.GetHashCode();
			}
			return result;
		}
	}
}
