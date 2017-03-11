namespace UnityEditor.Modules
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;

    internal class DefaultCompilationExtension : ICompilationExtension
    {
        public virtual IEnumerable<string> GetAdditionalAssemblyReferences() => 
            new string[0];

        public virtual IEnumerable<string> GetAdditionalDefines() => 
            new string[0];

        public virtual IEnumerable<string> GetAdditionalSourceFiles() => 
            new string[0];

        public virtual IAssemblyResolver GetAssemblyResolver(bool buildingForEditor, string assemblyPath, string[] searchDirectories) => 
            null;

        public virtual string[] GetCompilerExtraAssemblyPaths(bool isEditor, string assemblyPathName) => 
            new string[0];

        public virtual CSharpCompiler GetCsCompiler(bool buildingForEditor, string assemblyName) => 
            CSharpCompiler.Mono;

        public virtual IEnumerable<string> GetWindowsMetadataReferences() => 
            new string[0];
    }
}

