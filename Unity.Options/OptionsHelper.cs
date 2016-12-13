using System;
using System.Collections.Generic;
using System.IO;
using Unity.IL2CPP.Portability;

namespace Unity.Options
{
	public sealed class OptionsHelper
	{
		public const string DefaultArgumentFileName = "args.txt";

		public static string WriteArgumentsFile(string path, IEnumerable<string> args)
		{
			using (Unity.IL2CPP.Portability.StreamWriter streamWriter = new Unity.IL2CPP.Portability.StreamWriter(path))
			{
				foreach (string current in args)
				{
					streamWriter.WriteLine(current);
				}
			}
			return path;
		}

		public static string WriteArgumentFileInDirectory(string directory, IEnumerable<string> args)
		{
			if (!Directory.Exists(directory))
			{
				throw new DirectoryNotFoundException(directory);
			}
			return OptionsHelper.WriteArgumentsFile(Path.Combine(directory, "args.txt"), args);
		}

		public static IEnumerable<string> LoadArgumentsFromFile(string argFile)
		{
			if (!File.Exists(argFile))
			{
				throw new FileNotFoundException(argFile);
			}
			return File.ReadAllLines(argFile);
		}
	}
}
