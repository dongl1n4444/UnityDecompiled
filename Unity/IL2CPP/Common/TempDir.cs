namespace Unity.IL2CPP.Common
{
    using NiceIO;
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;

    public static class TempDir
    {
        public const string CppRunnerCachePostFix = "_cpprunner_cache";

        public static TempDirContext Begin([Optional, DefaultParameterValue("")] string nameSuggestion, [Optional, DefaultParameterValue(false)] bool noClean)
        {
            return new TempDirContext(Empty(nameSuggestion), noClean);
        }

        public static NPath Empty(string nameSuggestion)
        {
            int num = 0;
            while (true)
            {
                string[] append = new string[] { nameSuggestion + ((num != 0) ? num.ToString() : "") };
                NPath directory = Il2CppTemporaryDirectoryRoot.Combine(append);
                try
                {
                    if (ForgivingCleanDirectory(directory))
                    {
                        return directory.CreateDirectory();
                    }
                    num++;
                }
                catch (Exception)
                {
                    Console.WriteLine("Failed getting an empty tempdir at: " + directory + ", going to try again with a new name.");
                    num++;
                }
            }
        }

        public static bool ForgivingCleanDirectory(NPath directory)
        {
            try
            {
                if (directory.Exists(""))
                {
                    directory.Delete(DeleteMode.Normal);
                }
                return true;
            }
            catch (Exception)
            {
                if (!Enumerable.Any<NPath>(directory.Files(true)))
                {
                    return true;
                }
            }
            return false;
        }

        public static NPath Il2CppTemporaryDirectoryRoot
        {
            get
            {
                string[] append = new string[] { "il2cpp" };
                return Root.Combine(append);
            }
        }

        public static NPath Root
        {
            get
            {
                string environmentVariable = Environment.GetEnvironmentVariable("IL2CPP_CUSTOM_TEMP_DIR");
                if (!string.IsNullOrEmpty(environmentVariable))
                {
                    return Extensions.ToNPath(environmentVariable).EnsureDirectoryExists("");
                }
                return NPath.SystemTemp;
            }
        }
    }
}

