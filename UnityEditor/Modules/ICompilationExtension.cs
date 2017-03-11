namespace UnityEditor.Modules
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;

    internal interface ICompilationExtension
    {
        IEnumerable<string> GetAdditionalAssemblyReferences();
        IEnumerable<string> GetAdditionalDefines();
        IEnumerable<string> GetAdditionalSourceFiles();
        IAssemblyResolver GetAssemblyResolver(bool buildingForEditor, string assemblyPath, string[] searchDirectories);
        string[] GetCompilerExtraAssemblyPaths(bool isEditor, string assemblyPathName);
        CSharpCompiler GetCsCompiler(bool buildingForEditor, string assemblyName);
        IEnumerable<string> GetWindowsMetadataReferences();
    }
}

