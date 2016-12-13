using System;
using System.Runtime.InteropServices;

namespace Unity.IL2CPP.Portability
{
	public static class SafeHandleExtensions
	{
		public static void ClosePortable(this SafeHandle handle)
		{
			handle.Close();
		}
	}
}
