using System;
using System.Reflection;

namespace Unity.IL2CPP.Portability
{
	public static class MethodInfoExtensions
	{
		public static MethodImplAttributes GetMethodImplementationFlagsPortable(this MethodBase methodInfo)
		{
			return methodInfo.GetMethodImplementationFlags();
		}

		public static RuntimeMethodHandle GetMethodHandlePortable(this MethodBase methodInfo)
		{
			return methodInfo.MethodHandle;
		}
	}
}
