using System;
using System.IO;
using System.Reflection;
using Unity.IL2CPP.Portability;

namespace Unity.IL2CPP.Common
{
	public static class PlatformUtils
	{
		public enum Architecture
		{
			UseManagedRuntimeArchitecture,
			x86,
			x64
		}

		private static bool _runningWithMono;

		private static bool _runningOnIl2Cpp;

		private static PlatformUtils.Architecture _nativeCompilerArchitecture;

		public static PlatformUtils.Architecture ManagedRuntimeArchitecture
		{
			get
			{
				return (IntPtr.Size != 4) ? PlatformUtils.Architecture.x64 : PlatformUtils.Architecture.x86;
			}
		}

		public static PlatformUtils.Architecture NativeCompilerArchitecture
		{
			get
			{
				return (PlatformUtils._nativeCompilerArchitecture != PlatformUtils.Architecture.UseManagedRuntimeArchitecture) ? PlatformUtils._nativeCompilerArchitecture : PlatformUtils.ManagedRuntimeArchitecture;
			}
			set
			{
				PlatformUtils._nativeCompilerArchitecture = value;
			}
		}

		static PlatformUtils()
		{
			Type type = Type.GetType("Mono.Runtime");
			if (type != null)
			{
				MethodInfo method = type.GetMethod("GetDisplayName", BindingFlags.Static | BindingFlags.NonPublic);
				string text = (string)method.Invoke(null, null);
				if (text.Contains("IL2CPP"))
				{
					PlatformUtils._runningOnIl2Cpp = true;
				}
				else
				{
					PlatformUtils._runningWithMono = true;
				}
			}
		}

		public static bool IsWindows()
		{
			return PortabilityUtilities.IsWindows();
		}

		public static bool IsLinux()
		{
			return !PlatformUtils.IsWindows() && Directory.Exists("/proc");
		}

		public static bool IsOSX()
		{
			return !PlatformUtils.IsWindows() && !PlatformUtils.IsLinux();
		}

		public static bool RunningWithMono()
		{
			return PlatformUtils._runningWithMono;
		}

		public static bool RunningOnIl2Cpp()
		{
			return PlatformUtils._runningOnIl2Cpp;
		}
	}
}
