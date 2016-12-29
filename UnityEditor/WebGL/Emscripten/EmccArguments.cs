namespace UnityEditor.WebGL.Emscripten
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using UnityEngine;

    internal class EmccArguments
    {
        private static void FixClangSymLinkOnMac()
        {
            if (EmscriptenPaths.IsMac())
            {
                FixSymlinkIfNecessary("/clang++");
                FixSymlinkIfNecessary("/clang");
            }
        }

        private static void FixSymlinkIfNecessary(string binary)
        {
            if (new FileInfo(EmscriptenPaths.llvmDir + binary).Length == 0L)
            {
                Process process = Process.Start("ln", "-sf clang-3.3 " + EmscriptenPaths.llvmDir + binary);
                if (process != null)
                {
                    process.WaitForExit();
                }
            }
        }

        private static string SetEnvironmentVariable(ProcessStartInfo startInfo, string environmentVariable, string value)
        {
            if (debugEnvironmentAndInvocations)
            {
                UnityEngine.Debug.Log($"SET {environmentVariable}={value}");
            }
            string str = value;
            startInfo.EnvironmentVariables[environmentVariable] = str;
            return str;
        }

        internal static void SetupDefaultEmscriptenEnvironment(ProcessStartInfo startInfo)
        {
            FixClangSymLinkOnMac();
            SetEnvironmentVariable(startInfo, "EM_CONFIG", EmscriptenPaths.emscriptenConfig);
            SetEnvironmentVariable(startInfo, "LLVM", EmscriptenPaths.llvmDir);
            SetEnvironmentVariable(startInfo, "NODE", EmscriptenPaths.nodeExecutable);
            SetEnvironmentVariable(startInfo, "EMSCRIPTEN", EmscriptenPaths.emscriptenDir);
            SetEnvironmentVariable(startInfo, "EMSCRIPTEN_TMP", EmscriptenPaths.tempDirForEmscriptenCompiler);
            SetEnvironmentVariable(startInfo, "EM_CACHE", EmscriptenPaths.emscriptenCache);
            SetEnvironmentVariable(startInfo, "EMSCRIPTEN_NATIVE_OPTIMIZER", EmscriptenPaths.optimizer);
            SetEnvironmentVariable(startInfo, "BINARYEN", EmscriptenPaths.binaryen);
            SetEnvironmentVariable(startInfo, "EMCC_WASM_BACKEND", "0");
            SetEnvironmentVariable(startInfo, "EM_EXCLUSIVE_CACHE_ACCESS", "1");
        }

        public static bool debugEnvironmentAndInvocations =>
            false;
    }
}

