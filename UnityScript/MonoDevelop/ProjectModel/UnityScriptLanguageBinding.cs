namespace UnityScript.MonoDevelop.ProjectModel
{
    using MonoDevelop.Core;
    using MonoDevelop.Projects;
    using System;
    using System.CodeDom.Compiler;
    using System.Xml;

    public class UnityScriptLanguageBinding : IDotNetLanguageBinding, ILanguageBinding
    {
        public BuildResult Compile(ProjectItemCollection items, DotNetProjectConfiguration configuration, ConfigurationSelector configSelector, IProgressMonitor monitor) => 
            new UnityScriptCompiler(configuration, configSelector, items, monitor).Run();

        public ConfigurationParameters CreateCompilationParameters(XmlElement projectOptions) => 
            new UnityScriptCompilationParameters();

        public ProjectParameters CreateProjectParameters(XmlElement projectOptions) => 
            new UnityScriptProjectParameters();

        public CodeDomProvider GetCodeDomProvider() => 
            null;

        public FilePath GetFileName(FilePath fileNameWithoutExtension) => 
            ((FilePath) (fileNameWithoutExtension + ".js"));

        public ClrVersion[] GetSupportedClrVersions() => 
            new ClrVersion[] { 1, 2, 3, 4 };

        public bool IsSourceCodeFile(FilePath fileName) => 
            IsUnitScriptFile(fileName);

        private static bool IsUnitScriptFile(string fileName)
        {
            if (!fileName.ToLower().EndsWith(".js"))
            {
                return fileName.ToLower().EndsWith(".us");
            }
            return true;
        }

        public string BlockCommentEndTag =>
            "*/";

        public string BlockCommentStartTag =>
            "/*";

        public string Language =>
            "UnityScript";

        public string ProjectStockIcon =>
            "md-project";

        public string SingleLineCommentTag =>
            "//";
    }
}

