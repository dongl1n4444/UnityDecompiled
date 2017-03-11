namespace UnityEditor
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using UnityEditor.Utils;
    using UnityEngine;

    internal class MonoProcessUtility
    {
        public static Process PrepareMonoProcess(string workDir)
        {
            Process process = new Process();
            string str = (Application.platform != RuntimePlatform.WindowsEditor) ? "mono" : "mono.exe";
            string[] components = new string[] { MonoInstallationFinder.GetMonoInstallation(), "bin", str };
            process.StartInfo.FileName = Paths.Combine(components);
            process.StartInfo.EnvironmentVariables["_WAPI_PROCESS_HANDLE_OFFSET"] = "5";
            string profile = BuildPipeline.CompatibilityProfileToClassLibFolder(ApiCompatibilityLevel.NET_2_0);
            process.StartInfo.EnvironmentVariables["MONO_PATH"] = MonoInstallationFinder.GetProfileDirectory(profile);
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WorkingDirectory = workDir;
            return process;
        }

        public static Process PrepareMonoProcessBleedingEdge(string workDir)
        {
            Process process = new Process();
            string str = (Application.platform != RuntimePlatform.WindowsEditor) ? "mono" : "mono.exe";
            string[] components = new string[] { MonoInstallationFinder.GetMonoBleedingEdgeInstallation(), "bin", str };
            process.StartInfo.FileName = Paths.Combine(components);
            process.StartInfo.EnvironmentVariables["_WAPI_PROCESS_HANDLE_OFFSET"] = "5";
            string profile = BuildPipeline.CompatibilityProfileToClassLibFolder(ApiCompatibilityLevel.NET_4_6);
            process.StartInfo.EnvironmentVariables["MONO_PATH"] = MonoInstallationFinder.GetProfileDirectory(profile);
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WorkingDirectory = workDir;
            return process;
        }

        public static string ProcessToString(Process process)
        {
            string[] textArray1 = new string[] { process.StartInfo.FileName, " ", process.StartInfo.Arguments, " current dir : ", process.StartInfo.WorkingDirectory, "\n" };
            return string.Concat(textArray1);
        }

        public static void RunMonoProcess(Process process, string name, string resultingFile)
        {
            MonoProcessRunner runner = new MonoProcessRunner();
            bool flag = runner.Run(process);
            if ((process.ExitCode != 0) || !File.Exists(resultingFile))
            {
                string str2 = string.Concat(new object[] { "Failed ", name, ": ", ProcessToString(process), " result file exists: ", File.Exists(resultingFile), ". Timed out: ", !flag }) + "\n\n";
                object[] objArray2 = new object[] { str2, "stdout:\n", runner.Output, "\n" };
                str2 = string.Concat(objArray2);
                object[] objArray3 = new object[] { str2, "stderr:\n", runner.Error, "\n" };
                string str = string.Concat(objArray3);
                Console.WriteLine(str);
                throw new UnityException(str);
            }
        }
    }
}

