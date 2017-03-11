namespace Unity.IL2CPP
{
    using NiceIO;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using Unity.IL2CPP.Portability;

    public static class Shell
    {
        private static readonly object FileLocker = new object();
        private static readonly object PipeLocker = new object();

        public static ExecuteResult Execute(ExecuteArgs executeArgs, IExecuteController controller = null)
        {
            <Execute>c__AnonStorey1 storey = new <Execute>c__AnonStorey1 {
                controller = controller
            };
            using (Process process = NewProcess(executeArgs))
            {
                FileStream stream;
                FileStream stream2;
                string tempFileName;
                string str2;
                string str3;
                string str4;
                object fileLocker = FileLocker;
                lock (fileLocker)
                {
                    tempFileName = Path.GetTempFileName();
                    str2 = Path.GetTempFileName();
                    stream = File.Create(tempFileName);
                    stream2 = File.Create(str2);
                }
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                <Execute>c__AnonStorey0 storey2 = new <Execute>c__AnonStorey0 {
                    <>f__ref$1 = storey,
                    outputWriter = new System.IO.StreamWriter(stream, Encoding.UTF8)
                };
                try
                {
                    storey2.errorWriter = new System.IO.StreamWriter(stream2, Encoding.UTF8);
                    try
                    {
                        process.OutputDataReceived += new DataReceivedEventHandler(storey2.<>m__0);
                        process.ErrorDataReceived += new DataReceivedEventHandler(storey2.<>m__1);
                        object pipeLocker = PipeLocker;
                        lock (pipeLocker)
                        {
                            process.Start();
                            process.BeginOutputReadLine();
                            process.BeginErrorReadLine();
                        }
                        process.WaitForExit();
                        process.CancelErrorRead();
                        process.CancelOutputRead();
                    }
                    finally
                    {
                        if (storey2.errorWriter != null)
                        {
                            storey2.errorWriter.Dispose();
                        }
                    }
                }
                finally
                {
                    if (storey2.outputWriter != null)
                    {
                        storey2.outputWriter.Dispose();
                    }
                }
                object obj4 = FileLocker;
                lock (obj4)
                {
                    if (storey.controller != null)
                    {
                        storey.controller.AboutToCleanup(tempFileName, str2);
                    }
                    str3 = File.ReadAllText(tempFileName, Encoding.UTF8);
                    File.Delete(tempFileName);
                    str4 = File.ReadAllText(str2, Encoding.UTF8);
                    File.Delete(str2);
                }
                stopwatch.Stop();
                return new ExecuteResult { 
                    ExitCode = process.ExitCode,
                    StdOut = str3,
                    StdErr = str4,
                    Duration = TimeSpan.FromMilliseconds((double) stopwatch.ElapsedMilliseconds)
                };
            }
        }

        public static string ExecuteAndCaptureOutput(ExecuteArgs executeArgs)
        {
            ExecuteResult result = Execute(executeArgs, null);
            string str = result.StdErr.Trim() + result.StdOut.Trim();
            if (result.ExitCode != 0)
            {
                throw new Exception(string.Format("Process {0} ended with exitcode {1}" + Environment.NewLine + "{2}" + Environment.NewLine, executeArgs.Executable, result.ExitCode, str));
            }
            return str;
        }

        public static string ExecuteAndCaptureOutput(NPath filename, string arguments, Dictionary<string, string> envVars = null) => 
            ExecuteAndCaptureOutput(filename.ToString(), arguments, envVars);

        public static string ExecuteAndCaptureOutput(string filename, string arguments, Dictionary<string, string> envVars = null)
        {
            ExecuteArgs executeArgs = new ExecuteArgs {
                Executable = filename,
                Arguments = arguments,
                EnvVars = envVars
            };
            return ExecuteAndCaptureOutput(executeArgs);
        }

        public static string ExecuteAndCaptureOutput(string filename, string arguments, string workingDirectory, Dictionary<string, string> envVars = null)
        {
            ExecuteArgs executeArgs = new ExecuteArgs {
                Executable = filename,
                Arguments = arguments,
                WorkingDirectory = workingDirectory,
                EnvVars = envVars
            };
            return ExecuteAndCaptureOutput(executeArgs);
        }

        public static ExecuteResult ExecuteWithLiveOutput(ExecuteArgs executeArgs) => 
            Execute(executeArgs, LiveOutput.Instance);

        private static Process NewProcess(ExecuteArgs executeArgs)
        {
            Process process = new Process {
                StartInfo = { 
                    Arguments = executeArgs.Arguments,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    RedirectStandardError = true,
                    FileName = executeArgs.Executable,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8,
                    WorkingDirectory = executeArgs.WorkingDirectory
                }
            };
            if (executeArgs.EnvVars != null)
            {
                foreach (KeyValuePair<string, string> pair in executeArgs.EnvVars)
                {
                    process.StartInfo.SetEnvironmentVariablePortable(pair);
                }
            }
            return process;
        }

        public static Process StartProcess(string filename, string arguments)
        {
            ExecuteArgs executeArgs = new ExecuteArgs {
                Executable = filename,
                Arguments = arguments
            };
            Process process = NewProcess(executeArgs);
            process.Start();
            return process;
        }

        [CompilerGenerated]
        private sealed class <Execute>c__AnonStorey0
        {
            internal Shell.<Execute>c__AnonStorey1 <>f__ref$1;
            internal System.IO.StreamWriter errorWriter;
            internal System.IO.StreamWriter outputWriter;

            internal void <>m__0(object sender, DataReceivedEventArgs args)
            {
                this.outputWriter.WriteLine(args.Data);
                if (this.<>f__ref$1.controller != null)
                {
                    this.<>f__ref$1.controller.OnStdoutReceived(args.Data);
                }
            }

            internal void <>m__1(object sender, DataReceivedEventArgs args)
            {
                this.errorWriter.WriteLine(args.Data);
                if (this.<>f__ref$1.controller != null)
                {
                    this.<>f__ref$1.controller.OnStderrReceived(args.Data);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <Execute>c__AnonStorey1
        {
            internal Shell.IExecuteController controller;
        }

        public class ExecuteArgs
        {
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private string <Arguments>k__BackingField;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private Dictionary<string, string> <EnvVars>k__BackingField;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private string <Executable>k__BackingField;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private string <WorkingDirectory>k__BackingField;

            public string Arguments { get; set; }

            public Dictionary<string, string> EnvVars { get; set; }

            public string Executable { get; set; }

            public string WorkingDirectory { get; set; }
        }

        public class ExecuteResult
        {
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private TimeSpan <Duration>k__BackingField;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private int <ExitCode>k__BackingField;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private string <StdErr>k__BackingField;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private string <StdOut>k__BackingField;

            public TimeSpan Duration { get; set; }

            public int ExitCode { get; set; }

            public string StdErr { get; set; }

            public string StdOut { get; set; }
        }

        public interface IExecuteController
        {
            void AboutToCleanup(string tempOutputFile, string tempErrorFile);
            void OnStderrReceived(string data);
            void OnStdoutReceived(string data);
        }

        private class LiveOutput : Shell.IExecuteController
        {
            public static readonly Shell.IExecuteController Instance = new Shell.LiveOutput();

            public void AboutToCleanup(string tempOutputFile, string tempErrorFile)
            {
            }

            public void OnStderrReceived(string data)
            {
                Console.WriteLine(data);
            }

            public void OnStdoutReceived(string data)
            {
                Console.WriteLine(data);
            }
        }
    }
}

