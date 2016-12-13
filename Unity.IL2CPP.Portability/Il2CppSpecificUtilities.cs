using System;
using System.IO;

namespace Unity.IL2CPP.Portability
{
	public static class Il2CppSpecificUtilities
	{
		public static string GetIl2CppSolutionDirectory()
		{
			Uri uri = new Uri(typeof(PortabilityUtilities).Assembly.GetCodeBasePortable());
			string text = uri.LocalPath;
			if (!string.IsNullOrEmpty(uri.Fragment))
			{
				text += Uri.UnescapeDataString(uri.Fragment);
			}
			DirectoryInfo directoryInfo = new DirectoryInfo(text);
			while (directoryInfo.Parent != null)
			{
				string path = Path.Combine(directoryInfo.FullName, "il2cpp.sln");
				if (File.Exists(path))
				{
					string result = directoryInfo.FullName;
					return result;
				}
				directoryInfo = directoryInfo.Parent;
			}
			string path2 = Path.Combine(Environment.CurrentDirectory, Path.Combine("..", "il2cpp.sln"));
			if (File.Exists(path2))
			{
				string result = Path.GetFullPath(path2);
				return result;
			}
			string environmentVariable = Environment.GetEnvironmentVariable("IL2CPP_TESTHARNESS_SOLUTION_DIRECTORY");
			if (!string.IsNullOrEmpty(environmentVariable))
			{
				string result = environmentVariable;
				return result;
			}
			throw new InvalidOperationException("Unable to determine the il2cpp solution directory");
		}

		public static string GetIl2CppBuildDirectory()
		{
			return Path.Combine(Il2CppSpecificUtilities.GetIl2CppSolutionDirectory(), "build");
		}
	}
}
