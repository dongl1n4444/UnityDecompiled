namespace UnityEditor
{
    using Mono.Cecil;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor.Utils;
    using UnityEditorInternal;
    using UnityEngine;

    internal class MonoAOTRegistration
    {
        private static ArrayList BuildNativeMethodList(AssemblyDefinition[] assemblies)
        {
            ArrayList res = new ArrayList();
            foreach (AssemblyDefinition definition in assemblies)
            {
                if (!"System".Equals(definition.Name.Name))
                {
                    ExtractNativeMethodsFromTypes(definition.MainModule.Types, res);
                }
            }
            return res;
        }

        public static HashSet<string> BuildReferencedTypeList(AssemblyDefinition[] assemblies)
        {
            HashSet<string> set = new HashSet<string>();
            foreach (AssemblyDefinition definition in assemblies)
            {
                if (!definition.Name.Name.StartsWith("System") && !definition.Name.Name.Equals("UnityEngine"))
                {
                    foreach (TypeReference reference in definition.MainModule.GetTypeReferences())
                    {
                        set.Add(reference.FullName);
                    }
                }
            }
            return set;
        }

        private static void ExtractNativeMethodsFromTypes(ICollection<TypeDefinition> types, ArrayList res)
        {
            foreach (TypeDefinition definition in types)
            {
                foreach (MethodDefinition definition2 in definition.Methods)
                {
                    if ((definition2.IsStatic && definition2.IsPInvokeImpl) && definition2.PInvokeInfo.Module.Name.Equals("__Internal"))
                    {
                        if (res.Contains(definition2.Name))
                        {
                            throw new SystemException("Duplicate native method found : " + definition2.Name + ". Please check your source carefully.");
                        }
                        res.Add(definition2.Name);
                    }
                }
                if (definition.HasNestedTypes)
                {
                    ExtractNativeMethodsFromTypes(definition.NestedTypes, res);
                }
            }
        }

        private static void GenerateInternalCallMethod(TypeDefinition typeDefinition, MethodDefinition method, TextWriter output)
        {
            if (method.IsInternalCall)
            {
                string str = (typeDefinition.FullName + "_" + method.Name).Replace('/', '_').Replace('.', '_');
                if (!str.Contains("UnityEngine_Serialization"))
                {
                    output.WriteLine("\tvoid Register_{0} ();", str);
                    output.WriteLine("\tRegister_{0} ();", str);
                }
            }
        }

        public static void GenerateRegisterClasses(HashSet<UnityType> allClasses, TextWriter output)
        {
            output.WriteLine("void RegisterAllClasses() \n{");
            output.WriteLine("\tvoid RegisterAllClassesGranular();");
            output.WriteLine("\tRegisterAllClassesGranular();");
            output.WriteLine("}");
        }

        public static void GenerateRegisterClassesForStripping(HashSet<UnityType> nativeClassesAndBaseClasses, TextWriter output)
        {
            output.WriteLine("template <typename T> void RegisterClass();");
            output.WriteLine("template <typename T> void RegisterStrippedTypeInfo(int, const char*, const char*);");
            output.WriteLine();
            foreach (UnityType type in UnityType.GetTypes())
            {
                if ((type.baseClass != null) && !type.isEditorOnly)
                {
                    if (type.hasNativeNamespace)
                    {
                        output.WriteLine("class {0};", type.name);
                    }
                    else
                    {
                        output.WriteLine("namespace {0} {{ class {1}; }}", type.nativeNamespace, type.name);
                    }
                    output.WriteLine();
                }
            }
            output.Write("void RegisterAllClasses() \n{\n");
            output.WriteLine("\tvoid RegisterBuiltinTypes();");
            output.WriteLine("\tRegisterBuiltinTypes();");
            output.WriteLine("\t// Non stripped classes");
            foreach (UnityType type2 in UnityType.GetTypes())
            {
                if (((type2.baseClass != null) && !type2.isEditorOnly) && nativeClassesAndBaseClasses.Contains(type2))
                {
                    output.WriteLine("\tRegisterClass<{0}>();", type2.qualifiedName);
                }
            }
            output.WriteLine();
            output.Write("\n}\n");
        }

        public static void GenerateRegisterInternalCalls(AssemblyDefinition[] assemblies, TextWriter output)
        {
            output.Write("void RegisterAllStrippedInternalCalls ()\n{\n");
            foreach (AssemblyDefinition definition in assemblies)
            {
                GenerateRegisterInternalCallsForTypes(definition.MainModule.Types, output);
            }
            output.Write("}\n\n");
        }

        private static void GenerateRegisterInternalCallsForTypes(IEnumerable<TypeDefinition> types, TextWriter output)
        {
            foreach (TypeDefinition definition in types)
            {
                foreach (MethodDefinition definition2 in definition.Methods)
                {
                    GenerateInternalCallMethod(definition, definition2, output);
                }
                GenerateRegisterInternalCallsForTypes(definition.NestedTypes, output);
            }
        }

        public static void GenerateRegisterModules(HashSet<UnityType> nativeClasses, HashSet<string> nativeModules, TextWriter output, bool strippingEnabled)
        {
            output.WriteLine("void InvokeRegisterStaticallyLinkedModuleClasses()");
            output.WriteLine("{");
            if (nativeClasses == null)
            {
                output.WriteLine("\tvoid RegisterStaticallyLinkedModuleClasses();");
                output.WriteLine("\tRegisterStaticallyLinkedModuleClasses();");
            }
            else
            {
                output.WriteLine("\t// Do nothing (we're in stripping mode)");
            }
            output.WriteLine("}");
            output.WriteLine();
            output.WriteLine("void RegisterStaticallyLinkedModulesGranular()");
            output.WriteLine("{");
            foreach (string str in nativeModules)
            {
                output.WriteLine("\tvoid RegisterModule_" + str + "();");
                output.WriteLine("\tRegisterModule_" + str + "();");
                output.WriteLine();
            }
            output.WriteLine("}\n");
        }

        public static void ResolveDefinedNativeClassesFromMono(AssemblyDefinition[] assemblies, RuntimeClassRegistry res)
        {
            if (res != null)
            {
                foreach (AssemblyDefinition definition in assemblies)
                {
                    foreach (TypeDefinition definition2 in definition.MainModule.Types)
                    {
                        if (((definition2.Fields.Count > 0) || (definition2.Methods.Count > 0)) || (definition2.Properties.Count > 0))
                        {
                            string name = definition2.Name;
                            res.AddMonoClass(name);
                        }
                    }
                }
            }
        }

        public static void ResolveReferencedUnityEngineClassesFromMono(AssemblyDefinition[] assemblies, AssemblyDefinition unityEngine, RuntimeClassRegistry res)
        {
            if (res != null)
            {
                foreach (AssemblyDefinition definition in assemblies)
                {
                    if (definition != unityEngine)
                    {
                        foreach (TypeReference reference in definition.MainModule.GetTypeReferences())
                        {
                            if (reference.Namespace.StartsWith("UnityEngine"))
                            {
                                string name = reference.Name;
                                res.AddMonoClass(name);
                            }
                        }
                    }
                }
            }
        }

        public static void WriteCPlusPlusFileForStaticAOTModuleRegistration(BuildTarget buildTarget, string file, CrossCompileOptions crossCompileOptions, bool advancedLic, string targetDevice, bool stripping, RuntimeClassRegistry usedClassRegistry, AssemblyReferenceChecker checker, string stagingAreaDataManaged)
        {
            HashSet<UnityType> set;
            HashSet<string> set2;
            string str = Path.Combine(stagingAreaDataManaged, "ICallSummary.txt");
            string exe = Path.Combine(MonoInstallationFinder.GetFrameWorksFolder(), "Tools/InternalCallRegistrationWriter/InternalCallRegistrationWriter.exe");
            string args = string.Format("-assembly=\"{0}\" -summary=\"{1}\"", Path.Combine(stagingAreaDataManaged, "UnityEngine.dll"), str);
            Runner.RunManagedProgram(exe, args);
            CodeStrippingUtils.GenerateDependencies(Path.GetDirectoryName(stagingAreaDataManaged), str, usedClassRegistry, stripping, out set, out set2, null);
            using (TextWriter writer = new StreamWriter(file))
            {
                string[] assemblyFileNames = checker.GetAssemblyFileNames();
                AssemblyDefinition[] assemblyDefinitions = checker.GetAssemblyDefinitions();
                bool flag = (crossCompileOptions & CrossCompileOptions.FastICall) != CrossCompileOptions.Dynamic;
                ArrayList list = BuildNativeMethodList(assemblyDefinitions);
                if (buildTarget == BuildTarget.iOS)
                {
                    writer.WriteLine("#include \"RegisterMonoModules.h\"");
                    writer.WriteLine("#include <stdio.h>");
                }
                writer.WriteLine("");
                writer.WriteLine("#if defined(TARGET_IPHONE_SIMULATOR) && TARGET_IPHONE_SIMULATOR");
                writer.WriteLine("    #define DECL_USER_FUNC(f) void f() __attribute__((weak_import))");
                writer.WriteLine(@"    #define REGISTER_USER_FUNC(f)\");
                writer.WriteLine(@"        do {\");
                writer.WriteLine(@"        if(f != NULL)\");
                writer.WriteLine(@"            mono_dl_register_symbol(#f, (void*)f);\");
                writer.WriteLine(@"        else\");
                writer.WriteLine("            ::printf_console(\"Symbol '%s' not found. Maybe missing implementation for Simulator?\\n\", #f);\\");
                writer.WriteLine("        }while(0)");
                writer.WriteLine("#else");
                writer.WriteLine("    #define DECL_USER_FUNC(f) void f() ");
                writer.WriteLine("    #if !defined(__arm64__)");
                writer.WriteLine("    #define REGISTER_USER_FUNC(f) mono_dl_register_symbol(#f, (void*)&f)");
                writer.WriteLine("    #else");
                writer.WriteLine("        #define REGISTER_USER_FUNC(f)");
                writer.WriteLine("    #endif");
                writer.WriteLine("#endif");
                writer.WriteLine("extern \"C\"\n{");
                writer.WriteLine("    typedef void* gpointer;");
                writer.WriteLine("    typedef int gboolean;");
                if (buildTarget == BuildTarget.iOS)
                {
                    writer.WriteLine("    const char*         UnityIPhoneRuntimeVersion = \"{0}\";", Application.unityVersion);
                    writer.WriteLine("    void                mono_dl_register_symbol (const char* name, void *addr);");
                    writer.WriteLine("#if !defined(__arm64__)");
                    writer.WriteLine("    extern int          mono_ficall_flag;");
                    writer.WriteLine("#endif");
                }
                writer.WriteLine("    void                mono_aot_register_module(gpointer *aot_info);");
                writer.WriteLine("#if __ORBIS__ || SN_TARGET_PSP2");
                writer.WriteLine("#define DLL_EXPORT __declspec(dllexport)");
                writer.WriteLine("#else");
                writer.WriteLine("#define DLL_EXPORT");
                writer.WriteLine("#endif");
                writer.WriteLine("#if !(TARGET_IPHONE_SIMULATOR)");
                writer.WriteLine("    extern gboolean     mono_aot_only;");
                for (int i = 0; i < assemblyFileNames.Length; i++)
                {
                    string str4 = assemblyFileNames[i];
                    string str5 = assemblyDefinitions[i].Name.Name.Replace(".", "_").Replace("-", "_").Replace(" ", "_");
                    writer.WriteLine("    extern gpointer*    mono_aot_module_{0}_info; // {1}", str5, str4);
                }
                writer.WriteLine("#endif // !(TARGET_IPHONE_SIMULATOR)");
                IEnumerator enumerator = list.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        string current = (string) enumerator.Current;
                        writer.WriteLine("    DECL_USER_FUNC({0});", current);
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
                writer.WriteLine("}");
                writer.WriteLine("DLL_EXPORT void RegisterMonoModules()");
                writer.WriteLine("{");
                writer.WriteLine("#if !(TARGET_IPHONE_SIMULATOR) && !defined(__arm64__)");
                writer.WriteLine("    mono_aot_only = true;");
                if (buildTarget == BuildTarget.iOS)
                {
                    writer.WriteLine("    mono_ficall_flag = {0};", !flag ? "false" : "true");
                }
                foreach (AssemblyDefinition definition in assemblyDefinitions)
                {
                    string str7 = definition.Name.Name.Replace(".", "_").Replace("-", "_").Replace(" ", "_");
                    writer.WriteLine("    mono_aot_register_module(mono_aot_module_{0}_info);", str7);
                }
                writer.WriteLine("#endif // !(TARGET_IPHONE_SIMULATOR) && !defined(__arm64__)");
                writer.WriteLine("");
                if (buildTarget == BuildTarget.iOS)
                {
                    IEnumerator enumerator2 = list.GetEnumerator();
                    try
                    {
                        while (enumerator2.MoveNext())
                        {
                            string str8 = (string) enumerator2.Current;
                            writer.WriteLine("    REGISTER_USER_FUNC({0});", str8);
                        }
                    }
                    finally
                    {
                        IDisposable disposable2 = enumerator2 as IDisposable;
                        if (disposable2 != null)
                        {
                            disposable2.Dispose();
                        }
                    }
                }
                writer.WriteLine("}");
                writer.WriteLine("");
                AssemblyDefinition unityEngine = null;
                for (int j = 0; j < assemblyFileNames.Length; j++)
                {
                    if (assemblyFileNames[j] == "UnityEngine.dll")
                    {
                        unityEngine = assemblyDefinitions[j];
                    }
                }
                if (buildTarget == BuildTarget.iOS)
                {
                    AssemblyDefinition[] assemblies = new AssemblyDefinition[] { unityEngine };
                    GenerateRegisterInternalCalls(assemblies, writer);
                    ResolveDefinedNativeClassesFromMono(assemblies, usedClassRegistry);
                    ResolveReferencedUnityEngineClassesFromMono(assemblyDefinitions, unityEngine, usedClassRegistry);
                    GenerateRegisterModules(set, set2, writer, stripping);
                    if (stripping && (usedClassRegistry != null))
                    {
                        GenerateRegisterClassesForStripping(set, writer);
                    }
                    else
                    {
                        GenerateRegisterClasses(set, writer);
                    }
                }
                writer.Close();
            }
        }
    }
}

