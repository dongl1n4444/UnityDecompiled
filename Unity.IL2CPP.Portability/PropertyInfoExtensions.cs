using System;
using System.Reflection;

namespace Unity.IL2CPP.Portability
{
	public static class PropertyInfoExtensions
	{
		public static MethodInfo GetGetMethodPortable(this PropertyInfo propertyInfo)
		{
			return propertyInfo.GetGetMethod();
		}
	}
}
