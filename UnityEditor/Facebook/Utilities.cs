namespace UnityEditor.Facebook
{
    using System;
    using System.Diagnostics;
    using System.Text;
    using UnityEditor;
    using UnityEditor.Scripting;
    using UnityEditor.Utils;
    using UnityEngine;

    internal static class Utilities
    {
        internal static Program CreateManagedProgram(string executable, string arguments, Action<ProcessStartInfo> setupStartInfo)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                ProcessStartInfo info = new ProcessStartInfo {
                    FileName = executable,
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                if (setupStartInfo != null)
                {
                    setupStartInfo(info);
                }
                return new Program(info);
            }
            return new ManagedProgram(MonoInstallationFinder.GetMonoInstallation("Mono"), "2.0", executable, arguments, setupStartInfo);
        }

        private static bool Run7Zip(string workingDir, string arguments)
        {
            string str = "7za";
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                str = "7z.exe";
            }
            ProcessStartInfo startInfo = new ProcessStartInfo(EditorApplication.applicationContentsPath + "/Tools/" + str) {
                Arguments = arguments,
                UseShellExecute = false,
                WorkingDirectory = workingDir,
                CreateNoWindow = true
            };
            Process process = Process.Start(startInfo);
            process.WaitForExit();
            return (process.ExitCode == 0);
        }

        internal static bool UnZip(string file, string destFile)
        {
            string[] textArray1 = new string[] { "x -o\"", destFile, "\" \"", file, "\"" };
            return Run7Zip("", string.Concat(textArray1));
        }

        internal static bool Zip(string workingDir, string destFile, string[] paths)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("a -mx9 \"");
            builder.Append(destFile);
            builder.Append("\" ");
            foreach (string str in paths)
            {
                builder.Append("\"" + str + "\" ");
            }
            return Run7Zip(workingDir, builder.ToString());
        }
    }
}

