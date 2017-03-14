namespace UnityEditor.Scripting.ScriptCompilation
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEditor.Modules;
    using UnityEditor.Scripting;
    using UnityEditor.Scripting.Compilers;
    using UnityEditor.Scripting.Serialization;
    using UnityEditorInternal;
    using UnityEngine;

    internal class EditorCompilation
    {
        [CompilerGenerated]
        private static Func<CustomScriptAssembly, string> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<MonoIsland, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<CompilerMessage[], bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Comparison<AssemblyCompilerMessages> <>f__am$cache3;
        [CompilerGenerated]
        private static Comparison<AssemblyCompilerMessages> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<CompilerMessage, bool> <>f__am$cache5;
        private HashSet<string> allScripts = null;
        private bool areAllScriptsDirty;
        private string assemblySuffix = string.Empty;
        private static readonly string AssemblyTimestampPath = Path.Combine(EditorAssemblyPath, "BuiltinAssemblies.stamp");
        private CompilationTask compilationTask;
        private Dictionary<string, CompilerMessage[]> compilerMessages = new Dictionary<string, CompilerMessage[]>();
        private CustomScriptAssembly[] customScriptAssemblies;
        private EditorBuildRules.TargetAssembly[] customTargetAssemblies;
        private HashSet<string> dirtyScripts = new HashSet<string>();
        private static readonly string EditorAssemblyPath = Path.Combine("Library", "ScriptAssemblies");
        private static readonly string EditorTempPath = "Temp";
        private PrecompiledAssembly[] precompiledAssemblies;
        private string projectDirectory = string.Empty;
        private HashSet<string> runScriptUpdaterAssemblies = new HashSet<string>();
        private PrecompiledAssembly[] unityAssemblies;
        private bool weaverFailed;

        private string AssemblyFilenameWithSuffix(string assemblyFilename)
        {
            if (!string.IsNullOrEmpty(this.assemblySuffix))
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(assemblyFilename);
                string extension = Path.GetExtension(assemblyFilename);
                return (fileNameWithoutExtension + this.assemblySuffix + extension);
            }
            return assemblyFilename;
        }

        private static void CheckCyclicAssemblyReferences(CustomScriptAssembly[] customScriptAssemblies)
        {
            if ((customScriptAssemblies != null) && (customScriptAssemblies.Length >= 2))
            {
                Dictionary<string, CustomScriptAssembly> nameToCustomScriptAssembly = new Dictionary<string, CustomScriptAssembly>();
                foreach (CustomScriptAssembly assembly in customScriptAssemblies)
                {
                    nameToCustomScriptAssembly[assembly.Name] = assembly;
                }
                HashSet<CustomScriptAssembly> visited = new HashSet<CustomScriptAssembly>();
                foreach (CustomScriptAssembly assembly2 in customScriptAssemblies)
                {
                    CheckCyclicAssemblyReferencesDFS(assembly2, visited, nameToCustomScriptAssembly);
                }
            }
        }

        private static void CheckCyclicAssemblyReferencesDFS(CustomScriptAssembly visitAssembly, HashSet<CustomScriptAssembly> visited, IDictionary<string, CustomScriptAssembly> nameToCustomScriptAssembly)
        {
            if (visited.Contains(visitAssembly))
            {
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = a => $"'{a.Name}'";
                }
                throw new Exception($"Cyclic assembly references detected. Assemblies: {string.Join(", ", Enumerable.Select<CustomScriptAssembly, string>(visited, <>f__am$cache0).ToArray<string>())}");
            }
            visited.Add(visitAssembly);
            foreach (string str in visitAssembly.References)
            {
                CustomScriptAssembly assembly;
                if (!nameToCustomScriptAssembly.TryGetValue(str, out assembly))
                {
                    throw new Exception($"Reference to non-existent assembly. Assembly {visitAssembly.Name} has a reference to {str}");
                }
                CheckCyclicAssemblyReferencesDFS(assembly, visited, nameToCustomScriptAssembly);
            }
            visited.Remove(visitAssembly);
        }

        public void ClearCompileErrors()
        {
            this.compilerMessages.Clear();
            this.weaverFailed = false;
        }

        public bool CompileScripts(EditorScriptCompilationOptions definesOptions, BuildTargetGroup platformGroup, BuildTarget platform)
        {
            ScriptAssemblySettings scriptAssemblySettings = this.CreateScriptAssemblySettings(platformGroup, platform, definesOptions);
            BuildFlags none = BuildFlags.None;
            if ((definesOptions & EditorScriptCompilationOptions.BuildingForEditor) == EditorScriptCompilationOptions.BuildingForEditor)
            {
                none |= BuildFlags.BuildingForEditor;
            }
            if ((definesOptions & EditorScriptCompilationOptions.BuildingDevelopmentBuild) == EditorScriptCompilationOptions.BuildingDevelopmentBuild)
            {
                none |= BuildFlags.BuildingDevelopmentBuild;
            }
            string[] notCompiledSourceFiles = null;
            bool flag = this.CompileScripts(scriptAssemblySettings, EditorTempPath, none, ref notCompiledSourceFiles);
            if (notCompiledSourceFiles != null)
            {
                foreach (string str in notCompiledSourceFiles)
                {
                    string filePath = this.FindCustomScriptAssembly(str).FilePath;
                    if (filePath.StartsWith(this.projectDirectory))
                    {
                        filePath = filePath.Substring(this.projectDirectory.Length);
                    }
                    Debug.LogWarning($"Script file '{str}' not included in compilation. Assembly definition file '{filePath}' language does not match the script file extension.");
                }
            }
            return flag;
        }

        internal bool CompileScripts(ScriptAssemblySettings scriptAssemblySettings, string tempBuildDirectory, BuildFlags buildflags, ref string[] notCompiledSourceFiles)
        {
            <CompileScripts>c__AnonStorey3 storey = new <CompileScripts>c__AnonStorey3 {
                scriptAssemblySettings = scriptAssemblySettings,
                tempBuildDirectory = tempBuildDirectory,
                buildflags = buildflags,
                $this = this
            };
            this.DeleteUnusedAssemblies();
            this.allScripts.RemoveWhere(new Predicate<string>(storey.<>m__0));
            this.StopAllCompilation();
            this.weaverFailed = false;
            if (!Directory.Exists(storey.scriptAssemblySettings.OutputDirectory))
            {
                Directory.CreateDirectory(storey.scriptAssemblySettings.OutputDirectory);
            }
            if (!Directory.Exists(storey.tempBuildDirectory))
            {
                Directory.CreateDirectory(storey.tempBuildDirectory);
            }
            IEnumerable<string> source = !this.areAllScriptsDirty ? ((IEnumerable<string>) this.dirtyScripts.ToArray<string>()) : ((IEnumerable<string>) this.allScripts.ToArray<string>());
            this.areAllScriptsDirty = false;
            this.dirtyScripts.Clear();
            if (!source.Any<string>() && (this.runScriptUpdaterAssemblies.Count == 0))
            {
                return false;
            }
            EditorBuildRules.CompilationAssemblies assemblies = new EditorBuildRules.CompilationAssemblies {
                UnityAssemblies = this.unityAssemblies,
                PrecompiledAssemblies = this.precompiledAssemblies,
                CustomTargetAssemblies = this.customTargetAssemblies,
                EditorAssemblyReferences = ModuleUtils.GetAdditionalReferencesForUserScripts()
            };
            EditorBuildRules.GenerateChangedScriptAssembliesArgs args = new EditorBuildRules.GenerateChangedScriptAssembliesArgs {
                AllSourceFiles = this.allScripts,
                DirtySourceFiles = source,
                ProjectDirectory = this.projectDirectory,
                BuildFlags = storey.buildflags,
                Settings = storey.scriptAssemblySettings,
                Assemblies = assemblies,
                RunUpdaterAssemblies = this.runScriptUpdaterAssemblies
            };
            ScriptAssembly[] assemblyArray = EditorBuildRules.GenerateChangedScriptAssemblies(args);
            notCompiledSourceFiles = args.NotCompiledSourceFiles.ToArray<string>();
            if (!assemblyArray.Any<ScriptAssembly>())
            {
                return false;
            }
            this.compilationTask = new CompilationTask(assemblyArray, storey.tempBuildDirectory, storey.buildflags);
            this.compilationTask.OnCompilationStarted += new CompilationTask.OnCompilationStartedDelegate(storey.<>m__1);
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = i => 0 < i._files.Length;
            }
            storey.compilingMonoIslands = Enumerable.Where<MonoIsland>(this.GetAllMonoIslands(), <>f__am$cache1);
            this.compilationTask.OnCompilationFinished += new CompilationTask.OnCompilationFinishedDelegate(storey.<>m__2);
            this.compilationTask.Poll();
            return true;
        }

        private static void CopyAssembly(string sourcePath, string destinationPath)
        {
            if (MoveOrReplaceFile(sourcePath, destinationPath))
            {
                string path = MDBPath(sourcePath);
                string str2 = MDBPath(destinationPath);
                if (File.Exists(path))
                {
                    MoveOrReplaceFile(path, str2);
                }
                else if (File.Exists(str2))
                {
                    File.Delete(str2);
                }
                string str3 = PDBPath(sourcePath);
                string str4 = PDBPath(destinationPath);
                if (File.Exists(str3))
                {
                    MoveOrReplaceFile(str3, str4);
                }
                else if (File.Exists(str4))
                {
                    File.Delete(str4);
                }
            }
        }

        private ScriptAssemblySettings CreateEditorScriptAssemblySettings(EditorScriptCompilationOptions defines) => 
            this.CreateScriptAssemblySettings(EditorUserBuildSettings.activeBuildTargetGroup, EditorUserBuildSettings.activeBuildTarget, defines);

        private ScriptAssemblySettings CreateScriptAssemblySettings(BuildTargetGroup buildTargetGroup, BuildTarget buildTarget, EditorScriptCompilationOptions definesOptions)
        {
            string[] strArray = InternalEditorUtility.GetCompilationDefines(definesOptions, buildTargetGroup, buildTarget);
            return new ScriptAssemblySettings { 
                BuildTarget = buildTarget,
                BuildTargetGroup = buildTargetGroup,
                OutputDirectory = EditorAssemblyPath,
                Defines = strArray,
                ApiCompatibilityLevel = PlayerSettings.GetApiCompatibilityLevel(buildTargetGroup),
                FilenameSuffix = this.assemblySuffix
            };
        }

        public void DeleteUnusedAssemblies()
        {
            string path = Path.Combine(Path.GetDirectoryName(Application.dataPath), EditorAssemblyPath);
            if (Directory.Exists(path))
            {
                List<string> list = new List<string>(Directory.GetFiles(path));
                list.Remove(Path.Combine(Path.GetDirectoryName(Application.dataPath), AssemblyTimestampPath));
                ScriptAssembly[] allScriptAssemblies = this.GetAllScriptAssemblies(BuildFlags.BuildingForEditor, EditorScriptCompilationOptions.BuildingForEditor);
                foreach (ScriptAssembly assembly in allScriptAssemblies)
                {
                    if (assembly.Files.Length > 0)
                    {
                        string item = Path.Combine(path, assembly.Filename);
                        list.Remove(item);
                        list.Remove(MDBPath(item));
                        list.Remove(PDBPath(item));
                    }
                }
                foreach (string str3 in list)
                {
                    File.Delete(str3);
                }
            }
        }

        public void DirtyAllScripts()
        {
            this.areAllScriptsDirty = true;
        }

        public void DirtyScript(string path)
        {
            this.allScripts.Add(path);
            this.dirtyScripts.Add(path);
        }

        public bool DoesProjectFolderHaveAnyDirtyScripts()
        {
            if (this.dirtyScripts.Count > 0)
            {
                return true;
            }
            if (!this.areAllScriptsDirty)
            {
                return false;
            }
            this.allScripts.RemoveWhere(path => !File.Exists(Path.Combine(this.projectDirectory, path)));
            return (this.allScripts.Count > 0);
        }

        public bool DoesProjectFolderHaveAnyScripts() => 
            ((this.allScripts != null) && (this.allScripts.Count > 0));

        internal CustomScriptAssembly FindCustomScriptAssembly(string scriptPath)
        {
            <FindCustomScriptAssembly>c__AnonStorey2 storey = new <FindCustomScriptAssembly>c__AnonStorey2();
            EditorBuildRules.TargetAssembly candidateAssembly = null;
            EditorBuildRules.TargetAssembly assembly2 = EditorBuildRules.GetCustomTargetAssembly(scriptPath, this.projectDirectory, this.customTargetAssemblies, ref candidateAssembly);
            storey.assembly = (assembly2 == null) ? candidateAssembly : assembly2;
            return Enumerable.Single<CustomScriptAssembly>(this.customScriptAssemblies, new Func<CustomScriptAssembly, bool>(storey.<>m__0));
        }

        public AssemblyCompilerMessages[] GetAllAssemblyCompilerMessages()
        {
            AssemblyCompilerMessages[] array = new AssemblyCompilerMessages[this.compilerMessages.Count];
            int num = 0;
            foreach (KeyValuePair<string, CompilerMessage[]> pair in this.compilerMessages)
            {
                string key = pair.Key;
                CompilerMessage[] messageArray = pair.Value;
                AssemblyCompilerMessages messages = new AssemblyCompilerMessages {
                    assemblyFilename = key,
                    messages = messageArray
                };
                array[num++] = messages;
            }
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = (m1, m2) => string.Compare(m1.assemblyFilename, m2.assemblyFilename);
            }
            Array.Sort<AssemblyCompilerMessages>(array, <>f__am$cache4);
            return array;
        }

        public MonoIsland[] GetAllMonoIslands()
        {
            ScriptAssembly[] allScriptAssemblies = this.GetAllScriptAssemblies(BuildFlags.BuildingForEditor, EditorScriptCompilationOptions.BuildingEmpty);
            MonoIsland[] islandArray = new MonoIsland[allScriptAssemblies.Length];
            for (int i = 0; i < allScriptAssemblies.Length; i++)
            {
                islandArray[i] = allScriptAssemblies[i].ToMonoIsland(BuildFlags.BuildingForEditor, EditorTempPath);
            }
            return islandArray;
        }

        private ScriptAssembly[] GetAllScriptAssemblies(BuildFlags buildFlags, EditorScriptCompilationOptions options)
        {
            EditorBuildRules.CompilationAssemblies assemblies = new EditorBuildRules.CompilationAssemblies {
                UnityAssemblies = this.unityAssemblies,
                PrecompiledAssemblies = this.precompiledAssemblies,
                CustomTargetAssemblies = this.customTargetAssemblies,
                EditorAssemblyReferences = ModuleUtils.GetAdditionalReferencesForUserScripts()
            };
            ScriptAssemblySettings settings = this.CreateEditorScriptAssemblySettings(options);
            return EditorBuildRules.GetAllScriptAssemblies(this.allScripts, this.projectDirectory, buildFlags, settings, assemblies);
        }

        public AssemblyCompilerMessages[] GetLastAssemblyCompilerMessages()
        {
            if (this.compilationTask == null)
            {
                return null;
            }
            AssemblyCompilerMessages[] array = new AssemblyCompilerMessages[this.compilationTask.CompilerMessages.Count];
            int num = 0;
            foreach (KeyValuePair<ScriptAssembly, CompilerMessage[]> pair in this.compilationTask.CompilerMessages)
            {
                ScriptAssembly key = pair.Key;
                CompilerMessage[] messageArray = pair.Value;
                AssemblyCompilerMessages messages = new AssemblyCompilerMessages {
                    assemblyFilename = key.Filename,
                    messages = messageArray
                };
                array[num++] = messages;
            }
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = (m1, m2) => string.Compare(m1.assemblyFilename, m2.assemblyFilename);
            }
            Array.Sort<AssemblyCompilerMessages>(array, <>f__am$cache3);
            return array;
        }

        public TargetAssemblyInfo[] GetTargetAssemblies()
        {
            EditorBuildRules.TargetAssembly[] predefinedTargetAssemblies = EditorBuildRules.GetPredefinedTargetAssemblies();
            TargetAssemblyInfo[] infoArray = new TargetAssemblyInfo[predefinedTargetAssemblies.Length + ((this.customTargetAssemblies == null) ? 0 : this.customTargetAssemblies.Count<EditorBuildRules.TargetAssembly>())];
            for (int i = 0; i < predefinedTargetAssemblies.Length; i++)
            {
                infoArray[i].Name = this.AssemblyFilenameWithSuffix(predefinedTargetAssemblies[i].Filename);
                infoArray[i].Flags = predefinedTargetAssemblies[i].Flags;
            }
            if (this.customTargetAssemblies != null)
            {
                for (int j = 0; j < this.customTargetAssemblies.Count<EditorBuildRules.TargetAssembly>(); j++)
                {
                    int index = predefinedTargetAssemblies.Length + j;
                    infoArray[index].Name = this.AssemblyFilenameWithSuffix(this.customTargetAssemblies[j].Filename);
                    infoArray[index].Flags = this.customTargetAssemblies[j].Flags;
                }
            }
            return infoArray;
        }

        public TargetAssemblyInfo GetTargetAssembly(string scriptPath)
        {
            TargetAssemblyInfo info;
            EditorBuildRules.TargetAssembly assembly = EditorBuildRules.GetTargetAssembly(scriptPath, this.projectDirectory, this.customTargetAssemblies);
            info.Name = this.AssemblyFilenameWithSuffix(assembly.Filename);
            info.Flags = assembly.Flags;
            return info;
        }

        public EditorBuildRules.TargetAssembly GetTargetAssemblyDetails(string scriptPath) => 
            EditorBuildRules.GetTargetAssembly(scriptPath, this.projectDirectory, this.customTargetAssemblies);

        public bool HaveCompileErrors()
        {
            if (!this.weaverFailed)
            {
            }
            return ((<>f__am$cache2 != null) || Enumerable.Any<CompilerMessage[]>(this.compilerMessages.Values, <>f__am$cache2));
        }

        public bool IsCompilationPending() => 
            (this.DoesProjectFolderHaveAnyDirtyScripts() || (this.runScriptUpdaterAssemblies.Count<string>() > 0));

        private bool IsCompilationTaskCompiling() => 
            ((this.compilationTask != null) && this.compilationTask.IsCompiling);

        public bool IsCompiling() => 
            (this.IsCompilationTaskCompiling() || this.IsCompilationPending());

        public bool IsExtensionSupportedByCompiler(string extension)
        {
            <IsExtensionSupportedByCompiler>c__AnonStorey0 storey = new <IsExtensionSupportedByCompiler>c__AnonStorey0 {
                extension = extension
            };
            return (Enumerable.Count<SupportedLanguage>(ScriptCompilers.SupportedLanguages, new Func<SupportedLanguage, bool>(storey.<>m__0)) > 0);
        }

        private static CustomScriptAssembly LoadCustomScriptAssemblyFromJson(string path)
        {
            CustomScriptAssemblyData customScriptAssemblyData = CustomScriptAssemblyData.FromJson(File.ReadAllText(path));
            return CustomScriptAssembly.FromCustomScriptAssemblyData(path, customScriptAssemblyData);
        }

        private static string MDBPath(string dllPath) => 
            (dllPath + ".mdb");

        private static bool MoveOrReplaceFile(string sourcePath, string destinationPath)
        {
            bool flag = true;
            try
            {
                File.Move(sourcePath, destinationPath);
            }
            catch (IOException)
            {
                flag = false;
            }
            if (!flag)
            {
                flag = true;
                try
                {
                    File.Replace(sourcePath, destinationPath, null);
                }
                catch (IOException)
                {
                    flag = false;
                }
            }
            return flag;
        }

        private static string PDBPath(string dllPath) => 
            dllPath.Replace(".dll", ".pdb");

        public void RunScriptUpdaterOnAssembly(string assemblyFilename)
        {
            this.runScriptUpdaterAssemblies.Add(assemblyFilename);
        }

        public void SetAllCustomScriptAssemblyJsons(string[] paths)
        {
            List<CustomScriptAssembly> list = new List<CustomScriptAssembly>();
            foreach (string str in paths)
            {
                try
                {
                    <SetAllCustomScriptAssemblyJsons>c__AnonStorey1 storey = new <SetAllCustomScriptAssemblyJsons>c__AnonStorey1();
                    string path = !Path.IsPathRooted(str) ? Path.Combine(this.projectDirectory, str) : str;
                    storey.loadedCustomScriptAssembly = LoadCustomScriptAssemblyFromJson(path);
                    if (Enumerable.Any<CustomScriptAssembly>(list, new Func<CustomScriptAssembly, bool>(storey.<>m__0)))
                    {
                        throw new Exception($"Assembly with name '{storey.loadedCustomScriptAssembly.Name.Length}' is already defined ({storey.loadedCustomScriptAssembly.FilePath})");
                    }
                    if (storey.loadedCustomScriptAssembly.References == null)
                    {
                        storey.loadedCustomScriptAssembly.References = new string[0];
                    }
                    if (storey.loadedCustomScriptAssembly.References.Length != storey.loadedCustomScriptAssembly.References.Distinct<string>().Count<string>())
                    {
                        throw new Exception($"Duplicate assembly references in {storey.loadedCustomScriptAssembly.FilePath}");
                    }
                    list.Add(storey.loadedCustomScriptAssembly);
                }
                catch (Exception exception)
                {
                    throw new Exception(exception.Message + " - '" + str + "'");
                }
            }
            this.customScriptAssemblies = list.ToArray();
            try
            {
                CheckCyclicAssemblyReferences(this.customScriptAssemblies);
            }
            catch (Exception exception2)
            {
                this.customScriptAssemblies = null;
                this.customTargetAssemblies = null;
                throw exception2;
            }
            this.customTargetAssemblies = EditorBuildRules.CreateTargetAssemblies(this.customScriptAssemblies);
        }

        public void SetAllPrecompiledAssemblies(PrecompiledAssembly[] precompiledAssemblies)
        {
            this.precompiledAssemblies = precompiledAssemblies;
        }

        public void SetAllScripts(string[] allScripts)
        {
            this.allScripts = new HashSet<string>(allScripts);
        }

        public void SetAllUnityAssemblies(PrecompiledAssembly[] unityAssemblies)
        {
            this.unityAssemblies = unityAssemblies;
        }

        internal void SetAssemblySuffix(string assemblySuffix)
        {
            this.assemblySuffix = assemblySuffix;
        }

        internal void SetProjectDirectory(string projectDirectory)
        {
            this.projectDirectory = projectDirectory;
        }

        public void StopAllCompilation()
        {
            if (this.compilationTask != null)
            {
                this.compilationTask.Stop();
                this.compilationTask = null;
            }
        }

        public CompileStatus TickCompilationPipeline()
        {
            if ((!this.IsCompilationTaskCompiling() && this.IsCompilationPending()) && this.CompileScripts(EditorScriptCompilationOptions.BuildingForEditor, EditorUserBuildSettings.activeBuildTargetGroup, EditorUserBuildSettings.activeBuildTarget))
            {
                return CompileStatus.CompilationStarted;
            }
            if (this.IsCompilationTaskCompiling())
            {
                if (this.compilationTask.Poll())
                {
                    return (!this.compilationTask.CompileErrors ? CompileStatus.CompilationComplete : CompileStatus.CompilationFailed);
                }
                return CompileStatus.Compiling;
            }
            return CompileStatus.Idle;
        }

        [CompilerGenerated]
        private sealed class <CompileScripts>c__AnonStorey3
        {
            internal EditorCompilation $this;
            private static Func<CompilerMessage, bool> <>f__am$cache0;
            internal BuildFlags buildflags;
            internal IEnumerable<MonoIsland> compilingMonoIslands;
            internal ScriptAssemblySettings scriptAssemblySettings;
            internal string tempBuildDirectory;

            internal bool <>m__0(string path) => 
                !File.Exists(Path.Combine(this.$this.projectDirectory, path));

            internal void <>m__1(ScriptAssembly assembly, int phase)
            {
                Console.WriteLine("- Starting compile {0}", Path.Combine(this.scriptAssemblySettings.OutputDirectory, assembly.Filename));
            }

            internal void <>m__2(ScriptAssembly assembly, CompilerMessage[] messages)
            {
                Console.WriteLine("- Finished compile {0}", Path.Combine(this.scriptAssemblySettings.OutputDirectory, assembly.Filename));
                this.$this.compilerMessages[assembly.Filename] = messages;
                if (this.$this.runScriptUpdaterAssemblies.Contains(assembly.Filename))
                {
                    this.$this.runScriptUpdaterAssemblies.Remove(assembly.Filename);
                }
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = m => m.type == CompilerMessageType.Error;
                }
                if (!Enumerable.Any<CompilerMessage>(messages, <>f__am$cache0))
                {
                    string engineAssemblyPath = InternalEditorUtility.GetEngineAssemblyPath();
                    string unityUNet = EditorApplication.applicationContentsPath + "/UnityExtensions/Unity/Networking/UnityEngine.Networking.dll";
                    if (!Weaver.WeaveUnetFromEditor(this.compilingMonoIslands, Path.Combine(this.tempBuildDirectory, assembly.Filename), Path.Combine(EditorCompilation.EditorTempPath, assembly.Filename), engineAssemblyPath, unityUNet, (this.buildflags & BuildFlags.BuildingForEditor) != BuildFlags.None))
                    {
                        this.$this.StopAllCompilation();
                        this.$this.weaverFailed = true;
                    }
                    else
                    {
                        EditorCompilation.CopyAssembly(Path.Combine(this.tempBuildDirectory, assembly.Filename), assembly.FullPath);
                    }
                }
            }

            private static bool <>m__3(CompilerMessage m) => 
                (m.type == CompilerMessageType.Error);
        }

        [CompilerGenerated]
        private sealed class <FindCustomScriptAssembly>c__AnonStorey2
        {
            internal EditorBuildRules.TargetAssembly assembly;

            internal bool <>m__0(CustomScriptAssembly a) => 
                (a.Name == Path.GetFileNameWithoutExtension(this.assembly.Filename));
        }

        [CompilerGenerated]
        private sealed class <IsExtensionSupportedByCompiler>c__AnonStorey0
        {
            internal string extension;

            internal bool <>m__0(SupportedLanguage l) => 
                (l.GetExtensionICanCompile() == this.extension);
        }

        [CompilerGenerated]
        private sealed class <SetAllCustomScriptAssemblyJsons>c__AnonStorey1
        {
            internal CustomScriptAssembly loadedCustomScriptAssembly;

            internal bool <>m__0(CustomScriptAssembly a) => 
                string.Equals(a.Name, this.loadedCustomScriptAssembly.Name, StringComparison.OrdinalIgnoreCase);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct AssemblyCompilerMessages
        {
            public string assemblyFilename;
            public CompilerMessage[] messages;
        }

        public enum CompileStatus
        {
            Idle,
            Compiling,
            CompilationStarted,
            CompilationFailed,
            CompilationComplete
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TargetAssemblyInfo
        {
            public string Name;
            public AssemblyFlags Flags;
        }
    }
}

