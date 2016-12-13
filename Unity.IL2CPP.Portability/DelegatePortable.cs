using System;
using System.Reflection;

namespace Unity.IL2CPP.Portability
{
	public static class DelegatePortable
	{
		public static Delegate CreateDelegatePortable(Type type, MethodInfo methodInfo)
		{
			return Delegate.CreateDelegate(type, methodInfo);
		}
	}
}
