namespace UnityEditor.Scripting.Compilers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEditor.Modules;
    using UnityEditor.Scripting;
    using UnityEditor.Utils;
    using UnityEngine;

    internal class MicrosoftCSharpCompiler : ScriptCompilerBase
    {
        [CompilerGenerated]
        private static Func<string, string> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<string, string> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<string, string> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<string, string> <>f__am$cache3;

        public MicrosoftCSharpCompiler(MonoIsland island, bool runUpdater) : base(island)
        {
        }

        protected override CompilerOutputParserBase CreateOutputParser() => 
            new MicrosoftCSharpCompilerOutputParser();

        private void FillCompilerOptions(List<string> arguments, out string argsPrefix)
        {
            argsPrefix = "/noconfig ";
            arguments.Add("/nostdlib+");
            arguments.Add("/preferreduilang:en-US");
            ICompilationExtension extension = ModuleManager.FindPlatformSupportModule(ModuleManager.GetTargetStringFromBuildTarget(this.BuildTarget)).CreateCompilationExtension();
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = r => "/reference:\"" + r + "\"";
            }
            arguments.AddRange(Enumerable.Select<string, string>(this.GetClassLibraries(), <>f__am$cache0));
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = r => "/reference:\"" + r + "\"";
            }
            arguments.AddRange(Enumerable.Select<string, string>(extension.GetAdditionalAssemblyReferences(), <>f__am$cache1));
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = r => "/reference:\"" + r + "\"";
            }
            arguments.AddRange(Enumerable.Select<string, string>(extension.GetWindowsMetadataReferences(), <>f__am$cache2));
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = d => "/define:" + d;
            }
            arguments.AddRange(Enumerable.Select<string, string>(extension.GetAdditionalDefines(), <>f__am$cache3));
            arguments.AddRange(extension.GetAdditionalSourceFiles());
        }

        private string[] GetClassLibraries()
        {
            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(this.BuildTarget);
            if (PlayerSettings.GetScriptingBackend(buildTargetGroup) != ScriptingImplementation.WinRTDotNET)
            {
                <GetClassLibraries>c__AnonStorey0 storey = new <GetClassLibraries>c__AnonStorey0 {
                    monoAssemblyDirectory = base.GetMonoProfileLibDirectory()
                };
                List<string> list = new List<string>();
                list.AddRange(Enumerable.Select<string, string>(GetReferencesFromMonoDistribution(), new Func<string, string>(storey.<>m__0)));
                if (PlayerSettings.GetApiCompatibilityLevel(buildTargetGroup) == ApiCompatibilityLevel.NET_4_6)
                {
                    string str = Path.Combine(storey.monoAssemblyDirectory, "Facades");
                    list.Add(Path.Combine(str, "System.ObjectModel.dll"));
                    list.Add(Path.Combine(str, "System.Runtime.dll"));
                    list.Add(Path.Combine(str, "System.Runtime.InteropServices.WindowsRuntime.dll"));
                }
                return list.ToArray();
            }
            if (this.BuildTarget != UnityEditor.BuildTarget.WSAPlayer)
            {
                throw new InvalidOperationException($"MicrosoftCSharpCompiler cannot build for .NET Scripting backend for BuildTarget.{this.BuildTarget}.");
            }
            WSASDK wsaSDK = EditorUserBuildSettings.wsaSDK;
            if (wsaSDK != WSASDK.UWP)
            {
                return Directory.GetFiles(GetNETCoreFrameworkReferencesDirectory(wsaSDK), "*.dll");
            }
            NuGetPackageResolver resolver = new NuGetPackageResolver {
                ProjectLockFile = @"UWP\project.lock.json"
            };
            return resolver.Resolve();
        }

        internal static string GetNETCoreFrameworkReferencesDirectory(WSASDK wsaSDK)
        {
            switch (wsaSDK)
            {
                case WSASDK.SDK80:
                    return (ProgramFilesDirectory + @"\Reference Assemblies\Microsoft\Framework\.NETCore\v4.5");

                case WSASDK.SDK81:
                    return (ProgramFilesDirectory + @"\Reference Assemblies\Microsoft\Framework\.NETCore\v4.5.1");

                case WSASDK.PhoneSDK81:
                    return (ProgramFilesDirectory + @"\Reference Assemblies\Microsoft\Framework\WindowsPhoneApp\v8.1");

                case WSASDK.UWP:
                    return null;
            }
            throw new Exception("Unknown Windows SDK: " + wsaSDK.ToString());
        }

        private static string[] GetReferencesFromMonoDistribution() => 
            new string[] { "mscorlib.dll", "System.dll", "System.Core.dll", "System.Runtime.Serialization.dll", "System.Xml.dll", "System.Xml.Linq.dll", "UnityScript.dll", "UnityScript.Lang.dll", "Boo.Lang.dll" };

        protected override string[] GetStreamContainingCompilerMessages() => 
            base.GetStandardOutput();

        protected override Program StartCompiler()
        {
            string str2;
            string str = ScriptCompilerBase.PrepareFileName(this._island._output);
            List<string> arguments = new List<string> {
                "/debug:pdbonly",
                "/optimize+",
                "/target:library",
                "/nowarn:0169",
                "/unsafe",
                "/out:" + str
            };
            this.FillCompilerOptions(arguments, out str2);
            return this.StartCompilerImpl(arguments, str2);
        }

        private Program StartCompilerImpl(List<string> arguments, string argsPrefix)
        {
            foreach (string str in this._island._references)
            {
                arguments.Add("/reference:" + ScriptCompilerBase.PrepareFileName(str));
            }
            foreach (string str2 in this._island._defines.Distinct<string>())
            {
                arguments.Add("/define:" + str2);
            }
            foreach (string str3 in this._island._files)
            {
                arguments.Add(ScriptCompilerBase.PrepareFileName(str3).Replace('/', '\\'));
            }
            string[] components = new string[] { EditorApplication.applicationContentsPath, "Tools", "Roslyn", "CoreRun.exe" };
            string path = Paths.Combine(components).Replace('/', '\\');
            string[] textArray2 = new string[] { EditorApplication.applicationContentsPath, "Tools", "Roslyn", "csc.exe" };
            string str5 = Paths.Combine(textArray2).Replace('/', '\\');
            if (!File.Exists(path))
            {
                ThrowCompilerNotFoundException(path);
            }
            if (!File.Exists(str5))
            {
                ThrowCompilerNotFoundException(str5);
            }
            base.AddCustomResponseFileIfPresent(arguments, "csc.rsp");
            string str6 = CommandLineFormatter.GenerateResponseFile(arguments);
            ProcessStartInfo info2 = new ProcessStartInfo();
            string[] textArray3 = new string[] { "\"", str5, "\" ", argsPrefix, "@", str6 };
            info2.Arguments = string.Concat(textArray3);
            info2.FileName = path;
            info2.CreateNoWindow = true;
            ProcessStartInfo si = info2;
            Program program = new Program(si);
            program.Start();
            return program;
        }

        private static void ThrowCompilerNotFoundException(string path)
        {
            throw new Exception($"'{path}' not found. Is your Unity installation corrupted?");
        }

        private UnityEditor.BuildTarget BuildTarget =>
            this._island._target;

        internal static string ProgramFilesDirectory
        {
            get
            {
                string environmentVariable = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
                if (Directory.Exists(environmentVariable))
                {
                    return environmentVariable;
                }
                UnityEngine.Debug.Log("Env variables ProgramFiles(x86) & ProgramFiles didn't exist, trying hard coded paths");
                string fullPath = Path.GetFullPath(Environment.GetEnvironmentVariable("windir") + @"\..\..");
                string path = fullPath + "Program Files (x86)";
                string str5 = fullPath + "Program Files";
                if (Directory.Exists(path))
                {
                    return path;
                }
                if (Directory.Exists(str5))
                {
                    return str5;
                }
                string[] textArray1 = new string[] { "Path '", path, "' or '", str5, "' doesn't exist." };
                throw new Exception(string.Concat(textArray1));
            }
        }

        [CompilerGenerated]
        private sealed class <GetClassLibraries>c__AnonStorey0
        {
            internal string monoAssemblyDirectory;

            internal string <>m__0(string dll) => 
                Path.Combine(this.monoAssemblyDirectory, dll);
        }
    }
}

