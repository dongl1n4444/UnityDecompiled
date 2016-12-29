namespace UnityEditor.Scripting.Serialization
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using Unity.UNetWeaver;
    using UnityEditor;
    using UnityEditor.Modules;
    using UnityEditor.Scripting;
    using UnityEditor.Utils;
    using UnityEditorInternal;
    using UnityEngine;

    internal static class Weaver
    {
        [CompilerGenerated]
        private static Func<string, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<MonoIsland, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Action<string> <>f__mg$cache0;
        [CompilerGenerated]
        private static Action<string> <>f__mg$cache1;

        private static ICompilationExtension GetCompilationExtension() => 
            ModuleManager.GetCompilationExtension(ModuleManager.GetTargetStringFromBuildTarget(EditorUserBuildSettings.activeBuildTarget));

        public static string[] GetReferences(MonoIsland island, string projectDirectory)
        {
            List<string> list = new List<string>();
            List<string> first = new List<string>();
            foreach (string str in first.Union<string>(island._references))
            {
                string fileName = Path.GetFileName(str);
                if (string.IsNullOrEmpty(fileName) || (!fileName.Contains("UnityEditor.dll") && !fileName.Contains("UnityEngine.dll")))
                {
                    string file = !Path.IsPathRooted(str) ? Path.Combine(projectDirectory, str) : str;
                    if (AssemblyHelper.IsManagedAssembly(file) && !AssemblyHelper.IsInternalAssembly(file))
                    {
                        list.Add(file);
                    }
                }
            }
            return list.ToArray();
        }

        private static ManagedProgram ManagedProgramFor(string exe, string arguments) => 
            new ManagedProgram(MonoInstallationFinder.GetMonoInstallation("MonoBleedingEdge"), null, exe, arguments, false, null);

        private static void QueryAssemblyPathsAndResolver(ICompilationExtension compilationExtension, string file, bool editor, out string[] assemblyPaths, out IAssemblyResolver assemblyResolver)
        {
            assemblyResolver = compilationExtension.GetAssemblyResolver(editor, file, null);
            assemblyPaths = compilationExtension.GetCompilerExtraAssemblyPaths(editor, file).ToArray<string>();
        }

        private static ManagedProgram SerializationWeaverProgramWith(string arguments, string playerPackage) => 
            ManagedProgramFor(playerPackage + "/SerializationWeaver/SerializationWeaver.exe", arguments);

        public static bool ShouldWeave(string name)
        {
            if (name.Contains("Boo."))
            {
                return false;
            }
            if (name.Contains("Mono."))
            {
                return false;
            }
            if (name.Contains("System"))
            {
                return false;
            }
            if (!name.EndsWith(".dll"))
            {
                return false;
            }
            return true;
        }

        public static void WeaveAssembliesInFolder(string folder, string playerPackage)
        {
            ICompilationExtension compilationExtension = GetCompilationExtension();
            string unityEngine = Path.Combine(folder, "UnityEngine.dll");
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<string, bool>(null, (IntPtr) <WeaveAssembliesInFolder>m__0);
            }
            foreach (string str2 in Enumerable.Where<string>(Directory.GetFiles(folder), <>f__am$cache0))
            {
                IAssemblyResolver resolver;
                string[] strArray;
                QueryAssemblyPathsAndResolver(compilationExtension, str2, false, out strArray, out resolver);
                WeaveInto(str2, str2, unityEngine, playerPackage, strArray, resolver);
            }
        }

        public static bool WeaveInto(string unityUNet, string destPath, string unityEngine, string assemblyPath, string[] extraAssemblyPaths, IAssemblyResolver assemblyResolver)
        {
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<MonoIsland, bool>(null, (IntPtr) <WeaveInto>m__1);
            }
            IEnumerable<MonoIsland> enumerable = Enumerable.Where<MonoIsland>(InternalEditorUtility.GetMonoIslands(), <>f__am$cache1);
            string fullName = Directory.GetParent(Application.dataPath).FullName;
            string[] references = null;
            foreach (MonoIsland island in enumerable)
            {
                if (destPath.Equals(island._output))
                {
                    references = GetReferences(island, fullName);
                    break;
                }
            }
            if (references == null)
            {
                Debug.LogError("Weaver failure: unable to locate assemblies (no matching project) for: " + destPath);
                return false;
            }
            List<string> list = new List<string>();
            foreach (string str2 in references)
            {
                list.Add(Path.GetDirectoryName(str2));
            }
            if (extraAssemblyPaths != null)
            {
                list.AddRange(extraAssemblyPaths);
            }
            try
            {
                string[] assemblies = new string[] { assemblyPath };
                if (<>f__mg$cache0 == null)
                {
                    <>f__mg$cache0 = new Action<string>(Debug.LogWarning);
                }
                if (<>f__mg$cache1 == null)
                {
                    <>f__mg$cache1 = new Action<string>(Debug.LogError);
                }
                if (!Program.Process(unityEngine, unityUNet, Path.GetDirectoryName(destPath), assemblies, list.ToArray(), assemblyResolver, <>f__mg$cache0, <>f__mg$cache1))
                {
                    Debug.LogError("Failure generating network code.");
                    return false;
                }
            }
            catch (Exception exception)
            {
                Debug.LogError("Exception generating network code: " + exception.ToString() + " " + exception.StackTrace);
            }
            return true;
        }

        public static bool WeaveUnetFromEditor(string assemblyPath, string destPath, string unityEngine, string unityUNet, bool buildingForEditor)
        {
            IAssemblyResolver resolver;
            string[] strArray;
            Console.WriteLine("WeaveUnetFromEditor " + assemblyPath);
            QueryAssemblyPathsAndResolver(GetCompilationExtension(), assemblyPath, buildingForEditor, out strArray, out resolver);
            return WeaveInto(unityUNet, destPath, unityEngine, assemblyPath, strArray, resolver);
        }
    }
}

