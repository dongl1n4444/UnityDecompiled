namespace UnityEditor
{
    using Mono.Cecil;
    using Mono.Collections.Generic;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using UnityEditor.Modules;
    using UnityEditorInternal;
    using UnityEngine;

    internal class AssemblyHelper
    {
        [CompilerGenerated]
        private static Func<PluginImporter, string> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<FileInfo, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<FileInfo, string> <>f__am$cache2;
        private const int kDefaultDepth = 10;

        private static void AddReferencedAssembliesRecurse(string assemblyPath, List<string> alreadyFoundAssemblies, string[] allAssemblyPaths, string[] foldersToSearch, Dictionary<string, AssemblyDefinition> cache, BuildTarget target)
        {
            <AddReferencedAssembliesRecurse>c__AnonStorey1 storey = new <AddReferencedAssembliesRecurse>c__AnonStorey1 {
                target = target
            };
            if (!IgnoreAssembly(assemblyPath, storey.target))
            {
                AssemblyDefinition assemblyDefinitionCached = GetAssemblyDefinitionCached(assemblyPath, cache);
                if (assemblyDefinitionCached == null)
                {
                    throw new ArgumentException("Referenced Assembly " + Path.GetFileName(assemblyPath) + " could not be found!");
                }
                if (alreadyFoundAssemblies.IndexOf(assemblyPath) == -1)
                {
                    alreadyFoundAssemblies.Add(assemblyPath);
                    if (<>f__am$cache0 == null)
                    {
                        <>f__am$cache0 = new Func<PluginImporter, string>(null, (IntPtr) <AddReferencedAssembliesRecurse>m__0);
                    }
                    IEnumerable<string> enumerable = Enumerable.Select<PluginImporter, string>(Enumerable.Where<PluginImporter>(PluginImporter.GetImporters(storey.target), new Func<PluginImporter, bool>(storey, (IntPtr) this.<>m__0)), <>f__am$cache0).Distinct<string>();
                    using (Collection<AssemblyNameReference>.Enumerator enumerator = assemblyDefinitionCached.MainModule.AssemblyReferences.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            <AddReferencedAssembliesRecurse>c__AnonStorey2 storey2 = new <AddReferencedAssembliesRecurse>c__AnonStorey2 {
                                referencedAssembly = enumerator.Current
                            };
                            if (((storey2.referencedAssembly.Name != "BridgeInterface") && (storey2.referencedAssembly.Name != "WinRTBridge")) && ((storey2.referencedAssembly.Name != "UnityEngineProxy") && !IgnoreAssembly(storey2.referencedAssembly.Name + ".dll", storey.target)))
                            {
                                string str = FindAssemblyName(storey2.referencedAssembly.FullName, storey2.referencedAssembly.Name, allAssemblyPaths, foldersToSearch, cache);
                                if (str == "")
                                {
                                    bool flag = false;
                                    string[] strArray = new string[] { ".dll", ".winmd" };
                                    for (int i = 0; i < strArray.Length; i++)
                                    {
                                        <AddReferencedAssembliesRecurse>c__AnonStorey3 storey3 = new <AddReferencedAssembliesRecurse>c__AnonStorey3 {
                                            <>f__ref$2 = storey2,
                                            extension = strArray[i]
                                        };
                                        if (Enumerable.Any<string>(enumerable, new Func<string, bool>(storey3, (IntPtr) this.<>m__0)))
                                        {
                                            flag = true;
                                            break;
                                        }
                                    }
                                    if (flag)
                                    {
                                        continue;
                                    }
                                    throw new ArgumentException($"The Assembly {storey2.referencedAssembly.Name} is referenced by {assemblyDefinitionCached.MainModule.Assembly.Name.Name} ('{assemblyPath}'). But the dll is not allowed to be included or could not be found.");
                                }
                                AddReferencedAssembliesRecurse(str, alreadyFoundAssemblies, allAssemblyPaths, foldersToSearch, cache, storey.target);
                            }
                        }
                    }
                }
            }
        }

        public static void CheckForAssemblyFileNameMismatch(string assemblyPath)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(assemblyPath);
            string str2 = ExtractInternalAssemblyName(assemblyPath);
            if (fileNameWithoutExtension != str2)
            {
                Debug.LogWarning("Assembly '" + str2 + "' has non matching file name: '" + Path.GetFileName(assemblyPath) + "'. This can cause build issues on some platforms.");
            }
        }

        public static void ExtractAllClassesThatInheritMonoBehaviourAndScriptableObject(string path, out string[] classNamesArray, out string[] classNameSpacesArray)
        {
            List<string> list = new List<string>();
            List<string> list2 = new List<string>();
            ReaderParameters parameters = new ReaderParameters();
            DefaultAssemblyResolver resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(Path.GetDirectoryName(path));
            parameters.AssemblyResolver = resolver;
            AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(path, parameters);
            foreach (ModuleDefinition definition2 in assembly.Modules)
            {
                foreach (TypeDefinition definition3 in definition2.Types)
                {
                    TypeReference baseType = definition3.BaseType;
                    try
                    {
                        if (IsTypeMonoBehaviourOrScriptableObject(assembly, baseType))
                        {
                            list.Add(definition3.Name);
                            list2.Add(definition3.Namespace);
                        }
                    }
                    catch (Exception)
                    {
                        Debug.LogError("Failed to extract " + definition3.FullName + " class of base type " + baseType.FullName + " when inspecting " + path);
                    }
                }
            }
            classNamesArray = list.ToArray();
            classNameSpacesArray = list2.ToArray();
        }

        public static AssemblyTypeInfoGenerator.ClassInfo[] ExtractAssemblyTypeInfo(BuildTarget targetPlatform, bool isEditor, string assemblyPathName, string[] searchDirs)
        {
            AssemblyTypeInfoGenerator.ClassInfo[] infoArray;
            try
            {
                AssemblyTypeInfoGenerator generator;
                ICompilationExtension compilationExtension = ModuleManager.GetCompilationExtension(ModuleManager.GetTargetStringFromBuildTarget(targetPlatform));
                string[] compilerExtraAssemblyPaths = compilationExtension.GetCompilerExtraAssemblyPaths(isEditor, assemblyPathName);
                if ((compilerExtraAssemblyPaths != null) && (compilerExtraAssemblyPaths.Length > 0))
                {
                    List<string> list = new List<string>(searchDirs);
                    list.AddRange(compilerExtraAssemblyPaths);
                    searchDirs = list.ToArray();
                }
                IAssemblyResolver resolver = compilationExtension.GetAssemblyResolver(isEditor, assemblyPathName, searchDirs);
                if (resolver == null)
                {
                    generator = new AssemblyTypeInfoGenerator(assemblyPathName, searchDirs);
                }
                else
                {
                    generator = new AssemblyTypeInfoGenerator(assemblyPathName, resolver);
                }
                infoArray = generator.GatherClassInfo();
            }
            catch (Exception exception)
            {
                object[] objArray1 = new object[] { "ExtractAssemblyTypeInfo: Failed to process ", assemblyPathName, ", ", exception };
                throw new Exception(string.Concat(objArray1));
            }
            return infoArray;
        }

        public static string ExtractInternalAssemblyName(string path) => 
            AssemblyDefinition.ReadAssembly(path).Name.Name;

        internal static ICollection<string> FindAssemblies(string basePath) => 
            FindAssemblies(basePath, 10);

        internal static ICollection<string> FindAssemblies(string basePath, int maxDepth)
        {
            List<string> list = new List<string>();
            if (maxDepth != 0)
            {
                try
                {
                    DirectoryInfo info = new DirectoryInfo(basePath);
                    if (<>f__am$cache1 == null)
                    {
                        <>f__am$cache1 = new Func<FileInfo, bool>(null, (IntPtr) <FindAssemblies>m__1);
                    }
                    if (<>f__am$cache2 == null)
                    {
                        <>f__am$cache2 = new Func<FileInfo, string>(null, (IntPtr) <FindAssemblies>m__2);
                    }
                    list.AddRange(Enumerable.Select<FileInfo, string>(Enumerable.Where<FileInfo>(info.GetFiles(), <>f__am$cache1), <>f__am$cache2));
                    foreach (DirectoryInfo info2 in info.GetDirectories())
                    {
                        list.AddRange(FindAssemblies(info2.FullName, maxDepth - 1));
                    }
                }
                catch (Exception)
                {
                }
            }
            return list;
        }

        public static string[] FindAssembliesReferencedBy(string[] paths, string[] foldersToSearch, BuildTarget target)
        {
            List<string> alreadyFoundAssemblies = new List<string>();
            string[] allAssemblyPaths = paths;
            Dictionary<string, AssemblyDefinition> cache = new Dictionary<string, AssemblyDefinition>();
            for (int i = 0; i < paths.Length; i++)
            {
                AddReferencedAssembliesRecurse(paths[i], alreadyFoundAssemblies, allAssemblyPaths, foldersToSearch, cache, target);
            }
            for (int j = 0; j < paths.Length; j++)
            {
                alreadyFoundAssemblies.Remove(paths[j]);
            }
            return alreadyFoundAssemblies.ToArray();
        }

        public static string[] FindAssembliesReferencedBy(string path, string[] foldersToSearch, BuildTarget target) => 
            FindAssembliesReferencedBy(new string[] { path }, foldersToSearch, target);

        private static string FindAssemblyName(string fullName, string name, string[] allAssemblyPaths, string[] foldersToSearch, Dictionary<string, AssemblyDefinition> cache)
        {
            for (int i = 0; i < allAssemblyPaths.Length; i++)
            {
                if (GetAssemblyDefinitionCached(allAssemblyPaths[i], cache).MainModule.Assembly.Name.Name == name)
                {
                    return allAssemblyPaths[i];
                }
            }
            foreach (string str2 in foldersToSearch)
            {
                string path = Path.Combine(str2, name + ".dll");
                if (File.Exists(path))
                {
                    return path;
                }
            }
            return "";
        }

        [DebuggerHidden]
        internal static IEnumerable<T> FindImplementors<T>(Assembly assembly) where T: class => 
            new <FindImplementors>c__Iterator0<T> { 
                assembly = assembly,
                $PC = -2
            };

        public static Assembly FindLoadedAssemblyWithName(string s)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                try
                {
                    if (assembly.Location.Contains(s))
                    {
                        return assembly;
                    }
                }
                catch (NotSupportedException)
                {
                }
            }
            return null;
        }

        private static AssemblyDefinition GetAssemblyDefinitionCached(string path, Dictionary<string, AssemblyDefinition> cache)
        {
            if (cache.ContainsKey(path))
            {
                return cache[path];
            }
            AssemblyDefinition definition2 = AssemblyDefinition.ReadAssembly(path);
            cache[path] = definition2;
            return definition2;
        }

        public static string[] GetNamesOfAssembliesLoadedInCurrentDomain()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            List<string> list = new List<string>();
            foreach (Assembly assembly in assemblies)
            {
                try
                {
                    list.Add(assembly.Location);
                }
                catch (NotSupportedException)
                {
                }
            }
            return list.ToArray();
        }

        internal static Type[] GetTypesFromAssembly(Assembly assembly)
        {
            if (assembly == null)
            {
                return new Type[0];
            }
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException)
            {
                return new Type[0];
            }
        }

        private static bool IgnoreAssembly(string assemblyPath, BuildTarget target) => 
            (((target == BuildTarget.WSAPlayer) && ((((assemblyPath.IndexOf("mscorlib.dll") != -1) || (assemblyPath.IndexOf("System.") != -1)) || ((assemblyPath.IndexOf("Windows.dll") != -1) || (assemblyPath.IndexOf("Microsoft.") != -1))) || (((assemblyPath.IndexOf("Windows.") != -1) || (assemblyPath.IndexOf("WinRTLegacy.dll") != -1)) || (assemblyPath.IndexOf("platform.dll") != -1)))) || IsInternalAssembly(assemblyPath));

        public static bool IsInternalAssembly(string file)
        {
            <IsInternalAssembly>c__AnonStorey4 storey = new <IsInternalAssembly>c__AnonStorey4 {
                file = file
            };
            return (ModuleManager.IsRegisteredModule(storey.file) || Enumerable.Any<string>(ModuleUtils.GetAdditionalReferencesForUserScripts(), new Func<string, bool>(storey, (IntPtr) this.<>m__0)));
        }

        public static bool IsManagedAssembly(string file)
        {
            DllType type = InternalEditorUtility.DetectDotNetDll(file);
            return ((type != DllType.Unknown) && (type != DllType.Native));
        }

        private static bool IsTypeMonoBehaviourOrScriptableObject(AssemblyDefinition assembly, TypeReference type)
        {
            if (type == null)
            {
                return false;
            }
            if (type.FullName == "System.Object")
            {
                return false;
            }
            Assembly assembly2 = null;
            if (type.Scope.Name == "UnityEngine")
            {
                assembly2 = typeof(MonoBehaviour).Assembly;
            }
            else if (type.Scope.Name == "UnityEditor")
            {
                assembly2 = typeof(EditorWindow).Assembly;
            }
            else if (type.Scope.Name == "UnityEngine.UI")
            {
                assembly2 = FindLoadedAssemblyWithName("UnityEngine.UI");
            }
            if (assembly2 != null)
            {
                string name = !type.IsGenericInstance ? type.FullName : (type.Namespace + "." + type.Name);
                Type type2 = assembly2.GetType(name);
                if ((type2 == typeof(MonoBehaviour)) || type2.IsSubclassOf(typeof(MonoBehaviour)))
                {
                    return true;
                }
                if ((type2 == typeof(ScriptableObject)) || type2.IsSubclassOf(typeof(ScriptableObject)))
                {
                    return true;
                }
            }
            TypeDefinition definition = null;
            try
            {
                definition = type.Resolve();
            }
            catch (AssemblyResolutionException)
            {
            }
            return ((definition != null) && IsTypeMonoBehaviourOrScriptableObject(assembly, definition.BaseType));
        }

        [CompilerGenerated]
        private sealed class <AddReferencedAssembliesRecurse>c__AnonStorey1
        {
            internal BuildTarget target;

            internal bool <>m__0(PluginImporter i)
            {
                string platformData = i.GetPlatformData(this.target, "CPU");
                return (!string.IsNullOrEmpty(platformData) && !string.Equals(platformData, "AnyCPU", StringComparison.InvariantCultureIgnoreCase));
            }
        }

        [CompilerGenerated]
        private sealed class <AddReferencedAssembliesRecurse>c__AnonStorey2
        {
            internal AssemblyNameReference referencedAssembly;
        }

        [CompilerGenerated]
        private sealed class <AddReferencedAssembliesRecurse>c__AnonStorey3
        {
            internal AssemblyHelper.<AddReferencedAssembliesRecurse>c__AnonStorey2 <>f__ref$2;
            internal string extension;

            internal bool <>m__0(string p) => 
                string.Equals(p, this.<>f__ref$2.referencedAssembly.Name + this.extension, StringComparison.InvariantCultureIgnoreCase);
        }

        [CompilerGenerated]
        private sealed class <FindImplementors>c__Iterator0<T> : IEnumerable, IEnumerable<T>, IEnumerator, IDisposable, IEnumerator<T> where T: class
        {
            internal T $current;
            internal bool $disposing;
            internal Type[] $locvar0;
            internal int $locvar1;
            internal int $PC;
            internal Type <interfaze>__0;
            internal T <module>__2;
            internal Type <type>__1;
            internal Assembly assembly;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$disposing = true;
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<interfaze>__0 = typeof(T);
                        this.$locvar0 = AssemblyHelper.GetTypesFromAssembly(this.assembly);
                        this.$locvar1 = 0;
                        goto Label_0144;

                    case 1:
                        break;

                    default:
                        goto Label_015E;
                }
            Label_0136:
                this.$locvar1++;
            Label_0144:
                if (this.$locvar1 < this.$locvar0.Length)
                {
                    this.<type>__1 = this.$locvar0[this.$locvar1];
                    if ((!this.<type>__1.IsInterface && !this.<type>__1.IsAbstract) && this.<interfaze>__0.IsAssignableFrom(this.<type>__1))
                    {
                        this.<module>__2 = null;
                        if (typeof(ScriptableObject).IsAssignableFrom(this.<type>__1))
                        {
                            this.<module>__2 = ScriptableObject.CreateInstance(this.<type>__1) as T;
                        }
                        else
                        {
                            this.<module>__2 = Activator.CreateInstance(this.<type>__1) as T;
                        }
                        if (this.<module>__2 != null)
                        {
                            this.$current = this.<module>__2;
                            if (!this.$disposing)
                            {
                                this.$PC = 1;
                            }
                            return true;
                        }
                    }
                    goto Label_0136;
                }
                this.$PC = -1;
            Label_015E:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new AssemblyHelper.<FindImplementors>c__Iterator0<T> { assembly = this.assembly };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<T>.GetEnumerator();

            T IEnumerator<T>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }

        [CompilerGenerated]
        private sealed class <IsInternalAssembly>c__AnonStorey4
        {
            internal string file;

            internal bool <>m__0(string p) => 
                p.Equals(this.file);
        }
    }
}

