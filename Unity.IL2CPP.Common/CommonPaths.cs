using NiceIO;
using System;
using System.IO;
using Unity.IL2CPP.Portability;

namespace Unity.IL2CPP.Common
{
	public class CommonPaths
	{
		private static NPath _il2cppRoot;

		public static NPath Il2CppRoot
		{
			get
			{
				if (CommonPaths._il2cppRoot == null)
				{
					Uri uri = new Uri(typeof(CommonPaths).GetAssemblyPortable().GetCodeBasePortable());
					string text = uri.LocalPath;
					if (!string.IsNullOrEmpty(uri.Fragment))
					{
						text += Uri.UnescapeDataString(uri.Fragment);
					}
					CommonPaths._il2cppRoot = new NPath(text).ParentContaining("il2cpp_root");
				}
				return CommonPaths._il2cppRoot;
			}
		}

		public static NPath Il2CppBuild
		{
			get
			{
				return CommonPaths.Il2CppRoot.Combine(new string[]
				{
					"build"
				});
			}
		}

		public static bool Il2CppDependenciesAvailable
		{
			get
			{
				return CommonPaths.Il2CppRoot.ParentContaining("il2cpp-dependencies") != null;
			}
		}

		public static NPath Il2CppDependencies
		{
			get
			{
				NPath nPath = CommonPaths.Il2CppRoot.ParentContaining("il2cpp-dependencies");
				if (nPath == null)
				{
					throw new DirectoryNotFoundException(CommonPaths.Il2CppRoot.Combine(new string[]
					{
						"il2cpp-dependencies"
					}).ToString());
				}
				return nPath.Combine(new string[]
				{
					"il2cpp-dependencies"
				});
			}
		}

		public static string Il2CppPath(string path)
		{
			return CommonPaths.Il2CppRoot.Combine(new string[]
			{
				path
			}).ToString();
		}
	}
}
