using System;
using System.Diagnostics;

namespace Unity.IL2CPP.Portability
{
	public static class ProcessExtensions
	{
		public static IntPtr GetHandlePortable(this Process process)
		{
			return process.Handle;
		}
	}
}
