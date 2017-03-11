namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using UnityEditor.Scripting;
    using UnityEditor.Scripting.Compilers;
    using UnityEditor.Utils;
    using UnityEngine;

    internal class Runner
    {
        internal static void RunManagedProgram(string exe, string args)
        {
            RunManagedProgram(exe, args, Application.dataPath + "/..", null, null);
        }

        internal static void RunManagedProgram(string exe, string args, string workingDirectory, CompilerOutputParserBase parser, Action<ProcessStartInfo> setupStartInfo)
        {
            Program program;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                ProcessStartInfo info = new ProcessStartInfo {
                    Arguments = args,
                    CreateNoWindow = true,
                    FileName = exe
                };
                if (setupStartInfo != null)
                {
                    setupStartInfo(info);
                }
                program = new Program(info);
            }
            else
            {
                program = new ManagedProgram(MonoInstallationFinder.GetMonoInstallation("MonoBleedingEdge"), null, exe, args, false, setupStartInfo);
            }
            using (program)
            {
                program.GetProcessStartInfo().WorkingDirectory = workingDirectory;
                program.Start();
                program.WaitForExit();
                stopwatch.Stop();
                Console.WriteLine("{0} exited after {1} ms.", exe, stopwatch.ElapsedMilliseconds);
                if (program.ExitCode != 0)
                {
                    if (parser != null)
                    {
                        string[] errorOutput = program.GetErrorOutput();
                        string[] standardOutput = program.GetStandardOutput();
                        IEnumerable<CompilerMessage> enumerable = parser.Parse(errorOutput, standardOutput, true);
                        foreach (CompilerMessage message in enumerable)
                        {
                            UnityEngine.Debug.LogPlayerBuildError(message.message, message.file, message.line, message.column);
                        }
                    }
                    UnityEngine.Debug.LogError("Failed running " + exe + " " + args + "\n\n" + program.GetAllOutput());
                    throw new Exception($"{exe} did not run properly!");
                }
            }
        }

        public static void RunNativeProgram(string exe, string args)
        {
            using (NativeProgram program = new NativeProgram(exe, args))
            {
                program.Start();
                program.WaitForExit();
                if (program.ExitCode != 0)
                {
                    UnityEngine.Debug.LogError("Failed running " + exe + " " + args + "\n\n" + program.GetAllOutput());
                    throw new Exception($"{exe} did not run properly!");
                }
            }
        }
    }
}

