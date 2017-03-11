namespace UnityEditor
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;

    internal class MonoAssemblyStripping
    {
        [CompilerGenerated]
        private static Func<string, AssemblyNameReference> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<AssemblyDefinition, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<AssemblyDefinition, IEnumerable<AssemblyDefinition>> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<AssemblyDefinition, bool> <>f__am$cache3;

        public static IEnumerable<AssemblyDefinition> CollectAllAssemblies(string librariesFolder, RuntimeClassRegistry usedClasses)
        {
            <CollectAllAssemblies>c__AnonStorey0 storey = new <CollectAllAssemblies>c__AnonStorey0 {
                usedClasses = usedClasses,
                resolver = new DefaultAssemblyResolver()
            };
            storey.resolver.RemoveSearchDirectory(".");
            storey.resolver.RemoveSearchDirectory("bin");
            storey.resolver.AddSearchDirectory(librariesFolder);
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = file => AssemblyNameReference.Parse(Path.GetFileNameWithoutExtension(file));
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = a => a != null;
            }
            return CollectAssembliesRecursive(Enumerable.Where<AssemblyDefinition>(Enumerable.Select<AssemblyNameReference, AssemblyDefinition>(Enumerable.Select<string, AssemblyNameReference>(Enumerable.Where<string>(storey.usedClasses.GetUserAssemblies(), new Func<string, bool>(storey.<>m__0)), <>f__am$cache0), new Func<AssemblyNameReference, AssemblyDefinition>(storey.<>m__1)), <>f__am$cache1));
        }

        private static HashSet<AssemblyDefinition> CollectAssembliesRecursive(IEnumerable<AssemblyDefinition> assemblies)
        {
            HashSet<AssemblyDefinition> source = new HashSet<AssemblyDefinition>(assemblies, new AssemblyDefinitionComparer());
            int count = 0;
            while (source.Count > count)
            {
                count = source.Count;
                if (<>f__am$cache2 == null)
                {
                    <>f__am$cache2 = a => ResolveAssemblyReferences(a);
                }
                source.UnionWith(Enumerable.SelectMany<AssemblyDefinition, AssemblyDefinition>(source.ToArray<AssemblyDefinition>(), <>f__am$cache2));
            }
            return source;
        }

        private static void CollectBlackListTypes(HashSet<TypeDefinition> typesToPreserve, IList<TypeDefinition> types, List<string> baseTypes)
        {
            if (types != null)
            {
                foreach (TypeDefinition definition in types)
                {
                    if (definition == null)
                    {
                        continue;
                    }
                    foreach (string str in baseTypes)
                    {
                        if (DoesTypeEnheritFrom(definition, str))
                        {
                            typesToPreserve.Add(definition);
                            break;
                        }
                    }
                    CollectBlackListTypes(typesToPreserve, definition.NestedTypes, baseTypes);
                }
            }
        }

        private static bool DoesTypeEnheritFrom(TypeReference type, string typeName)
        {
            while (type != null)
            {
                if (type.FullName == typeName)
                {
                    return true;
                }
                TypeDefinition definition = type.Resolve();
                if (definition == null)
                {
                    return false;
                }
                type = definition.BaseType;
            }
            return false;
        }

        public static string GenerateLinkXmlToPreserveDerivedTypes(string stagingArea, string librariesFolder, RuntimeClassRegistry usedClasses)
        {
            string fullPath = Path.GetFullPath(Path.Combine(stagingArea, "preserved_derived_types.xml"));
            using (TextWriter writer = new StreamWriter(fullPath))
            {
                writer.WriteLine("<linker>");
                foreach (AssemblyDefinition definition in CollectAllAssemblies(librariesFolder, usedClasses))
                {
                    if (definition.Name.Name != "UnityEngine")
                    {
                        HashSet<TypeDefinition> typesToPreserve = new HashSet<TypeDefinition>();
                        CollectBlackListTypes(typesToPreserve, definition.MainModule.Types, usedClasses.GetAllManagedBaseClassesAsString());
                        if (typesToPreserve.Count != 0)
                        {
                            writer.WriteLine("<assembly fullname=\"{0}\">", definition.Name.Name);
                            foreach (TypeDefinition definition2 in typesToPreserve)
                            {
                                writer.WriteLine("<type fullname=\"{0}\" preserve=\"all\"/>", definition2.FullName);
                            }
                            writer.WriteLine("</assembly>");
                        }
                    }
                }
                writer.WriteLine("</linker>");
            }
            return fullPath;
        }

        public static void MonoCilStrip(BuildTarget buildTarget, string managedLibrariesDirectory, string[] fileNames)
        {
            string str2 = Path.Combine(BuildPipeline.GetBuildToolsDirectory(buildTarget), "mono-cil-strip.exe");
            foreach (string str3 in fileNames)
            {
                Process process = MonoProcessUtility.PrepareMonoProcessBleedingEdge(managedLibrariesDirectory);
                string str4 = str3 + ".out";
                process.StartInfo.Arguments = "\"" + str2 + "\"";
                ProcessStartInfo startInfo = process.StartInfo;
                string arguments = startInfo.Arguments;
                string[] textArray1 = new string[] { arguments, " \"", str3, "\" \"", str3, ".out\"" };
                startInfo.Arguments = string.Concat(textArray1);
                MonoProcessUtility.RunMonoProcess(process, "byte code stripper", Path.Combine(managedLibrariesDirectory, str4));
                ReplaceFile(managedLibrariesDirectory + "/" + str4, managedLibrariesDirectory + "/" + str3);
                File.Delete(managedLibrariesDirectory + "/" + str4);
            }
        }

        private static void ReplaceFile(string src, string dst)
        {
            if (File.Exists(dst))
            {
                FileUtil.DeleteFileOrDirectory(dst);
            }
            FileUtil.CopyFileOrDirectory(src, dst);
        }

        public static AssemblyDefinition ResolveAssemblyReference(IAssemblyResolver resolver, AssemblyNameReference assemblyName)
        {
            try
            {
                ReaderParameters parameters = new ReaderParameters {
                    AssemblyResolver = resolver,
                    ApplyWindowsRuntimeProjections = true
                };
                return resolver.Resolve(assemblyName, parameters);
            }
            catch (AssemblyResolutionException exception)
            {
                if (!exception.AssemblyReference.IsWindowsRuntime)
                {
                    throw;
                }
                return null;
            }
        }

        public static IEnumerable<AssemblyDefinition> ResolveAssemblyReferences(AssemblyDefinition assembly) => 
            ResolveAssemblyReferences(assembly.MainModule.AssemblyResolver, assembly.MainModule.AssemblyReferences);

        public static IEnumerable<AssemblyDefinition> ResolveAssemblyReferences(IAssemblyResolver resolver, IEnumerable<AssemblyNameReference> assemblyReferences)
        {
            <ResolveAssemblyReferences>c__AnonStorey1 storey = new <ResolveAssemblyReferences>c__AnonStorey1 {
                resolver = resolver
            };
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = a => a != null;
            }
            return Enumerable.Where<AssemblyDefinition>(Enumerable.Select<AssemblyNameReference, AssemblyDefinition>(assemblyReferences, new Func<AssemblyNameReference, AssemblyDefinition>(storey.<>m__0)), <>f__am$cache3);
        }

        [CompilerGenerated]
        private sealed class <CollectAllAssemblies>c__AnonStorey0
        {
            internal DefaultAssemblyResolver resolver;
            internal RuntimeClassRegistry usedClasses;

            internal bool <>m__0(string s) => 
                this.usedClasses.IsDLLUsed(s);

            internal AssemblyDefinition <>m__1(AssemblyNameReference dll) => 
                MonoAssemblyStripping.ResolveAssemblyReference(this.resolver, dll);
        }

        [CompilerGenerated]
        private sealed class <ResolveAssemblyReferences>c__AnonStorey1
        {
            internal IAssemblyResolver resolver;

            internal AssemblyDefinition <>m__0(AssemblyNameReference reference) => 
                MonoAssemblyStripping.ResolveAssemblyReference(this.resolver, reference);
        }

        private class AssemblyDefinitionComparer : IEqualityComparer<AssemblyDefinition>
        {
            public bool Equals(AssemblyDefinition x, AssemblyDefinition y) => 
                (x.FullName == y.FullName);

            public int GetHashCode(AssemblyDefinition obj) => 
                obj.FullName.GetHashCode();
        }
    }
}

