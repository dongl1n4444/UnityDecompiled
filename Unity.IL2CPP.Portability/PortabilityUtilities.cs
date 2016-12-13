using System;

namespace Unity.IL2CPP.Portability
{
	public static class PortabilityUtilities
	{
		public static bool IsWindows()
		{
			PlatformID platform = Environment.OSVersion.Platform;
			return platform == PlatformID.Win32Windows || platform == PlatformID.Win32NT;
		}
	}
}
