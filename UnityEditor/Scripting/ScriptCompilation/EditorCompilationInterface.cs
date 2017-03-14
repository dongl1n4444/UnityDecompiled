namespace UnityEditor.Scripting.ScriptCompilation
{
    using System;
    using UnityEditor;
    using UnityEditor.Scripting;
    using UnityEngine.Scripting;

    internal static class EditorCompilationInterface
    {
        private static readonly EditorCompilation editorCompilation = new EditorCompilation();

        [RequiredByNativeCode]
        public static void ClearCompileErrors()
        {
            editorCompilation.ClearCompileErrors();
        }

        [RequiredByNativeCode]
        public static bool CompileScripts(EditorScriptCompilationOptions definesOptions, BuildTargetGroup platformGroup, BuildTarget platform) => 
            editorCompilation.CompileScripts(definesOptions, platformGroup, platform);

        [RequiredByNativeCode]
        public static void DeleteUnusedAssemblies()
        {
            editorCompilation.DeleteUnusedAssemblies();
        }

        [RequiredByNativeCode]
        public static void DirtyAllScripts()
        {
            editorCompilation.DirtyAllScripts();
        }

        [RequiredByNativeCode]
        public static void DirtyScript(string path)
        {
            editorCompilation.DirtyScript(path);
        }

        [RequiredByNativeCode]
        public static bool DoesProjectFolderHaveAnyDirtyScripts() => 
            editorCompilation.DoesProjectFolderHaveAnyDirtyScripts();

        [RequiredByNativeCode]
        public static bool DoesProjectFolderHaveAnyScripts() => 
            editorCompilation.DoesProjectFolderHaveAnyScripts();

        [RequiredByNativeCode]
        public static EditorCompilation.AssemblyCompilerMessages[] GetAllAssemblyCompilerMessages() => 
            editorCompilation.GetAllAssemblyCompilerMessages();

        [RequiredByNativeCode]
        public static MonoIsland[] GetAllMonoIslands() => 
            editorCompilation.GetAllMonoIslands();

        [RequiredByNativeCode]
        public static EditorCompilation.AssemblyCompilerMessages[] GetLastAssemblyCompilerMessages() => 
            editorCompilation.GetLastAssemblyCompilerMessages();

        [RequiredByNativeCode]
        public static EditorCompilation.TargetAssemblyInfo[] GetTargetAssemblies() => 
            editorCompilation.GetTargetAssemblies();

        [RequiredByNativeCode]
        public static EditorCompilation.TargetAssemblyInfo GetTargetAssembly(string scriptPath) => 
            editorCompilation.GetTargetAssembly(scriptPath);

        public static EditorBuildRules.TargetAssembly GetTargetAssemblyDetails(string scriptPath) => 
            editorCompilation.GetTargetAssemblyDetails(scriptPath);

        [RequiredByNativeCode]
        public static bool HaveCompileErrors() => 
            editorCompilation.HaveCompileErrors();

        [RequiredByNativeCode]
        public static bool IsCompilationPending() => 
            editorCompilation.IsCompilationPending();

        [RequiredByNativeCode]
        public static bool IsCompiling() => 
            editorCompilation.IsCompiling();

        [RequiredByNativeCode]
        public static bool IsExtensionSupportedByCompiler(string extension) => 
            editorCompilation.IsExtensionSupportedByCompiler(extension);

        [RequiredByNativeCode]
        public static void RunScriptUpdaterOnAssembly(string assemblyFilename)
        {
            editorCompilation.RunScriptUpdaterOnAssembly(assemblyFilename);
        }

        [RequiredByNativeCode]
        public static void SetAllCustomScriptAssemblyJsons(string[] allAssemblyJsons)
        {
            editorCompilation.SetAllCustomScriptAssemblyJsons(allAssemblyJsons);
        }

        [RequiredByNativeCode]
        public static void SetAllPrecompiledAssemblies(PrecompiledAssembly[] precompiledAssemblies)
        {
            editorCompilation.SetAllPrecompiledAssemblies(precompiledAssemblies);
        }

        [RequiredByNativeCode]
        public static void SetAllScripts(string[] allScripts)
        {
            editorCompilation.SetAllScripts(allScripts);
        }

        [RequiredByNativeCode]
        public static void SetAllUnityAssemblies(PrecompiledAssembly[] unityAssemblies)
        {
            editorCompilation.SetAllUnityAssemblies(unityAssemblies);
        }

        [RequiredByNativeCode]
        public static void SetAssemblySuffix(string suffix)
        {
            editorCompilation.SetAssemblySuffix(suffix);
        }

        [RequiredByNativeCode]
        public static void StopAllCompilation()
        {
            editorCompilation.StopAllCompilation();
        }

        [RequiredByNativeCode]
        public static EditorCompilation.CompileStatus TickCompilationPipeline() => 
            editorCompilation.TickCompilationPipeline();
    }
}

