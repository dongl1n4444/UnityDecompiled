using System;
using System.Reflection;

namespace Unity.IL2CPP.Portability
{
	public static class CustomAttributeDataExtensions
	{
		public static ConstructorInfo GetConstructorInfoPortable(this CustomAttributeData target)
		{
			return target.Constructor;
		}
	}
}
