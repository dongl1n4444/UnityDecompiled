using NiceIO;
using System;

namespace Unity.IL2CPP.Common
{
	public static class UnitySourceCode
	{
		public static class Paths
		{
			private static readonly NPath _unityRoot;

			public static NPath UnityRoot
			{
				get
				{
					return UnitySourceCode.Paths._unityRoot;
				}
			}

			private static NPath EditorToolsPath
			{
				get
				{
					NPath result;
					if (PlatformUtils.IsWindows())
					{
						result = UnitySourceCode.Paths.UnityRoot.Combine(new string[]
						{
							"build/WindowsEditor/Data/Tools"
						});
					}
					else if (PlatformUtils.IsLinux())
					{
						result = UnitySourceCode.Paths.UnityRoot.Combine(new string[]
						{
							"build/LinuxEditor/Data/Tools/"
						});
					}
					else
					{
						result = UnitySourceCode.Paths.UnityRoot.Combine(new string[]
						{
							"build/MacEditor/Unity.app/Contents/Tools"
						});
					}
					return result;
				}
			}

			public static NPath UnusedBytecodeStripper
			{
				get
				{
					return UnitySourceCode.Paths.EditorToolsPath.Combine(new string[]
					{
						"UnusedByteCodeStripper2/UnusedBytecodeStripper2.exe"
					});
				}
			}

			static Paths()
			{
				string environmentVariable = Environment.GetEnvironmentVariable("IL2CPP_UNITY_ROOT");
				if (!string.IsNullOrEmpty(environmentVariable))
				{
					UnitySourceCode.Paths._unityRoot = environmentVariable.ToNPath();
				}
				else
				{
					UnitySourceCode.Paths._unityRoot = ((!(CommonPaths.Il2CppRoot != null)) ? null : CommonPaths.Il2CppRoot.ParentContaining("build.pl"));
				}
			}
		}

		public static bool Available
		{
			get
			{
				return UnitySourceCode.Paths.UnityRoot != null;
			}
		}
	}
}
