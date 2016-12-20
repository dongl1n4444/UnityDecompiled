namespace Unity.IL2CPP.Building.ToolChains
{
    using NiceIO;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP.Common;
    using Unity.IL2CPP.Portability;

    public static class EmscriptenPaths
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static Dictionary<string, string> <EmscriptenEnvironmentVariables>k__BackingField;

        static EmscriptenPaths()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string> {
                { 
                    "EM_CONFIG",
                    EmscriptenConfig.ToString()
                },
                { 
                    "LLVM",
                    ExternalEmscriptenLlvmRoot.ToString()
                },
                { 
                    "NODE",
                    NodeExecutable.ToString()
                },
                { 
                    "EMSCRIPTEN",
                    EmscriptenToolsRoot.ToString()
                },
                { 
                    "EMSCRIPTEN_TMP",
                    TempDir.Empty("emscriptenTemp").ToString()
                },
                { 
                    "EM_CACHE",
                    TempDir.Empty("emscriptencache").ToString()
                },
                { 
                    "EMSCRIPTEN_NATIVE_OPTIMIZER",
                    OptimizerExecutable.ToString()
                },
                { 
                    "BINARYEN",
                    BinaryenRoot.ToString()
                },
                { 
                    "EMCC_WASM_BACKEND",
                    "0"
                }
            };
            EmscriptenEnvironmentVariables = dictionary;
        }

        public static void ShowWindowsEnvironmentSettings()
        {
            foreach (KeyValuePair<string, string> pair in EmscriptenEnvironmentVariables)
            {
                Console.WriteLine("SET {0}={1}", pair.Key, pair.Value);
            }
        }

        private static NPath WebGLExternalPathForCurrentUnityVersion(string path)
        {
            if (!UseEmscriptenLocation5_5)
            {
                string[] textArray1 = new string[] { path };
                NPath path2 = WebGlExternalRoot.Combine(textArray1);
                if (path2.Exists(""))
                {
                    return path2;
                }
            }
            string[] append = new string[] { "Emscripten/", path };
            return WebGlExternalRoot.Combine(append);
        }

        public static NPath BaseLocation
        {
            get
            {
                return new NPath(new Uri(AssemblyExtensions.GetCodeBasePortable(TypeExtensions.GetAssemblyPortable(typeof(EmscriptenToolChain)))).LocalPath);
            }
        }

        public static NPath BinaryenRoot
        {
            get
            {
                string[] append = new string[] { "binaryen" };
                return ExternalEmscriptenLlvmRoot.Combine(append);
            }
        }

        public static NPath Emcc
        {
            get
            {
                string[] append = new string[] { "emcc" };
                return EmscriptenToolsRoot.Combine(append);
            }
        }

        public static NPath Emcpp
        {
            get
            {
                string[] append = new string[] { "em++" };
                return EmscriptenToolsRoot.Combine(append);
            }
        }

        public static NPath EmscriptenConfig
        {
            get
            {
                string[] append = new string[] { "emscripten.config" };
                return WebGlRoot.Combine(append);
            }
        }

        public static Dictionary<string, string> EmscriptenEnvironmentVariables
        {
            [CompilerGenerated]
            get
            {
                return <EmscriptenEnvironmentVariables>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                <EmscriptenEnvironmentVariables>k__BackingField = value;
            }
        }

        public static NPath EmscriptenToolsRoot
        {
            get
            {
                if (UnitySourceCode.Available)
                {
                    return WebGLExternalPathForCurrentUnityVersion("Emscripten/builds");
                }
                string[] append = new string[] { "Emscripten" };
                return WebGlRoot.Combine(append);
            }
        }

        public static NPath ExternalEmscriptenLlvmRoot
        {
            get
            {
                string str;
                if (PlatformUtils.IsWindows())
                {
                    str = "EmscriptenFastComp_Win/";
                }
                else if (PlatformUtils.IsOSX())
                {
                    str = "EmscriptenFastComp_Mac/";
                }
                else
                {
                    if (!PlatformUtils.IsLinux())
                    {
                        throw new NotSupportedException("Unknown or unsupported OS for Emscripten");
                    }
                    str = "EmscriptenFastComp_Linux/";
                }
                return (!UnitySourceCode.Available ? WebGlRoot.Combine(new string[] { str }) : WebGLExternalPathForCurrentUnityVersion(str + "builds/"));
            }
        }

        public static NPath LinuxEmscriptenSdkRoot
        {
            get
            {
                return MacEmscriptenSdkRoot;
            }
        }

        public static string LlcExecutableName
        {
            get
            {
                if (PlatformUtils.IsWindows())
                {
                    return "llc.exe";
                }
                return "llc";
            }
        }

        public static NPath MacEmscriptenSdkRoot
        {
            get
            {
                if (UnitySourceCode.Available)
                {
                    return WebGLExternalPathForCurrentUnityVersion("EmscriptenSdk_Mac/builds/");
                }
                string[] append = new string[] { "Emscripten_Mac/" };
                return WebGlRoot.Combine(append);
            }
        }

        public static NPath NodeExecutable
        {
            get
            {
                if (PlatformUtils.IsWindows())
                {
                    string[] append = new string[] { "node/node.exe" };
                    return WindowsEmscriptenSdkRoot.Combine(append);
                }
                if (PlatformUtils.IsOSX())
                {
                    string[] textArray2 = new string[] { "node/0.10.18_64bit/bin/node" };
                    return MacEmscriptenSdkRoot.Combine(textArray2);
                }
                if (!PlatformUtils.IsLinux())
                {
                    throw new NotSupportedException("Don't know how to get node path on current platform!");
                }
                return new NPath(!File.Exists("/usr/bin/node") ? "nodejs" : "node");
            }
        }

        public static NPath OptimizerExecutable
        {
            get
            {
                string[] append = new string[] { "optimizer.exe" };
                return ExternalEmscriptenLlvmRoot.Combine(append);
            }
        }

        public static NPath Python
        {
            get
            {
                if (PlatformUtils.IsWindows())
                {
                    string[] append = new string[] { "python/2.7.5.3_64bit/python.exe" };
                    return WindowsEmscriptenSdkRoot.Combine(append);
                }
                if (PlatformUtils.IsOSX())
                {
                    return new NPath("/usr/bin/python");
                }
                if (!PlatformUtils.IsLinux())
                {
                    throw new NotSupportedException("Don't know how to get python path on current platform!");
                }
                return new NPath(!File.Exists("/usr/bin/python2") ? "python" : "python2");
            }
        }

        private static bool UseEmscriptenLocation5_5
        {
            get
            {
                string[] append = new string[] { "External/Emscripten/Emscripten/builds.7z" };
                return UnitySourceCode.Paths.UnityRoot.Combine(append).Exists("");
            }
        }

        public static NPath WebGlExternalRoot
        {
            get
            {
                return WebGLExternalRootForCurrentUnityVersion;
            }
        }

        private static NPath WebGLExternalRootForCurrentUnityVersion
        {
            get
            {
                if (!UseEmscriptenLocation5_5)
                {
                    string[] textArray1 = new string[] { "External/" };
                    NPath path = WebGlRoot.Combine(textArray1);
                    if (path.Exists(""))
                    {
                        return path;
                    }
                }
                string[] append = new string[] { "External/" };
                return UnitySourceCode.Paths.UnityRoot.Combine(append);
            }
        }

        public static NPath WebGlRoot
        {
            get
            {
                if (UnitySourceCode.Available)
                {
                    string[] textArray1 = new string[] { "PlatformDependent/WebGL" };
                    return UnitySourceCode.Paths.UnityRoot.Combine(textArray1);
                }
                string[] append = new string[] { "PlaybackEngines", "WebGLSupport", "BuildTools" };
                NPath path2 = BaseLocation.ParentContaining("PlaybackEngines");
                NPath path3 = path2.Combine(append);
                if (path3.Exists(""))
                {
                    return path3;
                }
                return path2.Parent.ParentContaining("PlaybackEngines").Combine(append);
            }
        }

        public static NPath WindowsEmscriptenSdkRoot
        {
            get
            {
                if (UnitySourceCode.Available)
                {
                    return WebGLExternalPathForCurrentUnityVersion("EmscriptenSdk_Win/builds/");
                }
                string[] append = new string[] { "Emscripten_Win/" };
                return WebGlRoot.Combine(append);
            }
        }
    }
}

