using System;
using System.Reflection;

namespace Unity.IL2CPP.Portability
{
	public static class DelegateExtensions
	{
		public static MethodInfo GetMethodInfoPortable(this Delegate del)
		{
			return del.Method;
		}
	}
}
