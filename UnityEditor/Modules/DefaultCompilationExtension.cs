namespace UnityEditor.Modules
{
    using Mono.Cecil;
    using System;

    internal class DefaultCompilationExtension : ICompilationExtension
    {
        public virtual IAssemblyResolver GetAssemblyResolver(bool buildingForEditor, string assemblyPath, string[] searchDirectories) => 
            null;

        public virtual string[] GetCompilerExtraAssemblyPaths(bool isEditor, string assemblyPathName) => 
            new string[0];

        public virtual CSharpCompiler GetCsCompiler(bool buildingForEditor, string assemblyName) => 
            CSharpCompiler.Mono;
    }
}

