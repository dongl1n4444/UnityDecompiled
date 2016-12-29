namespace UnityEditor
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using UnityEngine;

    internal class MonoProcessUtility
    {
        public static string GetMonoExec(BuildTarget buildTarget)
        {
            string monoBinDirectory = BuildPipeline.GetMonoBinDirectory(buildTarget);
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                return Path.Combine(monoBinDirectory, "mono.exe");
            }
            return Path.Combine(monoBinDirectory, "mono");
        }

        private static BuildTarget GetMonoExecTarget(BuildTarget buildTarget)
        {
            BuildTarget target = buildTarget;
            switch (buildTarget)
            {
                case BuildTarget.PSP2:
                case BuildTarget.PS4:
                case BuildTarget.XboxOne:
                case BuildTarget.WiiU:
                    return BuildTarget.StandaloneWindows64;

                case BuildTarget.PSM:
                case BuildTarget.SamsungTV:
                case BuildTarget.N3DS:
                    return target;
            }
            return target;
        }

        public static string GetMonoPath(BuildTarget buildTarget) => 
            (BuildPipeline.GetMonoLibDirectory(buildTarget) + Path.PathSeparator + ".");

        public static Process PrepareMonoProcess(BuildTarget target, string workDir)
        {
            BuildTarget monoExecTarget = GetMonoExecTarget(target);
            return new Process { StartInfo = { 
                FileName = GetMonoExec(monoExecTarget),
                EnvironmentVariables = { 
                    ["_WAPI_PROCESS_HANDLE_OFFSET"] = "5",
                    ["MONO_PATH"] = GetMonoPath(monoExecTarget)
                },
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = workDir
            } };
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

