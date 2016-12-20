namespace UnityEditor.WebGL.Emscripten
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using UnityEditor;
    using UnityEditor.Utils;
    using UnityEngine;

    internal class EmscriptenPaths
    {
        private static string s_BuildToolsDir;
        private static string s_DataPath;

        public static string GetShortPathName(string path)
        {
            if ((Application.platform != RuntimePlatform.WindowsEditor) || (Encoding.UTF8.GetByteCount(path) == path.Length))
            {
                return path;
            }
            int capacity = WindowsGetShortPathName(path, null, 0);
            if (capacity == 0)
            {
                return path;
            }
            StringBuilder lpszShortPath = new StringBuilder(capacity);
            capacity = WindowsGetShortPathName(path, lpszShortPath, lpszShortPath.Capacity);
            if (capacity == 0)
            {
                return path;
            }
            return lpszShortPath.ToString(0, capacity);
        }

        public static bool IsLinux()
        {
            return Directory.Exists("/proc");
        }

        public static bool IsMac()
        {
            return (!IsLinux() && !IsWindows());
        }

        public static bool IsWindows()
        {
            return ((((Environment.OSVersion.Platform == PlatformID.Win32NT) || (Environment.OSVersion.Platform == PlatformID.Win32S)) || (Environment.OSVersion.Platform == PlatformID.Win32Windows)) || (Environment.OSVersion.Platform == PlatformID.WinCE));
        }

        public static void SetupBuildToolsDir()
        {
            string[] components = new string[] { BuildPipeline.GetPlaybackEngineDirectory(BuildTarget.WebGL, BuildOptions.CompressTextures), "BuildTools" };
            s_BuildToolsDir = Paths.Combine(components);
        }

        public static void SetupDataPath()
        {
            s_DataPath = GetShortPathName(Path.GetFullPath(Application.dataPath));
        }

        [DllImport("kernel32.dll", EntryPoint="GetShortPathName", CharSet=CharSet.Unicode, SetLastError=true)]
        private static extern int WindowsGetShortPathName([MarshalAs(UnmanagedType.LPWStr)] string lpszLongPath, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpszShortPath, int cchBuffer);

        public static string binaryen
        {
            get
            {
                string[] components = new string[] { llvmDir, "binaryen" };
                return Paths.Combine(components);
            }
        }

        public static string binaryenShellExecutable
        {
            get
            {
                string[] components = new string[] { binaryen, "bin", "binaryen-shell" };
                string str = Paths.Combine(components);
                if (IsWindows())
                {
                    str = str + ".exe";
                }
                return str;
            }
        }

        public static string buildToolsDir
        {
            get
            {
                if (s_BuildToolsDir == null)
                {
                    SetupBuildToolsDir();
                }
                return s_BuildToolsDir;
            }
        }

        public static string cacheDirForIl2CppIncrementalBuildArtifacts
        {
            get
            {
                string[] components = new string[] { dataPath, "..", "Library" };
                return Paths.Combine(components);
            }
        }

        public static string dataPath
        {
            get
            {
                if (s_DataPath == null)
                {
                    SetupDataPath();
                }
                return s_DataPath;
            }
        }

        public static string emcc
        {
            get
            {
                string[] components = new string[] { emscriptenDir, "emcc" };
                return Paths.Combine(components);
            }
        }

        public static string emlink
        {
            get
            {
                string[] components = new string[] { emscriptenDir, "emlink.py" };
                return Paths.Combine(components);
            }
        }

        public static string emscriptenCache
        {
            get
            {
                string[] components = new string[] { llvmDir, "cache" };
                return Paths.Combine(components);
            }
        }

        public static string emscriptenConfig
        {
            get
            {
                string[] components = new string[] { buildToolsDir, "emscripten.config" };
                return Paths.Combine(components);
            }
        }

        public static string emscriptenDir
        {
            get
            {
                string[] components = new string[] { buildToolsDir, "Emscripten" };
                return Paths.Combine(components);
            }
        }

        public static string emscriptenPlatformSdkDir
        {
            get
            {
                if (IsWindows())
                {
                    string[] textArray1 = new string[] { buildToolsDir, "Emscripten_Win" };
                    return Paths.Combine(textArray1);
                }
                if (!IsLinux() && !IsMac())
                {
                    throw new Exception("Unknown platform");
                }
                string[] components = new string[] { buildToolsDir, "Emscripten_Mac" };
                return Paths.Combine(components);
            }
        }

        public static string llvmDir
        {
            get
            {
                string str = "Unknown";
                if (IsWindows())
                {
                    str = "Win";
                }
                else if (IsMac())
                {
                    str = "Mac";
                }
                else
                {
                    if (!IsLinux())
                    {
                        throw new Exception("Unknown platform");
                    }
                    str = "Linux";
                }
                string[] components = new string[] { buildToolsDir, "Emscripten_FastComp_" + str };
                return Paths.Combine(components);
            }
        }

        public static string nmExecutable
        {
            get
            {
                string[] components = new string[] { llvmDir, "llvm-nm" };
                string str = Paths.Combine(components);
                if (IsWindows())
                {
                    str = str + ".exe";
                }
                return str;
            }
        }

        public static string nodeExecutable
        {
            get
            {
                if (IsWindows())
                {
                    string[] textArray1 = new string[] { emscriptenPlatformSdkDir, "node", "node.exe" };
                    return Paths.Combine(textArray1);
                }
                if (IsLinux())
                {
                    return (!File.Exists("/usr/bin/nodejs") ? "node" : "nodejs");
                }
                if (!IsMac())
                {
                    throw new Exception("Unknown platform");
                }
                string[] components = new string[] { emscriptenPlatformSdkDir, "node", "0.10.18_64bit", "bin", "node" };
                return Paths.Combine(components);
            }
        }

        public static string optimizer
        {
            get
            {
                string[] components = new string[] { llvmDir, "optimizer.exe" };
                return Paths.Combine(components);
            }
        }

        public static string packager
        {
            get
            {
                string[] components = new string[] { emscriptenDir, "tools", "file_packager.py" };
                return Paths.Combine(components);
            }
        }

        public static string pythonExecutable
        {
            get
            {
                if (IsWindows())
                {
                    string[] components = new string[] { emscriptenPlatformSdkDir, "python", "2.7.5.3_64bit", "python.exe" };
                    return ("\"" + Paths.Combine(components) + "\"");
                }
                return "python";
            }
        }

        public static string tempDirForEmscriptenCompiler
        {
            get
            {
                string[] components = new string[] { dataPath, "..", "Temp", "EmscriptenTemp" };
                return Paths.Combine(components);
            }
        }

        public static string workingDirForEmscriptenCompiler
        {
            get
            {
                string[] components = new string[] { dataPath, "..", "Temp", "EmscriptenWork" };
                return Paths.Combine(components);
            }
        }
    }
}

