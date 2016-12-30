namespace UnityScript.MonoDevelop.ProjectModel
{
    using MonoDevelop.Core;
    using MonoDevelop.Projects;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class UnityScriptCompiler
    {
        private UnityScriptCompilationParameters compilationParameters;
        private DotNetProjectConfiguration config;
        private IProgressMonitor monitor;
        private ProjectItemCollection projectItems;
        private ConfigurationSelector selector;

        public UnityScriptCompiler(DotNetProjectConfiguration config, ConfigurationSelector selector, ProjectItemCollection projectItems, IProgressMonitor monitor)
        {
            this.config = config;
            this.selector = selector;
            this.projectItems = projectItems;
            this.monitor = monitor;
            this.compilationParameters = ((UnityScriptCompilationParameters) config.get_CompilationParameters()) ?? new UnityScriptCompilationParameters();
        }

        private bool ContainsReference(string[] files, string fileName) => 
            Array.Exists<string>(files, f => Path.GetFileName(f) == fileName);

        private string ExecuteProcess(string executable, string commandLine)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(executable, commandLine) {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            using (Process process = Runtime.get_SystemAssemblyService().get_CurrentRuntime().ExecuteAssembly(startInfo, this.config.get_TargetFramework()))
            {
                return (process.StandardOutput.ReadToEnd() + Environment.NewLine + process.StandardError.ReadToEnd());
            }
        }

        private string[] GetReferencedFileNames() => 
            (from r in this.projectItems.OfType<ProjectReference>() select r.GetReferencedFileNames(this.selector)).ToArray<string>();

        private bool IsWarningCode(string code) => 
            (!string.IsNullOrEmpty(code) && code.StartsWith("BCW"));

        private string MapPath(string path) => 
            Path.Combine(Path.GetDirectoryName(base.GetType().Assembly.Location), path);

        private BuildResult ParseBuildResult(string stdout)
        {
            BuildResult result = new BuildResult();
            using (StringReader reader = new StringReader(stdout))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Match match = Regex.Match(line, @"(.+)\((\d+),(\d+)\):\s+(BC.+?):\s+(.+)$");
                    if (match.Success)
                    {
                        BuildError <>g__initLocal6 = new BuildError();
                        <>g__initLocal6.set_FileName(match.Groups[1].Value);
                        <>g__initLocal6.set_Line(int.Parse(match.Groups[2].Value));
                        <>g__initLocal6.set_Column(int.Parse(match.Groups[3].Value));
                        <>g__initLocal6.set_IsWarning(this.IsWarningCode(match.Groups[4].Value));
                        <>g__initLocal6.set_ErrorNumber(match.Groups[4].Value);
                        <>g__initLocal6.set_ErrorText(match.Groups[5].Value);
                        result.Append(<>g__initLocal6);
                    }
                    else
                    {
                        match = Regex.Match(line, @"(BC.+):\s+(.+)");
                        if (match.Success)
                        {
                            BuildError <>g__initLocal7 = new BuildError();
                            <>g__initLocal7.set_IsWarning(this.IsWarningCode(match.Groups[1].Value));
                            <>g__initLocal7.set_ErrorNumber(match.Groups[1].Value);
                            <>g__initLocal7.set_ErrorText(match.Groups[2].Value);
                            result.Append(<>g__initLocal7);
                        }
                    }
                }
            }
            return result;
        }

        public BuildResult Run()
        {
            BuildResult CS$1$0000;
            string responseFileName = Path.GetTempFileName();
            try
            {
                this.WriteOptionsToResponseFile(responseFileName);
                string compiler = this.MapPath("UnityScript/us.exe");
                string compilerOutput = this.ExecuteProcess(compiler, "\"@" + responseFileName + "\"");
                CS$1$0000 = this.ParseBuildResult(compilerOutput);
            }
            finally
            {
                FileService.DeleteFile(responseFileName);
            }
            return CS$1$0000;
        }

        private void WriteOptionsToResponseFile(string responseFile)
        {
            StringWriter commandLine = new StringWriter();
            string[] referencedFiles = this.GetReferencedFileNames();
            if (this.ContainsReference(referencedFiles, "UnityEngine.dll"))
            {
                commandLine.WriteLine("-nowarn:BCW0016");
                commandLine.WriteLine("-nowarn:BCW0003");
                commandLine.WriteLine("-base:UnityEngine.MonoBehaviour");
                commandLine.WriteLine("-method:Main");
                commandLine.WriteLine("-i:System.Collections");
                commandLine.WriteLine("-i:UnityEngine");
                if (this.ContainsReference(referencedFiles, "UnityEditor.dll"))
                {
                    commandLine.WriteLine("-i:UnityEditor");
                }
                commandLine.WriteLine("-t:library");
                commandLine.WriteLine("-x-type-inference-rule-attribute:UnityEngineInternal.TypeInferenceRuleAttribute");
            }
            else
            {
                commandLine.WriteLine("-base:System.Object");
                commandLine.WriteLine("-method:Awake");
                commandLine.WriteLine("-t:exe");
            }
            commandLine.WriteLine("-debug+");
            commandLine.WriteLine("-out:" + this.config.get_CompiledOutputName());
            foreach (string define in this.compilationParameters.DefineSymbols)
            {
                commandLine.WriteLine("-define:" + define);
            }
            foreach (string reference in referencedFiles)
            {
                commandLine.WriteLine("-reference:" + reference);
            }
            foreach (ProjectFile file in from item in this.projectItems
                where item is ProjectFile
                select item)
            {
                if (file.get_Subtype() != 1)
                {
                    if (file.get_BuildAction() == "Compile")
                    {
                        commandLine.WriteLine("\"" + file.get_Name() + "\"");
                    }
                    else
                    {
                        Console.WriteLine(string.Concat(new object[] { "Unrecognized build action for file ", file, " - ", file.get_BuildAction() }));
                    }
                }
            }
            string commandLineString = commandLine.ToString();
            if (this.monitor != null)
            {
                this.monitor.get_Log().WriteLine(commandLineString);
            }
            Console.WriteLine(commandLineString);
            File.WriteAllText(responseFile, commandLineString);
        }
    }
}

