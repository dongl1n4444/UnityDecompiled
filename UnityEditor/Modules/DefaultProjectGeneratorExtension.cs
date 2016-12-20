namespace UnityEditor.Modules
{
    using System;
    using System.Collections.Generic;

    internal abstract class DefaultProjectGeneratorExtension : IProjectGeneratorExtension
    {
        protected DefaultProjectGeneratorExtension()
        {
        }

        public virtual void GenerateCSharpProject(CSharpProject project, string assemblyName, IEnumerable<string> sourceFiles, IEnumerable<string> defines, IEnumerable<CSharpProject> additionalProjectReferences)
        {
        }
    }
}

