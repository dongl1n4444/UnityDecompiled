namespace UnityEditor.VisualStudioIntegration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Security;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;
    using UnityEditor;
    using UnityEditor.Modules;
    using UnityEditor.Scripting;
    using UnityEditor.Scripting.ScriptCompilation;
    using UnityEditorInternal;

    internal class SolutionSynchronizer
    {
        private static readonly Regex _MonoDevelopPropertyHeader;
        private readonly string _projectDirectory;
        private readonly string _projectName;
        private readonly ISolutionSynchronizationSettings _settings;
        [CompilerGenerated]
        private static Func<MonoIsland, bool> <>f__am$cache0;
        internal static readonly Dictionary<string, ScriptingLanguage> BuiltinSupportedExtensions;
        private static readonly string DefaultMonoDevelopSolutionProperties;
        public static readonly ISolutionSynchronizationSettings DefaultSynchronizationSettings = new DefaultSolutionSynchronizationSettings();
        public static readonly string MSBuildNamespaceUri;
        private static readonly Dictionary<ScriptingLanguage, string> ProjectExtensions;
        private string[] ProjectSupportedExtensions;
        public static readonly Regex scriptReferenceExpression;
        private static readonly string WindowsNewline = "\r\n";

        static SolutionSynchronizer()
        {
            Dictionary<string, ScriptingLanguage> dictionary = new Dictionary<string, ScriptingLanguage> {
                { 
                    "cs",
                    ScriptingLanguage.CSharp
                },
                { 
                    "js",
                    ScriptingLanguage.UnityScript
                },
                { 
                    "boo",
                    ScriptingLanguage.Boo
                },
                { 
                    "shader",
                    ScriptingLanguage.None
                },
                { 
                    "compute",
                    ScriptingLanguage.None
                },
                { 
                    "cginc",
                    ScriptingLanguage.None
                },
                { 
                    "hlsl",
                    ScriptingLanguage.None
                },
                { 
                    "glslinc",
                    ScriptingLanguage.None
                }
            };
            BuiltinSupportedExtensions = dictionary;
            Dictionary<ScriptingLanguage, string> dictionary2 = new Dictionary<ScriptingLanguage, string> {
                { 
                    ScriptingLanguage.Boo,
                    ".booproj"
                },
                { 
                    ScriptingLanguage.CSharp,
                    ".csproj"
                },
                { 
                    ScriptingLanguage.UnityScript,
                    ".unityproj"
                },
                { 
                    ScriptingLanguage.None,
                    ".csproj"
                }
            };
            ProjectExtensions = dictionary2;
            _MonoDevelopPropertyHeader = new Regex(@"^\s*GlobalSection\(MonoDevelopProperties.*\)");
            MSBuildNamespaceUri = "http://schemas.microsoft.com/developer/msbuild/2003";
            string[] textArray1 = new string[] { "    GlobalSection(MonoDevelopProperties) = preSolution", "        StartupItem = Assembly-CSharp.csproj", "    EndGlobalSection" };
            DefaultMonoDevelopSolutionProperties = string.Join("\r\n", textArray1).Replace("    ", "\t");
            scriptReferenceExpression = new Regex(@"^Library.ScriptAssemblies.(?<dllname>(?<project>.*)\.dll$)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public SolutionSynchronizer(string projectDirectory) : this(projectDirectory, DefaultSynchronizationSettings)
        {
        }

        public SolutionSynchronizer(string projectDirectory, ISolutionSynchronizationSettings settings)
        {
            this.ProjectSupportedExtensions = new string[0];
            this._projectDirectory = projectDirectory;
            this._settings = settings;
            this._projectName = Path.GetFileName(this._projectDirectory);
        }

        private static void DumpIsland(MonoIsland island)
        {
            Console.WriteLine("{0} ({1})", island._output, island._api_compatibility_level);
            Console.WriteLine("Files: ");
            Console.WriteLine(string.Join("\n", island._files));
            Console.WriteLine("References: ");
            Console.WriteLine(string.Join("\n", island._references));
            Console.WriteLine("");
        }

        private string EscapedRelativePathFor(string file)
        {
            string str = this._projectDirectory.Replace("/", @"\");
            file = file.Replace("/", @"\");
            return SecurityElement.Escape(!file.StartsWith(str) ? file : file.Substring(this._projectDirectory.Length + 1));
        }

        private string GenerateAllAssetProjectPart()
        {
            StringBuilder builder = new StringBuilder();
            foreach (string str in AssetDatabase.GetAllAssetPaths())
            {
                string extension = Path.GetExtension(str);
                if (this.IsSupportedExtension(extension) && (ScriptingLanguageFor(extension) == ScriptingLanguage.None))
                {
                    builder.AppendFormat("     <None Include=\"{0}\" />{1}", this.EscapedRelativePathFor(str), WindowsNewline);
                }
            }
            return builder.ToString();
        }

        private string GetProjectActiveConfigurations(string projectGuid) => 
            string.Format(DefaultSynchronizationSettings.SolutionProjectConfigurationTemplate, projectGuid);

        private string GetProjectEntries(IEnumerable<MonoIsland> islands)
        {
            IEnumerable<string> source = from i in islands select string.Format(DefaultSynchronizationSettings.SolutionProjectEntryTemplate, new object[] { this.SolutionGuid(i), this._projectName, Path.GetFileName(this.ProjectFile(i)), this.ProjectGuid(i._output) });
            return string.Join(WindowsNewline, source.ToArray<string>());
        }

        public static string GetProjectExtension(ScriptingLanguage language)
        {
            if (!ProjectExtensions.ContainsKey(language))
            {
                throw new ArgumentException("Unsupported language", "language");
            }
            return ProjectExtensions[language];
        }

        private static bool IsAdditionalInternalAssemblyReference(bool isBuildingEditorProject, string reference) => 
            (isBuildingEditorProject && ModuleUtils.GetAdditionalReferencesForEditorCsharpProject().Contains<string>(reference));

        [Obsolete("Use AssemblyHelper.IsManagedAssembly")]
        public static bool IsManagedAssembly(string file) => 
            AssemblyHelper.IsManagedAssembly(file);

        private bool IsSupportedExtension(string extension)
        {
            char[] trimChars = new char[] { '.' };
            extension = extension.TrimStart(trimChars);
            return (BuiltinSupportedExtensions.ContainsKey(extension) || this.ProjectSupportedExtensions.Contains<string>(extension));
        }

        private static Mode ModeForCurrentExternalEditor()
        {
            switch (InternalEditorUtility.GetScriptEditorFromPreferences())
            {
                case InternalEditorUtility.ScriptEditor.VisualStudio:
                case InternalEditorUtility.ScriptEditor.VisualStudioExpress:
                case InternalEditorUtility.ScriptEditor.VisualStudioCode:
                    return Mode.UnityScriptAsPrecompiledAssembly;

                case InternalEditorUtility.ScriptEditor.Internal:
                    return Mode.UnityScriptAsUnityProj;
            }
            return (!EditorPrefs.GetBool("kExternalEditorSupportsUnityProj", false) ? Mode.UnityScriptAsPrecompiledAssembly : Mode.UnityScriptAsUnityProj);
        }

        public bool ProjectExists(MonoIsland island) => 
            File.Exists(this.ProjectFile(island));

        public string ProjectFile(MonoIsland island)
        {
            ScriptingLanguage language = ScriptingLanguageFor(island);
            return Path.Combine(this._projectDirectory, $"{Path.GetFileNameWithoutExtension(island._output)}{ProjectExtensions[language]}");
        }

        private string ProjectFooter(MonoIsland island) => 
            string.Format(this._settings.GetProjectFooterTemplate(ScriptingLanguageFor(island)), this.ReadExistingMonoDevelopProjectProperties(island));

        private string ProjectGuid(string assembly) => 
            SolutionGuidGenerator.GuidForProject(this._projectName + Path.GetFileNameWithoutExtension(assembly));

        private string ProjectHeader(MonoIsland island)
        {
            string str3;
            string str = "4.0";
            string str2 = "10.0.20506";
            ScriptingLanguage language = ScriptingLanguageFor(island);
            if (this._settings.VisualStudioVersion == 9)
            {
                str = "3.5";
                str2 = "9.0.21022";
            }
            object[] objArray1 = new object[9];
            objArray1[0] = str;
            objArray1[1] = str2;
            objArray1[2] = this.ProjectGuid(island._output);
            objArray1[3] = this._settings.EngineAssemblyPath;
            objArray1[4] = this._settings.EditorAssemblyPath;
            string[] first = new string[] { "DEBUG", "TRACE" };
            objArray1[5] = string.Join(";", first.Concat<string>(this._settings.Defines).Concat<string>(island._defines).Distinct<string>().ToArray<string>());
            objArray1[6] = MSBuildNamespaceUri;
            objArray1[7] = Path.GetFileNameWithoutExtension(island._output);
            objArray1[8] = EditorSettings.projectGenerationRootNamespace;
            object[] args = objArray1;
            try
            {
                str3 = string.Format(this._settings.GetProjectHeaderTemplate(language), args);
            }
            catch (Exception)
            {
                throw new NotSupportedException("Failed creating c# project because the c# project header did not have the correct amount of arguments, which is " + args.Length);
            }
            return str3;
        }

        private string ProjectText(MonoIsland island, Mode mode, string allAssetsProject)
        {
            StringBuilder builder = new StringBuilder(this.ProjectHeader(island));
            List<string> first = new List<string>();
            List<Match> list2 = new List<Match>();
            bool isBuildingEditorProject = island._output.EndsWith("-Editor.dll");
            foreach (string str3 in island._files)
            {
                string str = Path.GetExtension(str3).ToLower();
                string file = !Path.IsPathRooted(str3) ? Path.Combine(this._projectDirectory, str3) : str3;
                if (".dll" != str)
                {
                    string str4 = "Compile";
                    builder.AppendFormat("     <{0} Include=\"{1}\" />{2}", str4, this.EscapedRelativePathFor(file), WindowsNewline);
                }
                else
                {
                    first.Add(file);
                }
            }
            builder.Append(allAssetsProject);
            List<string> list3 = new List<string>();
            foreach (string str5 in first.Union<string>(island._references))
            {
                if ((!str5.EndsWith("/UnityEditor.dll") && !str5.EndsWith("/UnityEngine.dll")) && (!str5.EndsWith(@"\UnityEditor.dll") && !str5.EndsWith(@"\UnityEngine.dll")))
                {
                    Match item = scriptReferenceExpression.Match(str5);
                    if (item.Success)
                    {
                        EditorBuildRules.TargetAssembly targetAssemblyDetails = EditorCompilationInterface.GetTargetAssemblyDetails(item.Groups["dllname"].Value);
                        ScriptingLanguage none = ScriptingLanguage.None;
                        if (targetAssemblyDetails != null)
                        {
                            none = (ScriptingLanguage) Enum.Parse(typeof(ScriptingLanguage), targetAssemblyDetails.Language.GetLanguageName(), true);
                        }
                        if ((mode == Mode.UnityScriptAsUnityProj) || (none == ScriptingLanguage.CSharp))
                        {
                            list2.Add(item);
                            continue;
                        }
                    }
                    string str6 = !Path.IsPathRooted(str5) ? Path.Combine(this._projectDirectory, str5) : str5;
                    if (AssemblyHelper.IsManagedAssembly(str6))
                    {
                        if (AssemblyHelper.IsInternalAssembly(str6))
                        {
                            if (!IsAdditionalInternalAssemblyReference(isBuildingEditorProject, str6))
                            {
                                continue;
                            }
                            string fileName = Path.GetFileName(str6);
                            if (list3.Contains(fileName))
                            {
                                continue;
                            }
                            list3.Add(fileName);
                        }
                        str6 = str6.Replace(@"\", "/").Replace(@"\\", "/");
                        builder.AppendFormat(" <Reference Include=\"{0}\">{1}", Path.GetFileNameWithoutExtension(str6), WindowsNewline);
                        builder.AppendFormat(" <HintPath>{0}</HintPath>{1}", str6, WindowsNewline);
                        builder.AppendFormat(" </Reference>{0}", WindowsNewline);
                    }
                }
            }
            if (0 < list2.Count)
            {
                builder.AppendLine("  </ItemGroup>");
                builder.AppendLine("  <ItemGroup>");
                foreach (Match match2 in list2)
                {
                    EditorBuildRules.TargetAssembly assembly2 = EditorCompilationInterface.GetTargetAssemblyDetails(match2.Groups["dllname"].Value);
                    ScriptingLanguage language = ScriptingLanguage.None;
                    if (assembly2 != null)
                    {
                        language = (ScriptingLanguage) Enum.Parse(typeof(ScriptingLanguage), assembly2.Language.GetLanguageName(), true);
                    }
                    string str8 = match2.Groups["project"].Value;
                    builder.AppendFormat("    <ProjectReference Include=\"{0}{1}\">{2}", str8, GetProjectExtension(language), WindowsNewline);
                    builder.AppendFormat("      <Project>{{{0}}}</Project>", this.ProjectGuid(Path.Combine("Temp", match2.Groups["project"].Value + ".dll")), WindowsNewline);
                    builder.AppendFormat("      <Name>{0}</Name>", str8, WindowsNewline);
                    builder.AppendLine("    </ProjectReference>");
                }
            }
            builder.Append(this.ProjectFooter(island));
            return builder.ToString();
        }

        private string ReadExistingMonoDevelopProjectProperties(MonoIsland island)
        {
            XmlNamespaceManager manager;
            if (!this.ProjectExists(island))
            {
                return string.Empty;
            }
            XmlDocument document = new XmlDocument();
            try
            {
                document.Load(this.ProjectFile(island));
                manager = new XmlNamespaceManager(document.NameTable);
                manager.AddNamespace("msb", MSBuildNamespaceUri);
            }
            catch (Exception exception)
            {
                if (!(exception is IOException) && !(exception is XmlException))
                {
                    throw;
                }
                return string.Empty;
            }
            XmlNodeList list = document.SelectNodes("/msb:Project/msb:ProjectExtensions", manager);
            if (list.Count == 0)
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder();
            IEnumerator enumerator = list.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    XmlNode current = (XmlNode) enumerator.Current;
                    builder.AppendLine(current.OuterXml);
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
            return builder.ToString();
        }

        private string ReadExistingMonoDevelopSolutionProperties()
        {
            if (this.SolutionExists())
            {
                string[] strArray;
                try
                {
                    strArray = File.ReadAllLines(this.SolutionFile());
                }
                catch (IOException)
                {
                    return DefaultMonoDevelopSolutionProperties;
                }
                StringBuilder builder = new StringBuilder();
                bool flag = false;
                foreach (string str2 in strArray)
                {
                    if (_MonoDevelopPropertyHeader.IsMatch(str2))
                    {
                        flag = true;
                    }
                    if (flag)
                    {
                        if (str2.Contains("EndGlobalSection"))
                        {
                            builder.Append(str2);
                            flag = false;
                        }
                        else
                        {
                            builder.AppendFormat("{0}{1}", str2, WindowsNewline);
                        }
                    }
                }
                if (0 < builder.Length)
                {
                    return builder.ToString();
                }
            }
            return DefaultMonoDevelopSolutionProperties;
        }

        private static IEnumerable<MonoIsland> RelevantIslandsForMode(IEnumerable<MonoIsland> islands, Mode mode)
        {
            <RelevantIslandsForMode>c__AnonStorey0 storey = new <RelevantIslandsForMode>c__AnonStorey0 {
                mode = mode
            };
            return Enumerable.Where<MonoIsland>(islands, new Func<MonoIsland, bool>(storey.<>m__0));
        }

        private static ScriptingLanguage ScriptingLanguageFor(string extension)
        {
            ScriptingLanguage language;
            char[] trimChars = new char[] { '.' };
            if (BuiltinSupportedExtensions.TryGetValue(extension.TrimStart(trimChars), out language))
            {
                return language;
            }
            return ScriptingLanguage.None;
        }

        private static ScriptingLanguage ScriptingLanguageFor(MonoIsland island) => 
            ScriptingLanguageFor(island.GetExtensionOfSourceFiles());

        private void SetupProjectSupportedExtensions()
        {
            this.ProjectSupportedExtensions = EditorSettings.projectGenerationUserExtensions;
        }

        public bool ShouldFileBePartOfSolution(string file)
        {
            string extension = Path.GetExtension(file);
            return ((extension == ".dll") || this.IsSupportedExtension(extension));
        }

        public bool SolutionExists() => 
            File.Exists(this.SolutionFile());

        internal string SolutionFile() => 
            Path.Combine(this._projectDirectory, $"{this._projectName}.sln");

        private string SolutionGuid(MonoIsland island) => 
            SolutionGuidGenerator.GuidForSolution(this._projectName, island.GetExtensionOfSourceFiles());

        private string SolutionText(IEnumerable<MonoIsland> islands, Mode mode)
        {
            string str = "11.00";
            string str2 = "2010";
            if (this._settings.VisualStudioVersion == 9)
            {
                str = "10.00";
                str2 = "2008";
            }
            IEnumerable<MonoIsland> enumerable = RelevantIslandsForMode(islands, mode);
            string projectEntries = this.GetProjectEntries(enumerable);
            string str4 = string.Join(WindowsNewline, (from i in enumerable select this.GetProjectActiveConfigurations(this.ProjectGuid(i._output))).ToArray<string>());
            return string.Format(this._settings.SolutionTemplate, new object[] { str, str2, projectEntries, str4, this.ReadExistingMonoDevelopSolutionProperties() });
        }

        public void Sync()
        {
            this.SetupProjectSupportedExtensions();
            if (!AssetPostprocessingInternal.OnPreGeneratingCSProjectFiles())
            {
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = i => 0 < i._files.Length;
                }
                IEnumerable<MonoIsland> islands = Enumerable.Where<MonoIsland>(InternalEditorUtility.GetMonoIslands(), <>f__am$cache0);
                string otherAssetsProjectPart = this.GenerateAllAssetProjectPart();
                this.SyncSolution(islands);
                foreach (MonoIsland island in RelevantIslandsForMode(islands, ModeForCurrentExternalEditor()))
                {
                    this.SyncProject(island, otherAssetsProjectPart);
                }
                if (InternalEditorUtility.GetScriptEditorFromPreferences() == InternalEditorUtility.ScriptEditor.VisualStudioCode)
                {
                    this.WriteVSCodeSettingsFiles();
                }
            }
            AssetPostprocessingInternal.CallOnGeneratedCSProjectFiles();
        }

        private static void SyncFileIfNotChanged(string filename, string newContents)
        {
            if (!File.Exists(filename) || (newContents != File.ReadAllText(filename)))
            {
                File.WriteAllText(filename, newContents, Encoding.UTF8);
            }
        }

        public bool SyncIfNeeded(IEnumerable<string> affectedFiles)
        {
            this.SetupProjectSupportedExtensions();
            if (this.SolutionExists() && Enumerable.Any<string>(affectedFiles, new Func<string, bool>(this.ShouldFileBePartOfSolution)))
            {
                this.Sync();
                return true;
            }
            return false;
        }

        private void SyncProject(MonoIsland island, string otherAssetsProjectPart)
        {
            SyncFileIfNotChanged(this.ProjectFile(island), this.ProjectText(island, ModeForCurrentExternalEditor(), otherAssetsProjectPart));
        }

        private void SyncSolution(IEnumerable<MonoIsland> islands)
        {
            SyncFileIfNotChanged(this.SolutionFile(), this.SolutionText(islands, ModeForCurrentExternalEditor()));
        }

        private void WriteVSCodeSettingsFiles()
        {
            string path = Path.Combine(this._projectDirectory, ".vscode");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string str2 = Path.Combine(path, "settings.json");
            if (!File.Exists(str2))
            {
                File.WriteAllText(str2, VSCodeTemplates.SettingsJson);
            }
        }

        [CompilerGenerated]
        private sealed class <RelevantIslandsForMode>c__AnonStorey0
        {
            internal SolutionSynchronizer.Mode mode;

            internal bool <>m__0(MonoIsland i) => 
                ((this.mode == SolutionSynchronizer.Mode.UnityScriptAsUnityProj) || (ScriptingLanguage.CSharp == SolutionSynchronizer.ScriptingLanguageFor(i)));
        }

        private enum Mode
        {
            UnityScriptAsUnityProj,
            UnityScriptAsPrecompiledAssembly
        }
    }
}

