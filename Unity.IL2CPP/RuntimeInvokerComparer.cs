using Mono.Cecil;
using System;
using System.Collections.Generic;
using Unity.IL2CPP.Common;

namespace Unity.IL2CPP
{
	internal class RuntimeInvokerComparer : EqualityComparer<TypeReference[]>
	{
		public override bool Equals(TypeReference[] x, TypeReference[] y)
		{
			bool result;
			if (x.Length != y.Length)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < x.Length; i++)
				{
					if (!TypeReferenceEqualityComparer.AreEqual(x[i], y[i], TypeComparisonMode.Exact))
					{
						result = false;
						return result;
					}
				}
				result = true;
			}
			return result;
		}

		public override int GetHashCode(TypeReference[] obj)
		{
			int num = 31 * obj.Length;
			for (int i = 0; i < obj.Length; i++)
			{
				num += 7 * TypeReferenceEqualityComparer.GetHashCodeFor(obj[i]);
			}
			return num;
		}
	}
}
