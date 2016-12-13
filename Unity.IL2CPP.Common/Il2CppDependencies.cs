using NiceIO;
using System;

namespace Unity.IL2CPP.Common
{
	internal static class Il2CppDependencies
	{
		private static readonly NPath _root;

		internal static bool Available
		{
			get
			{
				return Il2CppDependencies._root != null;
			}
		}

		internal static bool HasUnusedByteCodeStripper
		{
			get
			{
				return Il2CppDependencies.Available && Il2CppDependencies.UnusedByteCodeStripper.Exists("");
			}
		}

		internal static NPath Root
		{
			get
			{
				if (Il2CppDependencies._root == null)
				{
					throw new InvalidOperationException("No il2cpp dependencies found");
				}
				return Il2CppDependencies._root;
			}
		}

		internal static NPath UnusedByteCodeStripper
		{
			get
			{
				return Il2CppDependencies._root.Combine(new string[]
				{
					"UnusedByteCodeStripper2",
					"UnusedBytecodeStripper2.exe"
				});
			}
		}

		static Il2CppDependencies()
		{
			NPath nPath = CommonPaths.Il2CppRoot.ParentContaining("il2cpp-dependencies");
			if (nPath != null)
			{
				Il2CppDependencies._root = nPath.Combine(new string[]
				{
					"il2cpp-dependencies"
				});
			}
		}

		public static NPath MonoInstall(string installName)
		{
			return Il2CppDependencies._root.Combine(new string[]
			{
				installName,
				"builds",
				"monodistribution"
			});
		}
	}
}
