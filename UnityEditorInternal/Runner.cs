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
            RunProgram(program, exe, args, workingDirectory, parser);
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

        internal static void RunNetCoreProgram(string exe, string args, string workingDirectory, CompilerOutputParserBase parser, Action<ProcessStartInfo> setupStartInfo)
        {
            NetCoreProgram p = new NetCoreProgram(exe, args, setupStartInfo);
            RunProgram(p, exe, args, workingDirectory, parser);
        }

        private static void RunProgram(Program p, string exe, string args, string workingDirectory, CompilerOutputParserBase parser)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            using (p)
            {
                p.GetProcessStartInfo().WorkingDirectory = workingDirectory;
                p.Start();
                p.WaitForExit();
                stopwatch.Stop();
                Console.WriteLine("{0} exited after {1} ms.", exe, stopwatch.ElapsedMilliseconds);
                if (p.ExitCode != 0)
                {
                    if (parser != null)
                    {
                        string[] errorOutput = p.GetErrorOutput();
                        string[] standardOutput = p.GetStandardOutput();
                        IEnumerable<CompilerMessage> enumerable = parser.Parse(errorOutput, standardOutput, true);
                        foreach (CompilerMessage message in enumerable)
                        {
                            UnityEngine.Debug.LogPlayerBuildError(message.message, message.file, message.line, message.column);
                        }
                    }
                    UnityEngine.Debug.LogError("Failed running " + exe + " " + args + "\n\n" + p.GetAllOutput());
                    throw new Exception($"{exe} did not run properly!");
                }
            }
        }
    }
}

