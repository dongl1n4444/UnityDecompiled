using System;
using System.Collections.Generic;
using System.Reflection;

namespace Unity.Options
{
	internal class AssemblyNameComparer : IEqualityComparer<AssemblyName>
	{
		public bool Equals(AssemblyName x, AssemblyName y)
		{
			return x.FullName == y.FullName;
		}

		public int GetHashCode(AssemblyName obj)
		{
			return obj.FullName.GetHashCode();
		}
	}
}
