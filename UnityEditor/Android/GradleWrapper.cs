namespace UnityEditor.Android
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using UnityEditor;
    using UnityEditor.Utils;

    internal class GradleWrapper
    {
        public static string Run(string workingdir, string task, [Optional, DefaultParameterValue(null)] Progress progress)
        {
            string[] strArray = AndroidFileLocator.Find(Path.Combine(Path.Combine(Path.Combine(BuildPipeline.GetBuildToolsDirectory(BuildTarget.Android), "gradle"), "lib"), "gradle-launcher-*.jar"));
            if (strArray.Length != 1)
            {
                throw new Exception("Gradle install not valid");
            }
            string str3 = strArray[0];
            return RunJava(string.Format("-classpath \"{0}\" org.gradle.launcher.GradleMain \"{1}\"", str3, task), workingdir, progress);
        }

        private static string RunJava(string args, string workingdir, [Optional, DefaultParameterValue(null)] Progress progress)
        {
            <RunJava>c__AnonStorey0 storey = new <RunJava>c__AnonStorey0 {
                progress = progress
            };
            ProcessStartInfo psi = new ProcessStartInfo {
                FileName = AndroidJavaTools.javaPath,
                Arguments = args,
                WorkingDirectory = workingdir,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8
            };
            storey.lastLine = "";
            return Command.Run(psi, new Command.WaitingForProcessToExit(storey.<>m__0), "Gradle build failed. ");
        }

        [CompilerGenerated]
        private sealed class <RunJava>c__AnonStorey0
        {
            internal string lastLine;
            internal GradleWrapper.Progress progress;

            internal void <>m__0(Program program)
            {
                if (this.progress != null)
                {
                    string[] standardOutput = program.GetStandardOutput();
                    if ((standardOutput.Length > 0) && (standardOutput[standardOutput.Length - 1] != this.lastLine))
                    {
                        this.lastLine = standardOutput[standardOutput.Length - 1];
                        this.progress(this.lastLine);
                    }
                }
            }
        }

        public delegate void Progress(string task);
    }
}

