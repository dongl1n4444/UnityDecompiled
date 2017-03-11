namespace UnityEditor.WebGL
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;
    using UnityEngine;

    internal static class ProgramUtils
    {
        internal static bool StartProgramChecked(ProcessStartInfo p)
        {
            <StartProgramChecked>c__AnonStorey0 storey = new <StartProgramChecked>c__AnonStorey0();
            Console.WriteLine("Filename: " + p.FileName);
            Console.WriteLine("Arguments: " + p.Arguments);
            int index = p.Arguments.IndexOf("Temp/UnityTempFile");
            Console.WriteLine("index: " + index);
            if (index > 0)
            {
                string path = p.Arguments.Substring(index);
                Console.WriteLine("Responsefile: " + path + " Contents: ");
                Console.WriteLine(File.ReadAllText(path));
            }
            storey.stdout = new StringBuilder();
            storey.stderr = new StringBuilder();
            p.RedirectStandardOutput = true;
            p.RedirectStandardError = true;
            p.UseShellExecute = false;
            using (Process process = Process.Start(p))
            {
                process.OutputDataReceived += new DataReceivedEventHandler(storey.<>m__0);
                process.ErrorDataReceived += new DataReceivedEventHandler(storey.<>m__1);
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    UnityEngine.Debug.LogError(string.Concat(new object[] { "Failed running ", p.FileName, " ", p.Arguments, "\n\nstdout:", storey.stdout, "\nstderr:", storey.stderr }));
                    throw new Exception("Failed building WebGL Player.");
                }
            }
            return true;
        }

        [CompilerGenerated]
        private sealed class <StartProgramChecked>c__AnonStorey0
        {
            internal StringBuilder stderr;
            internal StringBuilder stdout;

            internal void <>m__0(object sender, DataReceivedEventArgs e)
            {
                this.stdout.Append(e.Data);
            }

            internal void <>m__1(object sender, DataReceivedEventArgs e)
            {
                this.stderr.Append(e.Data);
            }
        }
    }
}

