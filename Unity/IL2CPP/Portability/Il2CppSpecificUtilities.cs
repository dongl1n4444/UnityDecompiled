namespace Unity.IL2CPP.Portability
{
    using System;
    using System.IO;

    public static class Il2CppSpecificUtilities
    {
        public static string GetIl2CppBuildDirectory() => 
            Path.Combine(GetIl2CppSolutionDirectory(), "build");

        public static string GetIl2CppSolutionDirectory()
        {
            Uri uri = new Uri(typeof(PortabilityUtilities).Assembly.GetCodeBasePortable());
            string localPath = uri.LocalPath;
            if (!string.IsNullOrEmpty(uri.Fragment))
            {
                localPath = localPath + Uri.UnescapeDataString(uri.Fragment);
            }
            for (DirectoryInfo info = new DirectoryInfo(localPath); info.Parent != null; info = info.Parent)
            {
                if (File.Exists(Path.Combine(info.FullName, "il2cpp.sln")))
                {
                    return info.FullName;
                }
            }
            string path = Path.Combine(Environment.CurrentDirectory, Path.Combine("..", "il2cpp.sln"));
            if (File.Exists(path))
            {
                return Path.GetFullPath(path);
            }
            string environmentVariable = Environment.GetEnvironmentVariable("IL2CPP_TESTHARNESS_SOLUTION_DIRECTORY");
            if (string.IsNullOrEmpty(environmentVariable))
            {
                throw new InvalidOperationException("Unable to determine the il2cpp solution directory");
            }
            return environmentVariable;
        }
    }
}

