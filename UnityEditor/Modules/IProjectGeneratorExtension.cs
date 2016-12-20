namespace UnityEditor.Modules
{
    using System;
    using System.Collections.Generic;

    internal interface IProjectGeneratorExtension
    {
        void GenerateCSharpProject(CSharpProject project, string assemblyName, IEnumerable<string> sourceFiles, IEnumerable<string> defines, IEnumerable<CSharpProject> additionalProjectReferences);
    }
}

