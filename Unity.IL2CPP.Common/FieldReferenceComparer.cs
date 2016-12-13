using Mono.Cecil;
using System;
using System.Collections.Generic;

namespace Unity.IL2CPP.Common
{
	public class FieldReferenceComparer : EqualityComparer<FieldReference>
	{
		public override bool Equals(FieldReference x, FieldReference y)
		{
			return FieldReferenceComparer.AreEqual(x, y);
		}

		public override int GetHashCode(FieldReference obj)
		{
			return obj.FullName.GetHashCode();
		}

		public static bool AreEqual(FieldReference x, FieldReference y)
		{
			return object.ReferenceEquals(x, y) || (TypeReferenceEqualityComparer.AreEqual(x.DeclaringType, y.DeclaringType, TypeComparisonMode.Exact) && TypeReferenceEqualityComparer.AreEqual(x.FieldType, y.FieldType, TypeComparisonMode.Exact) && x.Name == y.Name);
		}
	}
}
