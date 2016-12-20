namespace Unity.IL2CPP.Running
{
    using Microsoft.Win32;
    using NiceIO;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Building.ToolChains;
    using Unity.IL2CPP.Common;
    using Unity.TinyProfiling;

    internal class WinRTRunner : Runner
    {
        public override string GetActivateSignature()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("int WINAPI wWinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPWSTR lpCmdLine, int nShowCmd)");
            builder.AppendLine("{");
            builder.AppendLine("\treturn WinRT::Activate(EntryPoint);");
            builder.AppendLine("}");
            return builder.ToString();
        }

        public override IEnumerable<string> GetAdditionalIncludes()
        {
            return new string[] { "windows.h", "ActivateApp.h" };
        }

        private static string GetMsBuildPathAtVersion(string msvcVersionNumber)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(string.Format(@"SOFTWARE\Microsoft\MSBuild\ToolsVersions\{0}", msvcVersionNumber));
            if (key == null)
            {
                return null;
            }
            string str2 = (string) key.GetValue("MSBuildToolsPath");
            if (str2 == null)
            {
                return null;
            }
            return Path.Combine(str2, "MSBuild.exe");
        }

        private static void MakeSureRunnerIsBuilt()
        {
            using (TinyProfiler.Section("MakeSureRunnerIsBuilt", ""))
            {
                Console.WriteLine("Making sure WinRT runner is built.");
                Shell.ExecuteAndCaptureOutput(MSBuildPath, WinRTRunnerSolutionPath.InQuotes(), null);
            }
        }

        public override Shell.ExecuteResult RunAndMakeExecuteResult(string executable)
        {
            bool flag;
            Shell.ExecuteResult result;
            string[] append = new string[] { "AppxManifest.xml" };
            NPath path = new NPath(Path.GetDirectoryName(executable)).Combine(append);
            if (!File.Exists(executable))
            {
                throw new ArgumentException(string.Format("Specified executable (\"{0}\") does not exist!", executable));
            }
            if (!path.Exists(""))
            {
                throw new ArgumentException(string.Format("AppX manifest was not found next to the executable at \"{0}\"!", path));
            }
            WinRTManifest.AddActivatableClasses(path);
            using (Mutex mutex = new Mutex(true, @"Global\WinRTRunnerBuild", out flag))
            {
                if (!flag)
                {
                    mutex.WaitOne();
                }
                try
                {
                    MakeSureRunnerIsBuilt();
                    Shell.ExecuteArgs executeArgs = new Shell.ExecuteArgs {
                        Executable = WinRTRunnerExecutablePath.ToString(),
                        Arguments = path.InQuotes(),
                        WorkingDirectory = WinRTRunnerExecutablePath.Parent.ToString()
                    };
                    using (TinyProfiler.Section("Run WinRT Application", ""))
                    {
                        result = Shell.Execute(executeArgs, null);
                    }
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
            return result;
        }

        private static string MSBuildPath
        {
            get
            {
                string str;
                string msBuildPathAtVersion = GetMsBuildPathAtVersion("14.0");
                if (msBuildPathAtVersion != null)
                {
                    str = msBuildPathAtVersion;
                }
                else
                {
                    str = GetMsBuildPathAtVersion("12.0");
                }
                if (str == null)
                {
                    throw new Exception("Visual Studio 2013 or Visual Studio 2015 must be installed!");
                }
                return str;
            }
        }

        private static NPath WinRTRunnerExecutablePath
        {
            get
            {
                string[] append = new string[] { "build", "Unity.IL2CPP.Running.WinRT.exe" };
                return CommonPaths.Il2CppRoot.Combine(append);
            }
        }

        private static NPath WinRTRunnerProjectFolderPath
        {
            get
            {
                string[] append = new string[] { "Unity.IL2CPP.Running.WinRTRunner" };
                return CommonPaths.Il2CppRoot.Combine(append);
            }
        }

        private static NPath WinRTRunnerSolutionPath
        {
            get
            {
                string[] append = new string[] { "Unity.IL2CPP.Running.WinRTRunner.sln" };
                return WinRTRunnerProjectFolderPath.Combine(append);
            }
        }
    }
}

