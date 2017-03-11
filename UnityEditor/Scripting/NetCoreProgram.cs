namespace UnityEditor.Scripting
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using UnityEditor.Scripting.Compilers;
    using UnityEditor.Utils;
    using UnityEngine;

    internal class NetCoreProgram : Program
    {
        public NetCoreProgram(string executable, string arguments, Action<ProcessStartInfo> setupStartInfo)
        {
            string[] components = new string[] { GetSdkRoot(), "dotnet" };
            string str = Paths.Combine(components);
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                str = CommandLineFormatter.PrepareFileName(str + ".exe");
            }
            ProcessStartInfo info = new ProcessStartInfo {
                Arguments = CommandLineFormatter.PrepareFileName(executable) + " " + arguments,
                CreateNoWindow = true,
                FileName = str,
                WorkingDirectory = Application.dataPath + "/.."
            };
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                string str2 = Path.Combine(Path.Combine(Path.Combine(GetNetCoreRoot(), "NativeDeps"), "osx"), "lib");
                info.EnvironmentVariables.Add("DYLD_LIBRARY_PATH", str2);
            }
            if (setupStartInfo != null)
            {
                setupStartInfo(info);
            }
            base._process.StartInfo = info;
        }

        private static string GetNetCoreRoot() => 
            Path.Combine(MonoInstallationFinder.GetFrameWorksFolder(), "NetCore");

        private static string GetSdkRoot() => 
            Path.Combine(GetNetCoreRoot(), "Sdk");
    }
}

