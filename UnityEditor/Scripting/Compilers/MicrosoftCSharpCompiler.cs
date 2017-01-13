namespace UnityEditor.Scripting.Compilers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.Scripting;
    using UnityEditor.Utils;
    using UnityEditorInternal;
    using UnityEngine;

    internal class MicrosoftCSharpCompiler : ScriptCompilerBase
    {
        private static string[] _uwpReferences;

        public MicrosoftCSharpCompiler(MonoIsland island, bool runUpdater) : base(island)
        {
        }

        protected override CompilerOutputParserBase CreateOutputParser() => 
            new MicrosoftCSharpCompilerOutputParser();

        private void FillNETCoreCompilerOptions(WSASDK wsaSDK, List<string> arguments, ref string argsPrefix)
        {
            string str;
            argsPrefix = "/noconfig ";
            arguments.Add("/nostdlib+");
            if (GetCurrentScriptingBackend() != ScriptingImplementation.IL2CPP)
            {
                arguments.Add("/define:NETFX_CORE");
            }
            arguments.Add("/preferreduilang:en-US");
            string platformAssemblyPath = GetPlatformAssemblyPath(wsaSDK);
            switch (wsaSDK)
            {
                case WSASDK.SDK80:
                    str = "8.0";
                    break;

                case WSASDK.SDK81:
                    str = "8.1";
                    break;

                case WSASDK.PhoneSDK81:
                    str = "Phone 8.1";
                    break;

                case WSASDK.UWP:
                    str = "UAP";
                    if (GetCurrentScriptingBackend() != ScriptingImplementation.IL2CPP)
                    {
                        arguments.Add("/define:WINDOWS_UWP");
                    }
                    break;

                default:
                    throw new Exception("Unknown Windows SDK: " + EditorUserBuildSettings.wsaSDK.ToString());
            }
            if (!File.Exists(platformAssemblyPath))
            {
                throw new Exception($"'{platformAssemblyPath}' not found, do you have Windows {str} SDK installed?");
            }
            arguments.Add("/reference:\"" + platformAssemblyPath + "\"");
            string[] additionalReferences = GetAdditionalReferences(wsaSDK);
            if (additionalReferences != null)
            {
                foreach (string str3 in additionalReferences)
                {
                    arguments.Add("/reference:\"" + str3 + "\"");
                }
            }
            foreach (string str4 in this.GetNETWSAAssemblies(wsaSDK))
            {
                arguments.Add("/reference:\"" + str4 + "\"");
            }
            if (GetCurrentScriptingBackend() != ScriptingImplementation.IL2CPP)
            {
                string str5;
                string netWSAAssemblyInfoUWP;
                string str7;
                switch (wsaSDK)
                {
                    case WSASDK.SDK80:
                        str5 = Path.Combine(Path.GetTempPath(), ".NETCore,Version=v4.5.AssemblyAttributes.cs");
                        netWSAAssemblyInfoUWP = this.GetNetWSAAssemblyInfoWindows80();
                        str7 = @"Managed\WinRTLegacy.dll";
                        break;

                    case WSASDK.SDK81:
                        str5 = Path.Combine(Path.GetTempPath(), ".NETCore,Version=v4.5.1.AssemblyAttributes.cs");
                        netWSAAssemblyInfoUWP = this.GetNetWSAAssemblyInfoWindows81();
                        str7 = @"Managed\WinRTLegacy.dll";
                        break;

                    case WSASDK.PhoneSDK81:
                        str5 = Path.Combine(Path.GetTempPath(), "WindowsPhoneApp,Version=v8.1.AssemblyAttributes.cs");
                        netWSAAssemblyInfoUWP = this.GetNetWSAAssemblyInfoWindowsPhone81();
                        str7 = @"Managed\Phone\WinRTLegacy.dll";
                        break;

                    case WSASDK.UWP:
                        str5 = Path.Combine(Path.GetTempPath(), ".NETCore,Version=v5.0.AssemblyAttributes.cs");
                        netWSAAssemblyInfoUWP = this.GetNetWSAAssemblyInfoUWP();
                        str7 = @"Managed\UAP\WinRTLegacy.dll";
                        break;

                    default:
                        throw new Exception("Unknown Windows SDK: " + EditorUserBuildSettings.wsaSDK.ToString());
                }
                str7 = Path.Combine(BuildPipeline.GetPlaybackEngineDirectory(this._island._target, BuildOptions.CompressTextures), str7);
                arguments.Add("/reference:\"" + str7.Replace('/', '\\') + "\"");
                if (File.Exists(str5))
                {
                    File.Delete(str5);
                }
                File.WriteAllText(str5, netWSAAssemblyInfoUWP);
                arguments.Add(str5);
            }
        }

        internal static string[] GetAdditionalReferences(WSASDK wsaSDK)
        {
            if (wsaSDK != WSASDK.UWP)
            {
                return null;
            }
            if (_uwpReferences == null)
            {
                _uwpReferences = UWPReferences.GetReferences();
            }
            return _uwpReferences;
        }

        private static ScriptingImplementation GetCurrentScriptingBackend() => 
            PlayerSettings.GetScriptingBackend(BuildTargetGroup.WSA);

        internal static string GetNETCoreFrameworkReferencesDirectory(WSASDK wsaSDK)
        {
            if (GetCurrentScriptingBackend() == ScriptingImplementation.IL2CPP)
            {
                return BuildPipeline.GetMonoLibDirectory(BuildTarget.WSAPlayer);
            }
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

        private string[] GetNETWSAAssemblies(WSASDK wsaSDK)
        {
            if (GetCurrentScriptingBackend() == ScriptingImplementation.IL2CPP)
            {
                <GetNETWSAAssemblies>c__AnonStorey0 storey = new <GetNETWSAAssemblies>c__AnonStorey0 {
                    monoAssemblyDirectory = BuildPipeline.GetMonoLibDirectory(BuildTarget.WSAPlayer)
                };
                return Enumerable.Select<string, string>(GetReferencesFromMonoDistribution(), new Func<string, string>(storey.<>m__0)).ToArray<string>();
            }
            if (wsaSDK != WSASDK.UWP)
            {
                return Directory.GetFiles(GetNETCoreFrameworkReferencesDirectory(wsaSDK), "*.dll");
            }
            NuGetPackageResolver resolver = new NuGetPackageResolver {
                ProjectLockFile = @"UWP\project.lock.json"
            };
            return resolver.Resolve();
        }

        private string GetNetWSAAssemblyInfoUWP()
        {
            string[] textArray1 = new string[] { "using System;", "using System.Reflection;", "[assembly: global::System.Runtime.Versioning.TargetFrameworkAttribute(\".NETCore,Version=v5.0\", FrameworkDisplayName = \".NET for Windows Universal\")]" };
            return string.Join("\r\n", textArray1);
        }

        private string GetNetWSAAssemblyInfoWindows80()
        {
            string[] textArray1 = new string[] { "using System;", " using System.Reflection;", "[assembly: global::System.Runtime.Versioning.TargetFrameworkAttribute(\".NETCore,Version=v4.5\", FrameworkDisplayName = \".NET for Windows Store apps\")]" };
            return string.Join("\r\n", textArray1);
        }

        private string GetNetWSAAssemblyInfoWindows81()
        {
            string[] textArray1 = new string[] { "using System;", " using System.Reflection;", "[assembly: global::System.Runtime.Versioning.TargetFrameworkAttribute(\".NETCore,Version=v4.5.1\", FrameworkDisplayName = \".NET for Windows Store apps (Windows 8.1)\")]" };
            return string.Join("\r\n", textArray1);
        }

        private string GetNetWSAAssemblyInfoWindowsPhone81()
        {
            string[] textArray1 = new string[] { "using System;", " using System.Reflection;", "[assembly: global::System.Runtime.Versioning.TargetFrameworkAttribute(\"WindowsPhoneApp,Version=v8.1\", FrameworkDisplayName = \"Windows Phone 8.1\")]" };
            return string.Join("\r\n", textArray1);
        }

        internal static string GetPlatformAssemblyPath(WSASDK wsaSDK)
        {
            string windowsKitDirectory = GetWindowsKitDirectory(wsaSDK);
            if (wsaSDK == WSASDK.UWP)
            {
                return Path.Combine(windowsKitDirectory, @"UnionMetadata\Facade\Windows.winmd");
            }
            return Path.Combine(windowsKitDirectory, @"References\CommonConfiguration\Neutral\Windows.winmd");
        }

        private static string[] GetReferencesFromMonoDistribution() => 
            new string[] { "mscorlib.dll", "System.dll", "System.Core.dll", "System.Runtime.Serialization.dll", "System.Xml.dll", "System.Xml.Linq.dll", "UnityScript.dll", "UnityScript.Lang.dll", "Boo.Lang.dll" };

        protected override string[] GetStreamContainingCompilerMessages() => 
            base.GetStandardOutput();

        internal static string GetWindowsKitDirectory(WSASDK wsaSDK)
        {
            string str;
            string str2;
            switch (wsaSDK)
            {
                case WSASDK.SDK80:
                    str2 = RegistryUtil.GetRegistryStringValue32(@"SOFTWARE\Microsoft\Microsoft SDKs\Windows\v8.0", "InstallationFolder", null);
                    str = @"Windows Kits\8.0";
                    break;

                case WSASDK.SDK81:
                    str2 = RegistryUtil.GetRegistryStringValue32(@"SOFTWARE\Microsoft\Microsoft SDKs\Windows\v8.1", "InstallationFolder", null);
                    str = @"Windows Kits\8.1";
                    break;

                case WSASDK.PhoneSDK81:
                    str2 = RegistryUtil.GetRegistryStringValue32(@"SOFTWARE\Microsoft\Microsoft SDKs\WindowsPhoneApp\v8.1", "InstallationFolder", null);
                    str = @"Windows Phone Kits\8.1";
                    break;

                case WSASDK.UWP:
                    str2 = RegistryUtil.GetRegistryStringValue32(@"SOFTWARE\Microsoft\Microsoft SDKs\Windows\v10.0", "InstallationFolder", null);
                    str = @"Windows Kits\10.0";
                    break;

                default:
                    throw new Exception("Unknown Windows SDK: " + wsaSDK.ToString());
            }
            if (str2 == null)
            {
                str2 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), str);
            }
            return str2;
        }

        protected override Program StartCompiler()
        {
            string str = ScriptCompilerBase.PrepareFileName(this._island._output);
            List<string> arguments = new List<string> {
                "/target:library",
                "/nowarn:0169",
                "/out:" + str
            };
            string[] collection = new string[] { "/debug:pdbonly", "/optimize+" };
            arguments.InsertRange(0, collection);
            string argsPrefix = "";
            if (base.CompilingForWSA() && ((PlayerSettings.WSA.compilationOverrides == PlayerSettings.WSACompilationOverrides.UseNetCore) || (PlayerSettings.WSA.compilationOverrides == PlayerSettings.WSACompilationOverrides.UseNetCorePartially)))
            {
                this.FillNETCoreCompilerOptions(EditorUserBuildSettings.wsaSDK, arguments, ref argsPrefix);
            }
            return this.StartCompilerImpl(arguments, argsPrefix, EditorUserBuildSettings.wsaSDK == WSASDK.UWP);
        }

        private Program StartCompilerImpl(List<string> arguments, string argsPrefix, bool msBuildCompiler)
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

        internal static string ProgramFilesDirectory
        {
            get
            {
                string environmentVariable = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
                if (Directory.Exists(environmentVariable))
                {
                    return environmentVariable;
                }
                Debug.Log("Env variables ProgramFiles(x86) & ProgramFiles didn't exist, trying hard coded paths");
                string fullPath = Path.GetFullPath(WindowsDirectory + @"\..\..");
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

        internal static string WindowsDirectory =>
            Environment.GetEnvironmentVariable("windir");

        [CompilerGenerated]
        private sealed class <GetNETWSAAssemblies>c__AnonStorey0
        {
            internal string monoAssemblyDirectory;

            internal string <>m__0(string dll) => 
                Path.Combine(this.monoAssemblyDirectory, dll);
        }
    }
}

