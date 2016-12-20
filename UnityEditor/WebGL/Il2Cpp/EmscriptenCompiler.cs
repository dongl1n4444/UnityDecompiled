namespace UnityEditor.WebGL.Il2Cpp
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor.WebGL.Emscripten;

    public class EmscriptenCompiler
    {
        [CompilerGenerated]
        private static Func<string, string> <>f__am$cache0;
        public const string LinkerFlags = "-Oz -s NO_EXIT_RUNTIME=1";

        public static void CleanupAndCreateEmscriptenDirs()
        {
            EmscriptenPaths.SetupDataPath();
            if (Directory.Exists(EmscriptenPaths.tempDirForEmscriptenCompiler))
            {
                Directory.Delete(EmscriptenPaths.tempDirForEmscriptenCompiler, true);
            }
            if (Directory.Exists(EmscriptenPaths.workingDirForEmscriptenCompiler))
            {
                Directory.Delete(EmscriptenPaths.workingDirForEmscriptenCompiler, true);
            }
            Directory.CreateDirectory(EmscriptenPaths.tempDirForEmscriptenCompiler);
            Directory.CreateDirectory(EmscriptenPaths.workingDirForEmscriptenCompiler);
        }

        public static string GetCompilerFlags(bool exceptionSupport)
        {
            return ("-Oz" + (exceptionSupport ? "" : " -DIL2CPP_EXCEPTION_DISABLED=1 "));
        }

        public static IEnumerable<string> GetIncludeFullPaths(IEnumerable<string> includePaths)
        {
            EmscriptenPaths.SetupDataPath();
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<string, string>(null, (IntPtr) <GetIncludeFullPaths>m__0);
            }
            return Enumerable.Select<string, string>(includePaths, <>f__am$cache0);
        }

        public static string GetOutFileFullPath(string outFileRelativePath)
        {
            EmscriptenPaths.SetupDataPath();
            return (EmscriptenPaths.dataPath + " /../" + outFileRelativePath);
        }
    }
}

