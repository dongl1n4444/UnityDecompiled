namespace Unity.Options
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Unity.IL2CPP.Portability;

    public sealed class OptionsHelper
    {
        public const string DefaultArgumentFileName = "args.txt";

        public static IEnumerable<string> LoadArgumentsFromFile(string argFile)
        {
            if (!File.Exists(argFile))
            {
                throw new FileNotFoundException(argFile);
            }
            return File.ReadAllLines(argFile);
        }

        public static string WriteArgumentFileInDirectory(string directory, IEnumerable<string> args)
        {
            if (!Directory.Exists(directory))
            {
                throw new DirectoryNotFoundException(directory);
            }
            return WriteArgumentsFile(Path.Combine(directory, "args.txt"), args);
        }

        public static string WriteArgumentsFile(string path, IEnumerable<string> args)
        {
            using (Unity.IL2CPP.Portability.StreamWriter writer = new Unity.IL2CPP.Portability.StreamWriter(path))
            {
                foreach (string str in args)
                {
                    writer.WriteLine(str);
                }
            }
            return path;
        }
    }
}

