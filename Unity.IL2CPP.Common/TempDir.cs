using NiceIO;
using System;
using System.Linq;

namespace Unity.IL2CPP.Common
{
	public static class TempDir
	{
		public const string CppRunnerCachePostFix = "_cpprunner_cache";

		public static NPath Root
		{
			get
			{
				string environmentVariable = Environment.GetEnvironmentVariable("IL2CPP_CUSTOM_TEMP_DIR");
				NPath result;
				if (!string.IsNullOrEmpty(environmentVariable))
				{
					result = environmentVariable.ToNPath().EnsureDirectoryExists("");
				}
				else
				{
					result = NPath.SystemTemp;
				}
				return result;
			}
		}

		public static NPath Il2CppTemporaryDirectoryRoot
		{
			get
			{
				return TempDir.Root.Combine(new string[]
				{
					"il2cpp"
				});
			}
		}

		public static NPath Empty(string nameSuggestion)
		{
			int num = 0;
			NPath result;
			while (true)
			{
				NPath nPath = TempDir.Il2CppTemporaryDirectoryRoot.Combine(new string[]
				{
					nameSuggestion + ((num != 0) ? num.ToString() : "")
				});
				try
				{
					if (TempDir.ForgivingCleanDirectory(nPath))
					{
						result = nPath.CreateDirectory();
						break;
					}
					num++;
				}
				catch (Exception)
				{
					Console.WriteLine("Failed getting an empty tempdir at: " + nPath + ", going to try again with a new name.");
					num++;
				}
			}
			return result;
		}

		public static TempDirContext Begin(string nameSuggestion = "", bool noClean = false)
		{
			return new TempDirContext(TempDir.Empty(nameSuggestion), noClean);
		}

		public static bool ForgivingCleanDirectory(NPath directory)
		{
			bool result;
			try
			{
				if (directory.Exists(""))
				{
					directory.Delete(DeleteMode.Normal);
				}
				result = true;
				return result;
			}
			catch (Exception)
			{
				if (!directory.Files(true).Any<NPath>())
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}
	}
}
