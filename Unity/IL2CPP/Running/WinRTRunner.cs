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
    using Unity.IL2CPP.Building.ToolChains.MsvcVersions.VisualStudioAPI;
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

        public override IEnumerable<string> GetAdditionalIncludes() => 
            new string[] { "windows.h", "ActivateApp.h" };

        private static string GetMsBuildForVisualStudio2017()
        {
            string str;
            try
            {
                IEnumSetupInstances instances = new SetupConfiguration().EnumInstances();
                object[] rgelt = new object[1];
                while (true)
                {
                    int num;
                    instances.Next(rgelt.Length, rgelt, out num);
                    if (num == 0)
                    {
                        return null;
                    }
                    ISetupInstance2 instance = rgelt[0] as ISetupInstance2;
                    if (instance != null)
                    {
                        try
                        {
                            string[] append = new string[] { "MSBuild", "15.0", "bin", "MSBuild.exe" };
                            NPath path = instance.GetInstallationPath().ToNPath().Combine(append);
                            if (path.FileExists(""))
                            {
                                return path.ToString();
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
            catch
            {
                str = null;
            }
            return str;
        }

        private static string GetMsBuildPathAtVersion(string msvcVersionNumber)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey($"SOFTWARE\Microsoft\MSBuild\ToolsVersions\{msvcVersionNumber}");
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

        public override Shell.ExecuteResult RunAndMakeExecuteResult(Unity.IL2CPP.Common.Architecture architecture, string executable)
        {
            bool flag;
            Shell.ExecuteResult result;
            string[] append = new string[] { "AppxManifest.xml" };
            NPath path = new NPath(Path.GetDirectoryName(executable)).Combine(append);
            if (!File.Exists(executable))
            {
                throw new ArgumentException($"Specified executable ("{executable}") does not exist!");
            }
            if (!path.Exists(""))
            {
                throw new ArgumentException($"AppX manifest was not found next to the executable at "{path}"!");
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
                string text1 = GetMsBuildForVisualStudio2017();
                if (text1 != null)
                {
                    str = text1;
                }
                else
                {
                    string msBuildPathAtVersion = GetMsBuildPathAtVersion("14.0");
                    if (msBuildPathAtVersion != null)
                    {
                        str = msBuildPathAtVersion;
                    }
                    else
                    {
                        str = GetMsBuildPathAtVersion("12.0");
                    }
                }
                if (str == null)
                {
                    throw new Exception("Visual Studio 2013 or Visual Studio 2015 or Visual Studio 2017 must be installed!");
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

