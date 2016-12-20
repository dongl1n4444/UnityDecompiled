namespace Unity.UNetWeaver
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class Program
    {
        private static void CheckAssemblies(IEnumerable<string> assemblyPaths)
        {
            foreach (string str in assemblyPaths)
            {
                CheckAssemblyPath(str);
            }
        }

        private static void CheckAssemblyPath(string assemblyPath)
        {
            if (!File.Exists(assemblyPath))
            {
                throw new Exception("Assembly " + assemblyPath + " does not exist!");
            }
        }

        private static void CheckDLLPath(string path)
        {
            if (!File.Exists(path))
            {
                throw new Exception("dll could not be located at " + path + "!");
            }
        }

        private static void CheckOutputDirectory(string outputDir)
        {
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }
        }

        public static bool Process(string unityEngine, string unetDLL, string outputDirectory, string[] assemblies, string[] extraAssemblyPaths, IAssemblyResolver assemblyResolver, Action<string> printWarning, Action<string> printError)
        {
            CheckDLLPath(unityEngine);
            CheckDLLPath(unetDLL);
            CheckOutputDirectory(outputDirectory);
            CheckAssemblies(assemblies);
            Log.WarningMethod = printWarning;
            Log.ErrorMethod = printError;
            return Weaver.WeaveAssemblies(assemblies, extraAssemblyPaths, assemblyResolver, outputDirectory, unityEngine, unetDLL);
        }
    }
}

