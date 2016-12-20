namespace UnityEditor
{
    using Mono.Cecil;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Xml;
    using UnityEditor.Analytics;
    using UnityEditor.BuildReporting;
    using UnityEditor.Utils;
    using UnityEditorInternal;

    internal class CodeStrippingUtils
    {
        [CompilerGenerated]
        private static Func<string, UnityType> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<string, UnityType> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<UnityType, bool> <>f__am$cache2;
        private static UnityType[] s_blackListNativeClasses;
        private static Dictionary<UnityType, UnityType> s_blackListNativeClassesDependency;
        private static readonly Dictionary<string, string> s_blackListNativeClassesDependencyNames;
        private static string[] s_blackListNativeClassNames = new string[] { "PreloadData", "Material", "Cubemap", "Texture3D", "Texture2DArray", "RenderTexture", "Mesh", "Sprite" };
        private static UnityType s_GameManagerTypeInfo = null;
        private static readonly string[] s_UserAssemblies;

        static CodeStrippingUtils()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string> {
                { 
                    "ParticleSystemRenderer",
                    "ParticleSystem"
                }
            };
            s_blackListNativeClassesDependencyNames = dictionary;
            s_UserAssemblies = new string[] { "Assembly-CSharp.dll", "Assembly-CSharp-firstpass.dll", "Assembly-UnityScript.dll", "Assembly-UnityScript-firstpass.dll", "UnityEngine.Analytics.dll" };
        }

        private static HashSet<string> CollectManagedTypeReferencesFromRoots(string directory, string[] rootAssemblies, StrippingInfo strippingInfo)
        {
            HashSet<string> set = new HashSet<string>();
            AssemblyReferenceChecker checker = new AssemblyReferenceChecker();
            bool collectMethods = false;
            bool ignoreSystemDlls = false;
            checker.CollectReferencesFromRoots(directory, rootAssemblies, collectMethods, 0f, ignoreSystemDlls);
            string[] assemblyFileNames = checker.GetAssemblyFileNames();
            AssemblyDefinition[] assemblyDefinitions = checker.GetAssemblyDefinitions();
            foreach (AssemblyDefinition definition in assemblyDefinitions)
            {
                foreach (TypeDefinition definition2 in definition.MainModule.Types)
                {
                    if (definition2.Namespace.StartsWith("UnityEngine") && (((definition2.Fields.Count > 0) || (definition2.Methods.Count > 0)) || (definition2.Properties.Count > 0)))
                    {
                        string name = definition2.Name;
                        set.Add(name);
                        if ((strippingInfo != null) && !AssemblyReferenceChecker.IsIgnoredSystemDll(definition.Name.Name))
                        {
                            strippingInfo.RegisterDependency(name, "Required by Scripts");
                        }
                    }
                }
            }
            AssemblyDefinition definition3 = null;
            for (int i = 0; i < assemblyFileNames.Length; i++)
            {
                if (assemblyFileNames[i] == "UnityEngine.dll")
                {
                    definition3 = assemblyDefinitions[i];
                }
            }
            foreach (AssemblyDefinition definition4 in assemblyDefinitions)
            {
                if (definition4 != definition3)
                {
                    foreach (TypeReference reference in definition4.MainModule.GetTypeReferences())
                    {
                        if (reference.Namespace.StartsWith("UnityEngine"))
                        {
                            string item = reference.Name;
                            set.Add(item);
                            if ((strippingInfo != null) && !AssemblyReferenceChecker.IsIgnoredSystemDll(definition4.Name.Name))
                            {
                                strippingInfo.RegisterDependency(item, "Required by Scripts");
                            }
                        }
                    }
                }
            }
            return set;
        }

        private static HashSet<UnityType> CollectNativeClassListFromRoots(string directory, string[] rootAssemblies, StrippingInfo strippingInfo)
        {
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<string, UnityType>(null, (IntPtr) <CollectNativeClassListFromRoots>m__1);
            }
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = new Func<UnityType, bool>(null, (IntPtr) <CollectNativeClassListFromRoots>m__2);
            }
            return new HashSet<UnityType>(Enumerable.Where<UnityType>(Enumerable.Select<string, UnityType>(CollectManagedTypeReferencesFromRoots(directory, rootAssemblies, strippingInfo), <>f__am$cache1), <>f__am$cache2));
        }

        private static void ExcludeModuleManagers(ref HashSet<UnityType> nativeClasses)
        {
            string[] moduleNames = ModuleMetadata.GetModuleNames();
            foreach (string str in moduleNames)
            {
                if (ModuleMetadata.GetModuleStrippable(str))
                {
                    UnityType[] moduleTypes = ModuleMetadata.GetModuleTypes(str);
                    HashSet<UnityType> set = new HashSet<UnityType>();
                    HashSet<UnityType> other = new HashSet<UnityType>();
                    foreach (UnityType type in moduleTypes)
                    {
                        if (type.IsDerivedFrom(GameManagerTypeInfo))
                        {
                            set.Add(type);
                        }
                        else
                        {
                            other.Add(type);
                        }
                    }
                    if (other.Count != 0)
                    {
                        if (!nativeClasses.Overlaps(other))
                        {
                            foreach (UnityType type2 in set)
                            {
                                nativeClasses.Remove(type2);
                            }
                        }
                        else
                        {
                            foreach (UnityType type3 in set)
                            {
                                nativeClasses.Add(type3);
                            }
                        }
                    }
                }
            }
        }

        private static UnityType FindTypeByNameChecked(string name, string msg)
        {
            UnityType type = UnityType.FindTypeByName(name);
            if (type != null)
            {
                return type;
            }
            if (msg == null)
            {
            }
            throw new ArgumentException(string.Format("Could not map typename '{0}' to type info ({1})", name, "no context"));
        }

        public static void GenerateDependencies(string strippedAssemblyDir, string icallsListFile, RuntimeClassRegistry rcr, bool doStripping, out HashSet<UnityType> nativeClasses, out HashSet<string> nativeModules, IIl2CppPlatformProvider platformProvider)
        {
            StrippingInfo strippingInfo = (platformProvider != null) ? StrippingInfo.GetBuildReportData(platformProvider.buildReport) : null;
            string[] userAssemblies = GetUserAssemblies(strippedAssemblyDir);
            nativeClasses = !doStripping ? null : GenerateNativeClassList(rcr, strippedAssemblyDir, userAssemblies, strippingInfo);
            if (nativeClasses != null)
            {
                ExcludeModuleManagers(ref nativeClasses);
            }
            nativeModules = GetNativeModulesToRegister(nativeClasses, strippingInfo);
            if ((nativeClasses != null) && (icallsListFile != null))
            {
                HashSet<string> modulesFromICalls = GetModulesFromICalls(icallsListFile);
                foreach (string str in modulesFromICalls)
                {
                    if (!nativeModules.Contains(str) && (strippingInfo != null))
                    {
                        strippingInfo.RegisterDependency(StrippingInfo.ModuleName(str), "Required by Scripts");
                    }
                    UnityType[] moduleTypes = ModuleMetadata.GetModuleTypes(str);
                    foreach (UnityType type in moduleTypes)
                    {
                        if (type.IsDerivedFrom(GameManagerTypeInfo))
                        {
                            nativeClasses.Add(type);
                        }
                    }
                }
                nativeModules.UnionWith(modulesFromICalls);
            }
            bool flag = true;
            if (platformProvider != null)
            {
                while (flag)
                {
                    flag = false;
                    foreach (string str2 in Enumerable.ToList<string>(nativeModules))
                    {
                        string moduleWhitelist = GetModuleWhitelist(str2, platformProvider.moduleStrippingInformationFolder);
                        if (File.Exists(moduleWhitelist))
                        {
                            foreach (string str4 in GetDependentModules(moduleWhitelist))
                            {
                                if (!nativeModules.Contains(str4))
                                {
                                    nativeModules.Add(str4);
                                    flag = true;
                                }
                                if (strippingInfo != null)
                                {
                                    string key = StrippingInfo.ModuleName(str2);
                                    strippingInfo.RegisterDependency(StrippingInfo.ModuleName(str4), "Required by " + key);
                                    if (strippingInfo.icons.ContainsKey(key))
                                    {
                                        strippingInfo.SetIcon("Required by " + key, strippingInfo.icons[key]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            new AssemblyReferenceChecker().CollectReferencesFromRoots(strippedAssemblyDir, userAssemblies, true, 0f, true);
            if (strippingInfo != null)
            {
                foreach (string str6 in nativeModules)
                {
                    strippingInfo.AddModule(StrippingInfo.ModuleName(str6));
                }
                strippingInfo.AddModule(StrippingInfo.ModuleName("Core"));
            }
            if ((nativeClasses != null) && (strippingInfo != null))
            {
                InjectCustomDependencies(strippingInfo, nativeClasses);
            }
        }

        private static HashSet<UnityType> GenerateNativeClassList(RuntimeClassRegistry rcr, string directory, string[] rootAssemblies, StrippingInfo strippingInfo)
        {
            HashSet<UnityType> set = CollectNativeClassListFromRoots(directory, rootAssemblies, strippingInfo);
            foreach (UnityType type in BlackListNativeClasses)
            {
                set.Add(type);
            }
            foreach (UnityType type2 in BlackListNativeClassesDependency.Keys)
            {
                if (set.Contains(type2))
                {
                    UnityType item = BlackListNativeClassesDependency[type2];
                    set.Add(item);
                }
            }
            foreach (string str in rcr.GetAllNativeClassesIncludingManagersAsString())
            {
                UnityType type4 = UnityType.FindTypeByName(str);
                if ((type4 != null) && (type4.baseClass != null))
                {
                    set.Add(type4);
                    if ((strippingInfo != null) && !type4.IsDerivedFrom(GameManagerTypeInfo))
                    {
                        List<string> scenesForClass = rcr.GetScenesForClass(type4.persistentTypeID);
                        if (scenesForClass != null)
                        {
                            foreach (string str2 in scenesForClass)
                            {
                                strippingInfo.RegisterDependency(str, str2);
                                if (str2.EndsWith(".unity"))
                                {
                                    strippingInfo.SetIcon(str2, "class/SceneAsset");
                                }
                                else
                                {
                                    strippingInfo.SetIcon(str2, "class/AssetBundle");
                                }
                            }
                        }
                    }
                }
            }
            HashSet<UnityType> set2 = new HashSet<UnityType>();
            foreach (UnityType type5 in set)
            {
                for (UnityType type6 = type5; type6.baseClass != null; type6 = type6.baseClass)
                {
                    set2.Add(type6);
                }
            }
            return set2;
        }

        private static HashSet<string> GetAllStrippableModules()
        {
            HashSet<string> set = new HashSet<string>();
            foreach (string str in ModuleMetadata.GetModuleNames())
            {
                if (ModuleMetadata.GetModuleStrippable(str))
                {
                    set.Add(str);
                }
            }
            return set;
        }

        private static IEnumerable<string> GetAssembliesInDirectory(string strippedAssemblyDir, string assemblyName)
        {
            return Directory.GetFiles(strippedAssemblyDir, assemblyName, SearchOption.TopDirectoryOnly);
        }

        private static string[] GetAssembliesWithSuffix()
        {
            <GetAssembliesWithSuffix>c__AnonStorey0 storey = new <GetAssembliesWithSuffix>c__AnonStorey0 {
                suffix = EditorSettings.Internal_UserGeneratedProjectSuffix
            };
            return Enumerable.ToArray<string>(Enumerable.Select<string, string>(s_UserAssemblies, new Func<string, string>(storey, (IntPtr) this.<>m__0)));
        }

        public static List<string> GetDependentModules(string moduleXml)
        {
            XmlDocument document = new XmlDocument();
            document.Load(moduleXml);
            List<string> list = new List<string>();
            IEnumerator enumerator = document.DocumentElement.SelectNodes("/linker/dependencies/module").GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    XmlNode current = (XmlNode) enumerator.Current;
                    list.Add(current.Attributes["name"].Value);
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
            return list;
        }

        public static HashSet<string> GetModulesFromICalls(string icallsListFile)
        {
            string[] strArray = File.ReadAllLines(icallsListFile);
            HashSet<string> set = new HashSet<string>();
            foreach (string str in strArray)
            {
                string iCallModule = ModuleMetadata.GetICallModule(str);
                if (!string.IsNullOrEmpty(iCallModule))
                {
                    set.Add(iCallModule);
                }
            }
            return set;
        }

        public static string GetModuleWhitelist(string module, string moduleStrippingInformationFolder)
        {
            string[] components = new string[] { moduleStrippingInformationFolder, module + ".xml" };
            return Paths.Combine(components);
        }

        public static HashSet<string> GetNativeModulesToRegister(HashSet<UnityType> nativeClasses, StrippingInfo strippingInfo)
        {
            return ((nativeClasses != null) ? GetRequiredStrippableModules(nativeClasses, strippingInfo) : GetAllStrippableModules());
        }

        private static HashSet<string> GetRequiredStrippableModules(HashSet<UnityType> nativeClasses, StrippingInfo strippingInfo)
        {
            HashSet<UnityType> set = new HashSet<UnityType>();
            HashSet<string> set2 = new HashSet<string>();
            foreach (string str in ModuleMetadata.GetModuleNames())
            {
                if (ModuleMetadata.GetModuleStrippable(str))
                {
                    HashSet<UnityType> other = new HashSet<UnityType>(ModuleMetadata.GetModuleTypes(str));
                    if (nativeClasses.Overlaps(other))
                    {
                        set2.Add(str);
                        if (strippingInfo != null)
                        {
                            foreach (UnityType type in other)
                            {
                                if (nativeClasses.Contains(type))
                                {
                                    strippingInfo.RegisterDependency(StrippingInfo.ModuleName(str), type.name);
                                    set.Add(type);
                                }
                            }
                        }
                    }
                }
            }
            if (strippingInfo != null)
            {
                foreach (UnityType type2 in nativeClasses)
                {
                    if (!set.Contains(type2))
                    {
                        strippingInfo.RegisterDependency(StrippingInfo.ModuleName("Core"), type2.name);
                    }
                }
            }
            return set2;
        }

        private static string[] GetUserAssemblies(string strippedAssemblyDir)
        {
            List<string> list = new List<string>();
            foreach (string str in UserAssemblies)
            {
                list.AddRange(GetAssembliesInDirectory(strippedAssemblyDir, str));
            }
            return list.ToArray();
        }

        public static void InjectCustomDependencies(StrippingInfo strippingInfo, HashSet<UnityType> nativeClasses)
        {
            UnityType item = UnityType.FindTypeByName("UnityAnalyticsManager");
            if (nativeClasses.Contains(item))
            {
                if (PlayerSettings.submitAnalytics)
                {
                    strippingInfo.RegisterDependency("UnityAnalyticsManager", "Required by HW Statistics (See Player Settings)");
                    strippingInfo.SetIcon("Required by HW Statistics (See Player Settings)", "class/PlayerSettings");
                }
                if (AnalyticsSettings.enabled)
                {
                    strippingInfo.RegisterDependency("UnityAnalyticsManager", "Required by Unity Analytics (See Services Window)");
                    strippingInfo.SetIcon("Required by Unity Analytics (See Services Window)", "class/PlayerSettings");
                }
            }
        }

        private static void WriteModuleAndClassRegistrationFile(string file, HashSet<string> nativeModules, HashSet<UnityType> nativeClasses, HashSet<UnityType> classesToSkip)
        {
            using (TextWriter writer = new StreamWriter(file))
            {
                writer.WriteLine("template <typename T> void RegisterClass();");
                writer.WriteLine("template <typename T> void RegisterStrippedTypeInfo(int, const char*, const char*);");
                writer.WriteLine();
                WriteStaticallyLinkedModuleRegistration(writer, nativeModules, nativeClasses);
                writer.WriteLine();
                if (nativeClasses != null)
                {
                    foreach (UnityType type in UnityType.GetTypes())
                    {
                        if (((type.baseClass != null) && !type.isEditorOnly) && !classesToSkip.Contains(type))
                        {
                            if (type.hasNativeNamespace)
                            {
                                writer.Write("namespace {0} {{ class {1}; }} ", type.nativeNamespace, type.name);
                            }
                            else
                            {
                                writer.Write("class {0}; ", type.name);
                            }
                            if (nativeClasses.Contains(type))
                            {
                                writer.WriteLine("template <> void RegisterClass<{0}>();", type.qualifiedName);
                            }
                            else
                            {
                                writer.WriteLine();
                            }
                        }
                    }
                    writer.WriteLine();
                }
                writer.WriteLine("void RegisterAllClasses()");
                writer.WriteLine("{");
                if (nativeClasses == null)
                {
                    writer.WriteLine("\tvoid RegisterAllClassesGranular();");
                    writer.WriteLine("\tRegisterAllClassesGranular();");
                }
                else
                {
                    writer.WriteLine("void RegisterBuiltinTypes();");
                    writer.WriteLine("RegisterBuiltinTypes();");
                    writer.WriteLine("\t//Total: {0} non stripped classes", nativeClasses.Count);
                    int num = 0;
                    foreach (UnityType type2 in nativeClasses)
                    {
                        writer.WriteLine("\t//{0}. {1}", num, type2.qualifiedName);
                        if (classesToSkip.Contains(type2))
                        {
                            writer.WriteLine("\t//Skipping {0}", type2.qualifiedName);
                        }
                        else
                        {
                            writer.WriteLine("\tRegisterClass<{0}>();", type2.qualifiedName);
                        }
                        num++;
                    }
                    writer.WriteLine();
                }
                writer.WriteLine("}");
                writer.Close();
            }
        }

        public static void WriteModuleAndClassRegistrationFile(string strippedAssemblyDir, string icallsListFile, string outputDir, RuntimeClassRegistry rcr, IEnumerable<UnityType> classesToSkip, IIl2CppPlatformProvider platformProvider)
        {
            HashSet<UnityType> set;
            HashSet<string> set2;
            bool stripEngineCode = PlayerSettings.stripEngineCode;
            GenerateDependencies(strippedAssemblyDir, icallsListFile, rcr, stripEngineCode, out set, out set2, platformProvider);
            WriteModuleAndClassRegistrationFile(Path.Combine(outputDir, "UnityClassRegistration.cpp"), set2, set, new HashSet<UnityType>(classesToSkip));
        }

        private static void WriteStaticallyLinkedModuleRegistration(TextWriter w, HashSet<string> nativeModules, HashSet<UnityType> nativeClasses)
        {
            w.WriteLine("void InvokeRegisterStaticallyLinkedModuleClasses()");
            w.WriteLine("{");
            if (nativeClasses == null)
            {
                w.WriteLine("\tvoid RegisterStaticallyLinkedModuleClasses();");
                w.WriteLine("\tRegisterStaticallyLinkedModuleClasses();");
            }
            else
            {
                w.WriteLine("\t// Do nothing (we're in stripping mode)");
            }
            w.WriteLine("}");
            w.WriteLine();
            w.WriteLine("void RegisterStaticallyLinkedModulesGranular()");
            w.WriteLine("{");
            foreach (string str in nativeModules)
            {
                w.WriteLine("\tvoid RegisterModule_" + str + "();");
                w.WriteLine("\tRegisterModule_" + str + "();");
                w.WriteLine();
            }
            w.WriteLine("}");
        }

        public static UnityType[] BlackListNativeClasses
        {
            get
            {
                if (s_blackListNativeClasses == null)
                {
                    if (<>f__am$cache0 == null)
                    {
                        <>f__am$cache0 = new Func<string, UnityType>(null, (IntPtr) <get_BlackListNativeClasses>m__0);
                    }
                    s_blackListNativeClasses = Enumerable.ToArray<UnityType>(Enumerable.Select<string, UnityType>(s_blackListNativeClassNames, <>f__am$cache0));
                }
                return s_blackListNativeClasses;
            }
        }

        public static Dictionary<UnityType, UnityType> BlackListNativeClassesDependency
        {
            get
            {
                if (s_blackListNativeClassesDependency == null)
                {
                    s_blackListNativeClassesDependency = new Dictionary<UnityType, UnityType>();
                    foreach (KeyValuePair<string, string> pair in s_blackListNativeClassesDependencyNames)
                    {
                        UnityType key = FindTypeByNameChecked(pair.Key, "code stripping blacklist native class dependency key");
                        BlackListNativeClassesDependency.Add(key, FindTypeByNameChecked(pair.Value, "code stripping blacklist native class dependency value"));
                    }
                }
                return s_blackListNativeClassesDependency;
            }
        }

        private static UnityType GameManagerTypeInfo
        {
            get
            {
                if (s_GameManagerTypeInfo == null)
                {
                    s_GameManagerTypeInfo = FindTypeByNameChecked("GameManager", "initializing code stripping utils");
                }
                return s_GameManagerTypeInfo;
            }
        }

        public static string[] UserAssemblies
        {
            get
            {
                try
                {
                    return GetAssembliesWithSuffix();
                }
                catch
                {
                    return s_UserAssemblies;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <GetAssembliesWithSuffix>c__AnonStorey0
        {
            internal string suffix;

            internal string <>m__0(string a)
            {
                if (a.StartsWith("Assembly-"))
                {
                    return (a.Substring(0, a.Length - ".dll".Length) + this.suffix + ".dll");
                }
                return a;
            }
        }
    }
}

