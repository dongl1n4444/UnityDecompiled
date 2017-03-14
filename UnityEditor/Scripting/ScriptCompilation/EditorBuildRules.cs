namespace UnityEditor.Scripting.ScriptCompilation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.Scripting;
    using UnityEditor.Scripting.Compilers;
    using UnityEngine;

    internal static class EditorBuildRules
    {
        [CompilerGenerated]
        private static Func<KeyValuePair<TargetAssembly, HashSet<string>>, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<KeyValuePair<TargetAssembly, HashSet<string>>, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<KeyValuePair<TargetAssembly, HashSet<string>>, TargetAssembly> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<KeyValuePair<TargetAssembly, HashSet<string>>, HashSet<string>> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<string, int> <>f__mg$cache0;
        [CompilerGenerated]
        private static Func<string, int> <>f__mg$cache1;
        [CompilerGenerated]
        private static Func<string, int> <>f__mg$cache2;
        private static readonly TargetAssembly[] predefinedTargetAssemblies = CreatePredefinedTargetAssemblies();

        internal static void AddScriptAssemblyReferences(ref ScriptAssembly scriptAssembly, TargetAssembly targetAssembly, ScriptAssemblySettings settings, BuildFlags buildFlags, CompilationAssemblies assemblies, IDictionary<TargetAssembly, ScriptAssembly> targetToScriptAssembly, string filenameSuffix)
        {
            List<ScriptAssembly> list = new List<ScriptAssembly>();
            List<string> list2 = new List<string>();
            bool flag = (buildFlags & BuildFlags.BuildingForEditor) == BuildFlags.BuildingForEditor;
            bool flag2 = (targetAssembly.Flags & AssemblyFlags.EditorOnly) == AssemblyFlags.EditorOnly;
            if (assemblies.UnityAssemblies != null)
            {
                foreach (PrecompiledAssembly assembly in assemblies.UnityAssemblies)
                {
                    if (flag || flag2)
                    {
                        if ((assembly.Flags & AssemblyFlags.UseForMono) != AssemblyFlags.None)
                        {
                            list2.Add(assembly.Path);
                        }
                    }
                    else if (((assembly.Flags & AssemblyFlags.EditorOnly) != AssemblyFlags.EditorOnly) && IsCompiledAssemblyCompatibleWithTargetAssembly(assembly, targetAssembly, settings.BuildTarget, assemblies.CustomTargetAssemblies))
                    {
                        list2.Add(assembly.Path);
                    }
                }
            }
            foreach (TargetAssembly assembly2 in targetAssembly.References)
            {
                ScriptAssembly assembly3;
                if (targetToScriptAssembly.TryGetValue(assembly2, out assembly3))
                {
                    list.Add(assembly3);
                }
                else
                {
                    string path = Path.Combine(settings.OutputDirectory, assembly2.Filename);
                    if (!string.IsNullOrEmpty(filenameSuffix))
                    {
                        path = path.Replace(".dll", filenameSuffix + ".dll");
                    }
                    if (File.Exists(path))
                    {
                        list2.Add(path);
                    }
                }
            }
            if ((assemblies.CustomTargetAssemblies != null) && (targetAssembly.Type == TargetAssemblyType.Predefined))
            {
                foreach (TargetAssembly assembly4 in assemblies.CustomTargetAssemblies)
                {
                    ScriptAssembly assembly5;
                    if (targetToScriptAssembly.TryGetValue(assembly4, out assembly5))
                    {
                        list.Add(assembly5);
                    }
                }
            }
            if (assemblies.PrecompiledAssemblies != null)
            {
                foreach (PrecompiledAssembly assembly6 in assemblies.PrecompiledAssemblies)
                {
                    if ((((assembly6.Flags & AssemblyFlags.EditorOnly) != AssemblyFlags.EditorOnly) || flag2) && IsCompiledAssemblyCompatibleWithTargetAssembly(assembly6, targetAssembly, settings.BuildTarget, assemblies.CustomTargetAssemblies))
                    {
                        list2.Add(assembly6.Path);
                    }
                }
            }
            if (flag && (assemblies.EditorAssemblyReferences != null))
            {
                list2.AddRange(assemblies.EditorAssemblyReferences);
            }
            scriptAssembly.ScriptAssemblyReferences = list.ToArray();
            scriptAssembly.References = list2.ToArray();
        }

        public static PrecompiledAssembly CreateEditorCompiledAssembly(string path) => 
            new PrecompiledAssembly { 
                Path = path,
                Flags = AssemblyFlags.EditorOnly
            };

        internal static TargetAssembly[] CreatePredefinedTargetAssemblies()
        {
            List<TargetAssembly> collection = new List<TargetAssembly>();
            List<TargetAssembly> list2 = new List<TargetAssembly>();
            List<TargetAssembly> list3 = new List<TargetAssembly>();
            List<TargetAssembly> list4 = new List<TargetAssembly>();
            List<SupportedLanguage> supportedLanguages = ScriptCompilers.SupportedLanguages;
            List<TargetAssembly> list6 = new List<TargetAssembly>();
            foreach (SupportedLanguage language in supportedLanguages)
            {
                string languageName = language.GetLanguageName();
                if (<>f__mg$cache0 == null)
                {
                    <>f__mg$cache0 = new Func<string, int>(EditorBuildRules.FilterAssemblyInFirstpassFolder);
                }
                TargetAssembly item = new TargetAssembly("Assembly-" + languageName + "-firstpass.dll", language, AssemblyFlags.None, TargetAssemblyType.Predefined, <>f__mg$cache0);
                TargetAssembly assembly2 = new TargetAssembly("Assembly-" + languageName + ".dll", language, AssemblyFlags.None, TargetAssemblyType.Predefined);
                if (<>f__mg$cache1 == null)
                {
                    <>f__mg$cache1 = new Func<string, int>(EditorBuildRules.FilterAssemblyInFirstpassEditorFolder);
                }
                TargetAssembly assembly3 = new TargetAssembly("Assembly-" + languageName + "-Editor-firstpass.dll", language, AssemblyFlags.EditorOnly, TargetAssemblyType.Predefined, <>f__mg$cache1);
                if (<>f__mg$cache2 == null)
                {
                    <>f__mg$cache2 = new Func<string, int>(EditorBuildRules.FilterAssemblyInEditorFolder);
                }
                TargetAssembly assembly4 = new TargetAssembly("Assembly-" + languageName + "-Editor.dll", language, AssemblyFlags.EditorOnly, TargetAssemblyType.Predefined, <>f__mg$cache2);
                collection.Add(item);
                list2.Add(assembly2);
                list3.Add(assembly3);
                list4.Add(assembly4);
                list6.Add(item);
                list6.Add(assembly2);
                list6.Add(assembly3);
                list6.Add(assembly4);
            }
            foreach (TargetAssembly assembly5 in list2)
            {
                assembly5.References.AddRange(collection);
            }
            foreach (TargetAssembly assembly6 in list3)
            {
                assembly6.References.AddRange(collection);
            }
            foreach (TargetAssembly assembly7 in list4)
            {
                assembly7.References.AddRange(collection);
                assembly7.References.AddRange(list2);
                assembly7.References.AddRange(list3);
            }
            return list6.ToArray();
        }

        public static TargetAssembly[] CreateTargetAssemblies(IEnumerable<CustomScriptAssembly> customScriptAssemblies)
        {
            if (customScriptAssemblies == null)
            {
                return null;
            }
            List<TargetAssembly> list = new List<TargetAssembly>();
            Dictionary<string, TargetAssembly> dictionary = new Dictionary<string, TargetAssembly>();
            foreach (CustomScriptAssembly assembly in customScriptAssemblies)
            {
                <CreateTargetAssemblies>c__AnonStorey1 storey = new <CreateTargetAssemblies>c__AnonStorey1 {
                    pathPrefixLowerCase = assembly.PathPrefix.ToLower()
                };
                TargetAssembly item = new TargetAssembly(assembly.Name + ".dll", assembly.Language, assembly.AssemblyFlags, TargetAssemblyType.Custom, new Func<string, int>(storey.<>m__0));
                list.Add(item);
                dictionary[assembly.Name] = item;
            }
            List<TargetAssembly>.Enumerator enumerator = list.GetEnumerator();
            foreach (CustomScriptAssembly assembly3 in customScriptAssemblies)
            {
                enumerator.MoveNext();
                TargetAssembly current = enumerator.Current;
                if (assembly3.References != null)
                {
                    foreach (string str in assembly3.References)
                    {
                        TargetAssembly assembly5 = null;
                        if (!dictionary.TryGetValue(str, out assembly5))
                        {
                            UnityEngine.Debug.LogWarning($"Could not find reference '{str}' for assembly '{assembly3.Name}'");
                        }
                        else
                        {
                            current.References.Add(assembly5);
                        }
                    }
                }
            }
            return list.ToArray();
        }

        public static PrecompiledAssembly CreateUserCompiledAssembly(string path)
        {
            AssemblyFlags none = AssemblyFlags.None;
            string str = path.ToLower();
            if (str.Contains("/editor/") || str.Contains(@"\editor\"))
            {
                none |= AssemblyFlags.EditorOnly;
            }
            return new PrecompiledAssembly { 
                Path = path,
                Flags = none
            };
        }

        private static int FilterAssemblyInEditorFolder(string pathName)
        {
            int index = pathName.IndexOf("/editor/");
            if (index == -1)
            {
                return -1;
            }
            return (index + "/editor/".Length);
        }

        private static int FilterAssemblyInFirstpassEditorFolder(string pathName)
        {
            if (FilterAssemblyInFirstpassFolder(pathName) == -1)
            {
                return -1;
            }
            return FilterAssemblyInEditorFolder(pathName);
        }

        private static int FilterAssemblyInFirstpassFolder(string pathName)
        {
            int num = FilterAssemblyPathBeginsWith(pathName, "/assets/plugins/");
            if (num >= 0)
            {
                return num;
            }
            num = FilterAssemblyPathBeginsWith(pathName, "/assets/standard assets/");
            if (num >= 0)
            {
                return num;
            }
            num = FilterAssemblyPathBeginsWith(pathName, "/assets/pro standard assets/");
            if (num >= 0)
            {
                return num;
            }
            num = FilterAssemblyPathBeginsWith(pathName, "/assets/iphone standard assets/");
            if (num >= 0)
            {
                return num;
            }
            return -1;
        }

        private static int FilterAssemblyPathBeginsWith(string pathName, string prefix) => 
            (!pathName.StartsWith(prefix) ? -1 : prefix.Length);

        public static ScriptAssembly[] GenerateChangedScriptAssemblies(GenerateChangedScriptAssembliesArgs args)
        {
            int num2;
            bool flag = (args.BuildFlags & BuildFlags.BuildingForEditor) == BuildFlags.BuildingForEditor;
            Dictionary<TargetAssembly, HashSet<string>> dictionary = new Dictionary<TargetAssembly, HashSet<string>>();
            TargetAssembly[] assemblyArray = (args.Assemblies.CustomTargetAssemblies != null) ? predefinedTargetAssemblies.Concat<TargetAssembly>(args.Assemblies.CustomTargetAssemblies).ToArray<TargetAssembly>() : predefinedTargetAssemblies;
            if (args.RunUpdaterAssemblies != null)
            {
                using (HashSet<string>.Enumerator enumerator = args.RunUpdaterAssemblies.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        <GenerateChangedScriptAssemblies>c__AnonStorey2 storey = new <GenerateChangedScriptAssemblies>c__AnonStorey2 {
                            assemblyFilename = enumerator.Current
                        };
                        TargetAssembly assembly = Enumerable.First<TargetAssembly>(assemblyArray, new Func<TargetAssembly, bool>(storey.<>m__0));
                        dictionary[assembly] = new HashSet<string>();
                    }
                }
            }
            foreach (string str in args.DirtySourceFiles)
            {
                TargetAssembly key = GetTargetAssembly(str, args.ProjectDirectory, args.Assemblies.CustomTargetAssemblies);
                if (key == null)
                {
                    args.NotCompiledSourceFiles.Add(str);
                }
                else if (flag || !key.EditorOnly)
                {
                    HashSet<string> set;
                    if (!dictionary.TryGetValue(key, out set))
                    {
                        set = new HashSet<string>();
                        dictionary[key] = set;
                    }
                    set.Add(Path.Combine(args.ProjectDirectory, str));
                }
            }
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = entry => entry.Key.Type == TargetAssemblyType.Custom;
            }
            if (Enumerable.Any<KeyValuePair<TargetAssembly, HashSet<string>>>(dictionary, <>f__am$cache0))
            {
                foreach (TargetAssembly assembly3 in predefinedTargetAssemblies)
                {
                    if ((flag || !assembly3.EditorOnly) && !dictionary.ContainsKey(assembly3))
                    {
                        dictionary[assembly3] = new HashSet<string>();
                    }
                }
            }
            do
            {
                num2 = 0;
                foreach (TargetAssembly assembly4 in assemblyArray)
                {
                    if ((flag || !assembly4.EditorOnly) && !dictionary.ContainsKey(assembly4))
                    {
                        foreach (TargetAssembly assembly5 in assembly4.References)
                        {
                            if (dictionary.ContainsKey(assembly5))
                            {
                                dictionary[assembly4] = new HashSet<string>();
                                num2++;
                                break;
                            }
                        }
                    }
                }
            }
            while (num2 > 0);
            foreach (string str2 in args.AllSourceFiles)
            {
                TargetAssembly assembly6 = GetTargetAssembly(str2, args.ProjectDirectory, args.Assemblies.CustomTargetAssemblies);
                if (assembly6 == null)
                {
                    args.NotCompiledSourceFiles.Add(str2);
                }
                else
                {
                    HashSet<string> set2;
                    if ((flag || !assembly6.EditorOnly) && dictionary.TryGetValue(assembly6, out set2))
                    {
                        set2.Add(Path.Combine(args.ProjectDirectory, str2));
                    }
                }
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = e => e.Value.Count > 0;
            }
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = e => e.Key;
            }
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = e => e.Value;
            }
            return ToScriptAssemblies(Enumerable.ToDictionary<KeyValuePair<TargetAssembly, HashSet<string>>, TargetAssembly, HashSet<string>>(Enumerable.Where<KeyValuePair<TargetAssembly, HashSet<string>>>(dictionary, <>f__am$cache1), <>f__am$cache2, <>f__am$cache3), args.Settings, args.BuildFlags, args.Assemblies, args.RunUpdaterAssemblies);
        }

        public static ScriptAssembly[] GetAllScriptAssemblies(IEnumerable<string> allSourceFiles, string projectDirectory, BuildFlags buildFlags, ScriptAssemblySettings settings, CompilationAssemblies assemblies)
        {
            bool flag = (buildFlags & BuildFlags.BuildingForEditor) == BuildFlags.BuildingForEditor;
            Dictionary<TargetAssembly, HashSet<string>> targetAssemblies = new Dictionary<TargetAssembly, HashSet<string>>();
            foreach (string str in allSourceFiles)
            {
                TargetAssembly key = GetTargetAssembly(str, projectDirectory, assemblies.CustomTargetAssemblies);
                if ((key != null) && (flag || !key.EditorOnly))
                {
                    HashSet<string> set;
                    if (!targetAssemblies.TryGetValue(key, out set))
                    {
                        set = new HashSet<string>();
                        targetAssemblies[key] = set;
                    }
                    set.Add(Path.Combine(projectDirectory, str));
                }
            }
            return ToScriptAssemblies(targetAssemblies, settings, buildFlags, assemblies, null);
        }

        internal static TargetAssembly GetCustomTargetAssembly(string scriptPath, string projectDirectory, TargetAssembly[] customTargetAssemblies, ref TargetAssembly candidateAssembly)
        {
            if (customTargetAssemblies == null)
            {
                return null;
            }
            string str = Path.GetExtension(scriptPath).Substring(1).ToLower();
            int num = -1;
            TargetAssembly assembly2 = null;
            string str2 = !Path.IsPathRooted(scriptPath) ? Path.Combine(projectDirectory, scriptPath).ToLower() : scriptPath.ToLower();
            foreach (TargetAssembly assembly3 in customTargetAssemblies)
            {
                int num3 = assembly3.PathFilter(str2);
                if (num3 > num)
                {
                    if (str == assembly3.Language.GetExtensionICanCompile())
                    {
                        assembly2 = assembly3;
                        num = num3;
                    }
                    else
                    {
                        candidateAssembly = assembly3;
                    }
                }
            }
            return assembly2;
        }

        public static TargetAssembly[] GetPredefinedTargetAssemblies() => 
            predefinedTargetAssemblies;

        internal static TargetAssembly GetPredefinedTargetAssembly(string scriptPath)
        {
            TargetAssembly assembly = null;
            string str = Path.GetExtension(scriptPath).Substring(1).ToLower();
            string str2 = "/" + scriptPath.ToLower();
            int num = -1;
            foreach (TargetAssembly assembly2 in predefinedTargetAssemblies)
            {
                if (str == assembly2.Language.GetExtensionICanCompile())
                {
                    Func<string, int> pathFilter = assembly2.PathFilter;
                    int num3 = -1;
                    if (pathFilter == null)
                    {
                        num3 = 0;
                    }
                    else
                    {
                        num3 = pathFilter(str2);
                    }
                    if (num3 > num)
                    {
                        assembly = assembly2;
                        num = num3;
                    }
                }
            }
            return assembly;
        }

        public static IEnumerable<TargetAssembly> GetTargetAssemblies(SupportedLanguage language, TargetAssembly[] customTargetAssemblies)
        {
            <GetTargetAssemblies>c__AnonStorey0 storey = new <GetTargetAssemblies>c__AnonStorey0 {
                language = language
            };
            IEnumerable<TargetAssembly> first = Enumerable.Where<TargetAssembly>(predefinedTargetAssemblies, new Func<TargetAssembly, bool>(storey.<>m__0));
            if (customTargetAssemblies == null)
            {
                return first;
            }
            IEnumerable<TargetAssembly> second = Enumerable.Where<TargetAssembly>(customTargetAssemblies, new Func<TargetAssembly, bool>(storey.<>m__1));
            return first.Concat<TargetAssembly>(second);
        }

        internal static TargetAssembly GetTargetAssembly(string scriptPath, string projectDirectory, TargetAssembly[] customTargetAssemblies)
        {
            TargetAssembly candidateAssembly = null;
            TargetAssembly assembly2 = GetCustomTargetAssembly(scriptPath, projectDirectory, customTargetAssemblies, ref candidateAssembly);
            if (assembly2 != null)
            {
                return assembly2;
            }
            if (candidateAssembly != null)
            {
                return null;
            }
            return GetPredefinedTargetAssembly(scriptPath);
        }

        private static bool IsCompiledAssemblyCompatibleWithTargetAssembly(PrecompiledAssembly compiledAssembly, TargetAssembly targetAssembly, BuildTarget buildTarget, TargetAssembly[] customTargetAssemblies)
        {
            if (WSAHelpers.UseDotNetCore(targetAssembly.Filename, buildTarget, customTargetAssemblies))
            {
                return ((compiledAssembly.Flags & AssemblyFlags.UseForDotNet) == AssemblyFlags.UseForDotNet);
            }
            return ((compiledAssembly.Flags & AssemblyFlags.UseForMono) == AssemblyFlags.UseForMono);
        }

        internal static ScriptAssembly[] ToScriptAssemblies(IDictionary<TargetAssembly, HashSet<string>> targetAssemblies, ScriptAssemblySettings settings, BuildFlags buildFlags, CompilationAssemblies assemblies, HashSet<string> runUpdaterAssemblies)
        {
            ScriptAssembly[] assemblyArray = new ScriptAssembly[targetAssemblies.Count];
            Dictionary<TargetAssembly, ScriptAssembly> targetToScriptAssembly = new Dictionary<TargetAssembly, ScriptAssembly>();
            int index = 0;
            bool flag = (buildFlags & BuildFlags.BuildingForEditor) == BuildFlags.BuildingForEditor;
            foreach (KeyValuePair<TargetAssembly, HashSet<string>> pair in targetAssemblies)
            {
                TargetAssembly key = pair.Key;
                HashSet<string> source = pair.Value;
                ScriptAssembly assembly2 = new ScriptAssembly();
                assemblyArray[index] = assembly2;
                targetToScriptAssembly[key] = assemblyArray[index++];
                assembly2.BuildTarget = settings.BuildTarget;
                if (key.EditorOnly || (flag && (settings.ApiCompatibilityLevel == ApiCompatibilityLevel.NET_4_6)))
                {
                    assembly2.ApiCompatibilityLevel = ApiCompatibilityLevel.NET_2_0;
                }
                else
                {
                    assembly2.ApiCompatibilityLevel = settings.ApiCompatibilityLevel;
                }
                if (!string.IsNullOrEmpty(settings.FilenameSuffix))
                {
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(key.Filename);
                    string extension = Path.GetExtension(key.Filename);
                    assembly2.Filename = fileNameWithoutExtension + settings.FilenameSuffix + extension;
                }
                else
                {
                    assembly2.Filename = key.Filename;
                }
                if ((runUpdaterAssemblies != null) && runUpdaterAssemblies.Contains(assembly2.Filename))
                {
                    assembly2.RunUpdater = true;
                }
                assembly2.OutputDirectory = settings.OutputDirectory;
                assembly2.Defines = settings.Defines;
                assembly2.Files = source.ToArray<string>();
            }
            index = 0;
            foreach (KeyValuePair<TargetAssembly, HashSet<string>> pair2 in targetAssemblies)
            {
                AddScriptAssemblyReferences(ref assemblyArray[index++], pair2.Key, settings, buildFlags, assemblies, targetToScriptAssembly, settings.FilenameSuffix);
            }
            return assemblyArray;
        }

        [CompilerGenerated]
        private sealed class <CreateTargetAssemblies>c__AnonStorey1
        {
            internal string pathPrefixLowerCase;

            internal int <>m__0(string path) => 
                (!path.StartsWith(this.pathPrefixLowerCase) ? -1 : this.pathPrefixLowerCase.Length);
        }

        [CompilerGenerated]
        private sealed class <GenerateChangedScriptAssemblies>c__AnonStorey2
        {
            internal string assemblyFilename;

            internal bool <>m__0(EditorBuildRules.TargetAssembly a) => 
                (a.Filename == this.assemblyFilename);
        }

        [CompilerGenerated]
        private sealed class <GetTargetAssemblies>c__AnonStorey0
        {
            internal SupportedLanguage language;

            internal bool <>m__0(EditorBuildRules.TargetAssembly a) => 
                (a.Language.GetLanguageName() == this.language.GetLanguageName());

            internal bool <>m__1(EditorBuildRules.TargetAssembly a) => 
                (a.Language.GetLanguageName() == this.language.GetLanguageName());
        }

        public class CompilationAssemblies
        {
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private EditorBuildRules.TargetAssembly[] <CustomTargetAssemblies>k__BackingField;
            [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
            private string[] <EditorAssemblyReferences>k__BackingField;
            [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
            private PrecompiledAssembly[] <PrecompiledAssemblies>k__BackingField;
            [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
            private PrecompiledAssembly[] <UnityAssemblies>k__BackingField;

            public EditorBuildRules.TargetAssembly[] CustomTargetAssemblies { get; set; }

            public string[] EditorAssemblyReferences { get; set; }

            public PrecompiledAssembly[] PrecompiledAssemblies { get; set; }

            public PrecompiledAssembly[] UnityAssemblies { get; set; }
        }

        public class GenerateChangedScriptAssembliesArgs
        {
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private IEnumerable<string> <AllSourceFiles>k__BackingField;
            [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
            private EditorBuildRules.CompilationAssemblies <Assemblies>k__BackingField;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private UnityEditor.Scripting.ScriptCompilation.BuildFlags <BuildFlags>k__BackingField;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private IEnumerable<string> <DirtySourceFiles>k__BackingField;
            [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
            private HashSet<string> <NotCompiledSourceFiles>k__BackingField;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private string <ProjectDirectory>k__BackingField;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private HashSet<string> <RunUpdaterAssemblies>k__BackingField;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private ScriptAssemblySettings <Settings>k__BackingField;

            public GenerateChangedScriptAssembliesArgs()
            {
                this.NotCompiledSourceFiles = new HashSet<string>();
            }

            public IEnumerable<string> AllSourceFiles { get; set; }

            public EditorBuildRules.CompilationAssemblies Assemblies { get; set; }

            public UnityEditor.Scripting.ScriptCompilation.BuildFlags BuildFlags { get; set; }

            public IEnumerable<string> DirtySourceFiles { get; set; }

            public HashSet<string> NotCompiledSourceFiles { get; set; }

            public string ProjectDirectory { get; set; }

            public HashSet<string> RunUpdaterAssemblies { get; set; }

            public ScriptAssemblySettings Settings { get; set; }
        }

        internal class TargetAssembly
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
            private string <Filename>k__BackingField;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private AssemblyFlags <Flags>k__BackingField;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private SupportedLanguage <Language>k__BackingField;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private Func<string, int> <PathFilter>k__BackingField;
            [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
            private List<EditorBuildRules.TargetAssembly> <References>k__BackingField;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private EditorBuildRules.TargetAssemblyType <Type>k__BackingField;

            public TargetAssembly()
            {
                this.References = new List<EditorBuildRules.TargetAssembly>();
            }

            public TargetAssembly(string name, SupportedLanguage language, AssemblyFlags flags, EditorBuildRules.TargetAssemblyType type) : this(name, language, flags, type, null)
            {
            }

            public TargetAssembly(string name, SupportedLanguage language, AssemblyFlags flags, EditorBuildRules.TargetAssemblyType type, Func<string, int> pathFilter) : this()
            {
                this.Language = language;
                this.Filename = name;
                this.Flags = flags;
                this.PathFilter = pathFilter;
                this.Type = type;
            }

            public bool EditorOnly =>
                ((this.Flags & AssemblyFlags.EditorOnly) == AssemblyFlags.EditorOnly);

            public string Filename { get; private set; }

            public AssemblyFlags Flags { get; private set; }

            public SupportedLanguage Language { get; private set; }

            public Func<string, int> PathFilter { get; private set; }

            public List<EditorBuildRules.TargetAssembly> References { get; private set; }

            public EditorBuildRules.TargetAssemblyType Type { get; private set; }
        }

        internal enum TargetAssemblyType
        {
            Undefined,
            Predefined,
            Custom
        }
    }
}

