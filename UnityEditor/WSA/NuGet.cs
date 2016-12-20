namespace UnityEditor.WSA
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;
    using System.Threading;
    using Unity;
    using UnityEditor;
    using UnityEditor.Utils;

    internal sealed class NuGet
    {
        private readonly string _toolsDirectory;
        public const string LockFile = @"UWP\project.lock.json";
        private const string ProjectFile = @"UWP\project.json";

        private NuGet(BuildTarget target)
        {
            this._toolsDirectory = BuildPipeline.GetBuildToolsDirectory(target);
        }

        private void Cleanup()
        {
            File.Delete(@"UWP\project.lock.json");
            File.Delete(@"UWP\project.json");
        }

        private bool Restore(out string result)
        {
            if (this.Validate())
            {
                Console.WriteLine("NuGet packages will not be restored. Reusing '{0}'.", Path.GetFullPath(@"UWP\project.lock.json"));
                result = null;
                return true;
            }
            if (!Directory.Exists("UWP"))
            {
                Directory.CreateDirectory("UWP");
            }
            this.Cleanup();
            File.Copy(Utility.CombinePath(this._toolsDirectory, "project.json"), @"UWP\project.json", true);
            Console.WriteLine("Restoring NuGet packages from '{0}'.", Path.GetFullPath(@"UWP\project.json"));
            string fileName = Utility.CombinePath(this._toolsDirectory, "nuget.exe");
            string arguments = "restore \"UWP\\project.json\" -NonInteractive";
            ProcessStartInfo si = new ProcessStartInfo(fileName, arguments) {
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            using (Program program = new Program(si))
            {
                program.Start();
                while (true)
                {
                    if (EditorUtility.DisplayCancelableProgressBar("Building Player", "Restoring NuGet packages", 0.5f))
                    {
                        Console.WriteLine("NuGet package restore was cancelled.");
                        program.Kill();
                        this.Cleanup();
                        throw new OperationCanceledException("Build was cancelled while restoring NuGet packages");
                    }
                    if (program.HasExited)
                    {
                        break;
                    }
                    Thread.Sleep(1);
                }
                if (program.ExitCode != 0)
                {
                    string allOutput = program.GetAllOutput();
                    Console.WriteLine("Failed to restore NuGet packages:" + Environment.NewLine + allOutput);
                    Regex regex = new Regex("Could not find file '(.*).sha512'");
                    string errorOutputAsString = program.GetErrorOutputAsString();
                    IEnumerator enumerator = regex.Matches(errorOutputAsString).GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            Match current = (Match) enumerator.Current;
                            string str6 = current.Groups[1].Value;
                            Console.WriteLine("Deleting '{0}' because no corresponding sha512 file was found.", str6);
                            File.Delete(str6);
                        }
                    }
                    finally
                    {
                        IDisposable disposable = enumerator as IDisposable;
                        if (disposable != null)
                        {
                            disposable.Dispose();
                        }
                    }
                    this.Cleanup();
                    result = "Failed to restore NuGet packages:" + Environment.NewLine + allOutput;
                    return false;
                }
            }
            if (!this.Validate())
            {
                Console.WriteLine("Project lock file validation failed.");
                this.Cleanup();
                result = "Failed to restore NuGet packages.";
                return true;
            }
            Console.WriteLine("NuGet packages successfully restored.");
            result = null;
            return true;
        }

        public static string Restore(BuildTarget target)
        {
            string str;
            NuGet get = new NuGet(target);
            if (!get.Restore(out str))
            {
                Console.WriteLine("Retrying NuGet package restore.");
                get.Restore(out str);
            }
            return str;
        }

        private bool Validate()
        {
            if (!File.Exists(@"UWP\project.lock.json"))
            {
                return false;
            }
            try
            {
                NuGetAssemblyResolver resolver = new NuGetAssemblyResolver(@"UWP\project.lock.json");
                return (resolver.Resolve("mscorlib") != null);
            }
            catch
            {
                return false;
            }
        }
    }
}

