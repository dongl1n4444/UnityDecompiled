namespace UnityEditor.WSA
{
    using Mono.Cecil;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Unity;
    using UnityEditor;
    using UnityEditor.Modules;
    using UnityEditor.Scripting.Compilers;
    using UnityEditorInternal;

    internal class MetroCompilationExtension : DefaultCompilationExtension
    {
        private static string[] _uwpReferences;
        [CompilerGenerated]
        private static Func<CustomAttribute, bool> <>f__am$cache0;
        private static List<string> userScriptAssemblies;

        static MetroCompilationExtension()
        {
            string[] collection = new string[] { "Assembly-CSharp", "Assembly-CSharp-firstpass", "Assembly-UnityScript", "Assembly-UnityScript-firstpass", "Assembly-Boo", "Assembly-Boo-firstpass" };
            userScriptAssemblies = new List<string>(collection);
        }

        public override IEnumerable<string> GetAdditionalAssemblyReferences()
        {
            string str;
            if (PlayerSettings.GetScriptingBackend(BuildTargetGroup.WSA) != ScriptingImplementation.WinRTDotNET)
            {
                return new string[0];
            }
            WSASDK wsaSDK = EditorUserBuildSettings.wsaSDK;
            switch (wsaSDK)
            {
                case WSASDK.SDK80:
                    str = @"Managed\WinRTLegacy.dll";
                    break;

                case WSASDK.SDK81:
                    str = @"Managed\WinRTLegacy.dll";
                    break;

                case WSASDK.PhoneSDK81:
                    str = @"Managed\Phone\WinRTLegacy.dll";
                    break;

                case WSASDK.UWP:
                    str = @"Managed\UAP\WinRTLegacy.dll";
                    break;

                default:
                    throw new Exception("Unknown Windows SDK: " + wsaSDK.ToString());
            }
            str = Path.Combine(BuildPipeline.GetPlaybackEngineDirectory(BuildTarget.WSAPlayer, BuildOptions.CompressTextures), str);
            return new string[] { str.Replace('/', '\\') };
        }

        [DebuggerHidden]
        public override IEnumerable<string> GetAdditionalDefines() => 
            new <GetAdditionalDefines>c__Iterator0 { $PC = -2 };

        public override IEnumerable<string> GetAdditionalSourceFiles()
        {
            string str;
            string netWSAAssemblyInfoUWP;
            if (PlayerSettings.GetScriptingBackend(BuildTargetGroup.WSA) != ScriptingImplementation.WinRTDotNET)
            {
                return new string[0];
            }
            WSASDK wsaSDK = EditorUserBuildSettings.wsaSDK;
            switch (wsaSDK)
            {
                case WSASDK.SDK80:
                    str = Path.Combine(Path.GetTempPath(), ".NETCore,Version=v4.5.AssemblyAttributes.cs");
                    netWSAAssemblyInfoUWP = this.GetNetWSAAssemblyInfoWindows80();
                    break;

                case WSASDK.SDK81:
                    str = Path.Combine(Path.GetTempPath(), ".NETCore,Version=v4.5.1.AssemblyAttributes.cs");
                    netWSAAssemblyInfoUWP = this.GetNetWSAAssemblyInfoWindows81();
                    break;

                case WSASDK.PhoneSDK81:
                    str = Path.Combine(Path.GetTempPath(), "WindowsPhoneApp,Version=v8.1.AssemblyAttributes.cs");
                    netWSAAssemblyInfoUWP = this.GetNetWSAAssemblyInfoWindowsPhone81();
                    break;

                case WSASDK.UWP:
                    str = Path.Combine(Path.GetTempPath(), ".NETCore,Version=v5.0.AssemblyAttributes.cs");
                    netWSAAssemblyInfoUWP = this.GetNetWSAAssemblyInfoUWP();
                    break;

                default:
                    throw new Exception("Unknown Windows SDK: " + wsaSDK.ToString());
            }
            if (File.Exists(str))
            {
                File.Delete(str);
            }
            File.WriteAllText(str, netWSAAssemblyInfoUWP);
            return new string[] { str };
        }

        public override IAssemblyResolver GetAssemblyResolver(bool buildingForEditor, string assemblyPath, string[] searchDirectories)
        {
            if (!buildingForEditor)
            {
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = a => a.AttributeType.FullName == "System.Runtime.Versioning.TargetFrameworkAttribute";
                }
                CustomAttribute attribute = Enumerable.FirstOrDefault<CustomAttribute>(AssemblyDefinition.ReadAssembly(assemblyPath).CustomAttributes, <>f__am$cache0);
                if (attribute != null)
                {
                    CustomAttributeArgument argument = attribute.ConstructorArguments[0];
                    string str = (string) argument.Value;
                    if (str == ".NETCore,Version=v5.0")
                    {
                        NuGetAssemblyResolver resolver = new NuGetAssemblyResolver(@"UWP\project.lock.json");
                        if (searchDirectories != null)
                        {
                            foreach (string str2 in searchDirectories)
                            {
                                resolver.AddSearchDirectory(str2);
                            }
                        }
                        return resolver;
                    }
                }
            }
            return base.GetAssemblyResolver(buildingForEditor, assemblyPath, searchDirectories);
        }

        public override string[] GetCompilerExtraAssemblyPaths(bool isEditor, string assemblyPathName)
        {
            List<string> list = new List<string>();
            list.AddRange(base.GetCompilerExtraAssemblyPaths(isEditor, assemblyPathName));
            bool flag = false;
            if (this.IsUserScriptAssembly(assemblyPathName))
            {
                if (this.GetCsCompiler(isEditor, assemblyPathName) == CSharpCompiler.Microsoft)
                {
                    flag = true;
                }
            }
            else
            {
                flag = !isEditor;
            }
            if (flag)
            {
                WSASDK wsaSDK = EditorUserBuildSettings.wsaSDK;
                bool flag2 = PlayerSettings.GetScriptingBackend(BuildTargetGroup.WSA) == ScriptingImplementation.IL2CPP;
                if (wsaSDK == WSASDK.UniversalSDK81)
                {
                    wsaSDK = WSASDK.SDK81;
                }
                if ((wsaSDK != WSASDK.UWP) && !flag2)
                {
                    list.Add(MicrosoftCSharpCompiler.GetNETCoreFrameworkReferencesDirectory(wsaSDK));
                }
            }
            return list.ToArray();
        }

        public override CSharpCompiler GetCsCompiler(bool buildingForEditor, string assemblyName)
        {
            if (!buildingForEditor)
            {
                assemblyName = Path.GetFileNameWithoutExtension(assemblyName);
                PlayerSettings.WSACompilationOverrides compilationOverrides = PlayerSettings.WSA.compilationOverrides;
                if (compilationOverrides != PlayerSettings.WSACompilationOverrides.UseNetCore)
                {
                    if ((compilationOverrides == PlayerSettings.WSACompilationOverrides.UseNetCorePartially) && (string.Compare(assemblyName, Utility.AssemblyCSharpName, true) == 0))
                    {
                        return CSharpCompiler.Microsoft;
                    }
                }
                else if ((string.Compare(assemblyName, Utility.AssemblyCSharpName, true) == 0) || (string.Compare(assemblyName, Utility.AssemblyCSharpFirstPassName, true) == 0))
                {
                    return CSharpCompiler.Microsoft;
                }
            }
            return CSharpCompiler.Mono;
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

        internal static string GetWindowsKitDirectory(WSASDK wsaSDK)
        {
            string str;
            string str2;
            switch (wsaSDK)
            {
                case WSASDK.SDK80:
                    str2 = RegistryUtil.GetRegistryStringValue(@"SOFTWARE\Microsoft\Microsoft SDKs\Windows\v8.0", "InstallationFolder", null, RegistryView._32);
                    str = @"Windows Kits\8.0";
                    break;

                case WSASDK.SDK81:
                    str2 = RegistryUtil.GetRegistryStringValue(@"SOFTWARE\Microsoft\Microsoft SDKs\Windows\v8.1", "InstallationFolder", null, RegistryView._32);
                    str = @"Windows Kits\8.1";
                    break;

                case WSASDK.PhoneSDK81:
                    str2 = RegistryUtil.GetRegistryStringValue(@"SOFTWARE\Microsoft\Microsoft SDKs\WindowsPhoneApp\v8.1", "InstallationFolder", null, RegistryView._32);
                    str = @"Windows Phone Kits\8.1";
                    break;

                case WSASDK.UWP:
                    str2 = RegistryUtil.GetRegistryStringValue(@"SOFTWARE\Microsoft\Microsoft SDKs\Windows\v10.0", "InstallationFolder", null, RegistryView._32);
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

        public override IEnumerable<string> GetWindowsMetadataReferences()
        {
            if ((PlayerSettings.GetScriptingBackend(BuildTargetGroup.WSA) != ScriptingImplementation.WinRTDotNET) && (PlayerSettings.GetApiCompatibilityLevel(BuildTargetGroup.WSA) != ApiCompatibilityLevel.NET_4_6))
            {
                return new string[0];
            }
            List<string> list = new List<string>();
            if (EditorUserBuildSettings.wsaSDK == WSASDK.UWP)
            {
                if (_uwpReferences == null)
                {
                    _uwpReferences = UWPReferences.GetReferences();
                }
                list.AddRange(_uwpReferences);
            }
            list.Add(GetWindowsWinmdPath(EditorUserBuildSettings.wsaSDK));
            return list;
        }

        internal static string GetWindowsWinmdPath(WSASDK wsaSDK)
        {
            string str2;
            string str3;
            string windowsKitDirectory = GetWindowsKitDirectory(wsaSDK);
            if (wsaSDK == WSASDK.UWP)
            {
                str2 = Path.Combine(windowsKitDirectory, @"UnionMetadata\Facade\Windows.winmd");
            }
            else
            {
                str2 = Path.Combine(windowsKitDirectory, @"References\CommonConfiguration\Neutral\Windows.winmd");
            }
            if (File.Exists(str2))
            {
                return str2;
            }
            switch (wsaSDK)
            {
                case WSASDK.SDK80:
                    str3 = "8.0";
                    break;

                case WSASDK.SDK81:
                    str3 = "8.1";
                    break;

                case WSASDK.PhoneSDK81:
                    str3 = "Phone 8.1";
                    break;

                case WSASDK.UWP:
                    str3 = "UAP";
                    break;

                default:
                    throw new Exception("Unknown Windows SDK: " + wsaSDK.ToString());
            }
            throw new Exception($"'{str2}' not found, do you have Windows {str3} SDK installed?");
        }

        private bool IsUserScriptAssembly(string assemblyPathName)
        {
            <IsUserScriptAssembly>c__AnonStorey1 storey = new <IsUserScriptAssembly>c__AnonStorey1 {
                assemblyName = Path.GetFileNameWithoutExtension(assemblyPathName),
                suffix = EditorSettings.Internal_UserGeneratedProjectSuffix
            };
            return Enumerable.Any<string>(userScriptAssemblies, new Func<string, bool>(storey.<>m__0));
        }

        [CompilerGenerated]
        private sealed class <GetAdditionalDefines>c__Iterator0 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
            internal bool $disposing;
            internal int $PC;
            internal bool <isDotNetScriptingBackend>__1;
            internal bool <isTargeting46Profile>__1;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$disposing = true;
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<isDotNetScriptingBackend>__1 = PlayerSettings.GetScriptingBackend(BuildTargetGroup.WSA) == ScriptingImplementation.WinRTDotNET;
                        this.<isTargeting46Profile>__1 = PlayerSettings.GetApiCompatibilityLevel(BuildTargetGroup.WSA) == ApiCompatibilityLevel.NET_4_6;
                        if (!this.<isDotNetScriptingBackend>__1)
                        {
                            break;
                        }
                        this.$current = "NETFX_CORE";
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        goto Label_00DE;

                    case 1:
                        break;

                    case 2:
                        if (EditorUserBuildSettings.wsaSDK != WSASDK.UWP)
                        {
                            goto Label_00D5;
                        }
                        this.$current = "WINDOWS_UWP";
                        if (!this.$disposing)
                        {
                            this.$PC = 3;
                        }
                        goto Label_00DE;

                    case 3:
                        goto Label_00D5;

                    default:
                        goto Label_00DC;
                }
                if (this.<isDotNetScriptingBackend>__1 || this.<isTargeting46Profile>__1)
                {
                    this.$current = "ENABLE_WINMD_SUPPORT";
                    if (!this.$disposing)
                    {
                        this.$PC = 2;
                    }
                    goto Label_00DE;
                }
            Label_00D5:
                this.$PC = -1;
            Label_00DC:
                return false;
            Label_00DE:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<string> IEnumerable<string>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new MetroCompilationExtension.<GetAdditionalDefines>c__Iterator0();
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<string>.GetEnumerator();

            string IEnumerator<string>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }

        [CompilerGenerated]
        private sealed class <IsUserScriptAssembly>c__AnonStorey1
        {
            internal string assemblyName;
            internal string suffix;

            internal bool <>m__0(string x) => 
                x.Equals(this.assemblyName + this.suffix, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}

