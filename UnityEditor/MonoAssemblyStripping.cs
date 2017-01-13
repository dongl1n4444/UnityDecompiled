namespace UnityEditor
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor.Utils;

    internal class MonoAssemblyStripping
    {
        [CompilerGenerated]
        private static Func<AssemblyDefinition, IEnumerable<AssemblyDefinition>> <>f__am$cache0;

        private static HashSet<AssemblyDefinition> CollectAssembliesRecursive(IEnumerable<AssemblyDefinition> assemblies)
        {
            HashSet<AssemblyDefinition> source = new HashSet<AssemblyDefinition>(assemblies, new AssemblyDefinitionComparer());
            int count = 0;
            while (source.Count > count)
            {
                count = source.Count;
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = delegate (AssemblyDefinition assembly) {
                        <CollectAssembliesRecursive>c__AnonStorey1 storey = new <CollectAssembliesRecursive>c__AnonStorey1 {
                            assembly = assembly
                        };
                        return Enumerable.Select<AssemblyNameReference, AssemblyDefinition>(storey.assembly.MainModule.AssemblyReferences, new Func<AssemblyNameReference, AssemblyDefinition>(storey.<>m__0));
                    };
                }
                source.UnionWith(Enumerable.SelectMany<AssemblyDefinition, AssemblyDefinition>(source.ToArray<AssemblyDefinition>(), <>f__am$cache0));
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

        private static void CopyAllDlls(string fromDir, string toDir)
        {
            FileInfo[] files = new DirectoryInfo(toDir).GetFiles("*.dll");
            foreach (FileInfo info2 in files)
            {
                FileUtil.ReplaceFile(Path.Combine(toDir, info2.Name), Path.Combine(fromDir, info2.Name));
            }
        }

        private static void CopyFiles(IEnumerable<string> files, string fromDir, string toDir)
        {
            foreach (string str in files)
            {
                FileUtil.ReplaceFile(Path.Combine(fromDir, str), Path.Combine(toDir, str));
            }
        }

        private static void DeleteAllDllsFrom(string managedLibrariesDirectory)
        {
            FileInfo[] files = new DirectoryInfo(managedLibrariesDirectory).GetFiles("*.dll");
            foreach (FileInfo info2 in files)
            {
                FileUtil.DeleteFileOrDirectory(info2.FullName);
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

        public static string GenerateBlackList(string librariesFolder, RuntimeClassRegistry usedClasses, string[] allAssemblies)
        {
            string str = "tmplink.xml";
            usedClasses.SynchronizeClasses();
            using (TextWriter writer = new StreamWriter(Path.Combine(librariesFolder, str)))
            {
                writer.WriteLine("<linker>");
                writer.WriteLine("<assembly fullname=\"UnityEngine\">");
                foreach (string str2 in usedClasses.GetAllManagedClassesAsString())
                {
                    writer.WriteLine($"<type fullname="UnityEngine.{str2}" preserve="{usedClasses.GetRetentionLevel(str2)}"/>");
                }
                writer.WriteLine("</assembly>");
                DefaultAssemblyResolver resolver = new DefaultAssemblyResolver();
                resolver.AddSearchDirectory(librariesFolder);
                foreach (string str3 in allAssemblies)
                {
                    ReaderParameters parameters = new ReaderParameters {
                        AssemblyResolver = resolver
                    };
                    AssemblyDefinition definition = resolver.Resolve(Path.GetFileNameWithoutExtension(str3), parameters);
                    writer.WriteLine("<assembly fullname=\"{0}\">", definition.Name.Name);
                    if (definition.Name.Name.StartsWith("UnityEngine."))
                    {
                        foreach (string str4 in usedClasses.GetAllManagedClassesAsString())
                        {
                            writer.WriteLine($"<type fullname="UnityEngine.{str4}" preserve="{usedClasses.GetRetentionLevel(str4)}"/>");
                        }
                    }
                    GenerateBlackListTypeXML(writer, definition.MainModule.Types, usedClasses.GetAllManagedBaseClassesAsString());
                    writer.WriteLine("</assembly>");
                }
                writer.WriteLine("</linker>");
            }
            return str;
        }

        private static void GenerateBlackListTypeXML(TextWriter w, IList<TypeDefinition> types, List<string> baseTypes)
        {
            HashSet<TypeDefinition> typesToPreserve = new HashSet<TypeDefinition>();
            CollectBlackListTypes(typesToPreserve, types, baseTypes);
            foreach (TypeDefinition definition in typesToPreserve)
            {
                w.WriteLine("<type fullname=\"{0}\" preserve=\"all\"/>", definition.FullName);
            }
        }

        public static string GenerateLinkXmlToPreserveDerivedTypes(string stagingArea, string librariesFolder, RuntimeClassRegistry usedClasses)
        {
            <GenerateLinkXmlToPreserveDerivedTypes>c__AnonStorey0 storey = new <GenerateLinkXmlToPreserveDerivedTypes>c__AnonStorey0 {
                usedClasses = usedClasses
            };
            string fullPath = Path.GetFullPath(Path.Combine(stagingArea, "preserved_derived_types.xml"));
            storey.resolver = new DefaultAssemblyResolver();
            storey.resolver.AddSearchDirectory(librariesFolder);
            using (TextWriter writer = new StreamWriter(fullPath))
            {
                writer.WriteLine("<linker>");
                foreach (AssemblyDefinition definition in CollectAssembliesRecursive(Enumerable.Select<string, AssemblyDefinition>(Enumerable.Where<string>(storey.usedClasses.GetUserAssemblies(), new Func<string, bool>(storey.<>m__0)), new Func<string, AssemblyDefinition>(storey.<>m__1))))
                {
                    if (definition.Name.Name != "UnityEngine")
                    {
                        HashSet<TypeDefinition> typesToPreserve = new HashSet<TypeDefinition>();
                        CollectBlackListTypes(typesToPreserve, definition.MainModule.Types, storey.usedClasses.GetAllManagedBaseClassesAsString());
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
                Process process = MonoProcessUtility.PrepareMonoProcess(buildTarget, managedLibrariesDirectory);
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

        public static void MonoLink(BuildTarget buildTarget, string managedLibrariesDirectory, string[] input, string[] allAssemblies, RuntimeClassRegistry usedClasses)
        {
            Process process = MonoProcessUtility.PrepareMonoProcess(buildTarget, managedLibrariesDirectory);
            string buildToolsDirectory = BuildPipeline.GetBuildToolsDirectory(buildTarget);
            string str2 = null;
            string path = Path.Combine(MonoInstallationFinder.GetFrameWorksFolder(), StripperExe());
            string str5 = Path.Combine(Path.GetDirectoryName(path), "link.xml");
            string str6 = Path.Combine(managedLibrariesDirectory, "output");
            Directory.CreateDirectory(str6);
            process.StartInfo.Arguments = "\"" + path + "\" -l none -c link";
            foreach (string str7 in input)
            {
                ProcessStartInfo info1 = process.StartInfo;
                info1.Arguments = info1.Arguments + " -a \"" + str7 + "\"";
            }
            ProcessStartInfo startInfo = process.StartInfo;
            string arguments = startInfo.Arguments;
            string[] textArray1 = new string[] { arguments, " -out output -x \"", str5, "\" -d \"", managedLibrariesDirectory, "\"" };
            startInfo.Arguments = string.Concat(textArray1);
            string str9 = Path.Combine(buildToolsDirectory, "link.xml");
            if (File.Exists(str9))
            {
                ProcessStartInfo info3 = process.StartInfo;
                info3.Arguments = info3.Arguments + " -x \"" + str9 + "\"";
            }
            string str10 = Path.Combine(Path.GetDirectoryName(path), "Core.xml");
            if (File.Exists(str10))
            {
                ProcessStartInfo info4 = process.StartInfo;
                info4.Arguments = info4.Arguments + " -x \"" + str10 + "\"";
            }
            string[] strArray2 = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "Assets"), "link.xml", SearchOption.AllDirectories);
            foreach (string str11 in strArray2)
            {
                ProcessStartInfo info5 = process.StartInfo;
                info5.Arguments = info5.Arguments + " -x \"" + str11 + "\"";
            }
            if (usedClasses != null)
            {
                str2 = GenerateBlackList(managedLibrariesDirectory, usedClasses, allAssemblies);
                ProcessStartInfo info6 = process.StartInfo;
                info6.Arguments = info6.Arguments + " -x \"" + str2 + "\"";
            }
            string str12 = Path.Combine(BuildPipeline.GetPlaybackEngineDirectory(EditorUserBuildSettings.activeBuildTarget, BuildOptions.CompressTextures), "Whitelists");
            foreach (string str13 in Directory.GetFiles(str12, "*.xml"))
            {
                ProcessStartInfo info7 = process.StartInfo;
                info7.Arguments = info7.Arguments + " -x \"" + str13 + "\"";
            }
            MonoProcessUtility.RunMonoProcess(process, "assemblies stripper", Path.Combine(str6, "mscorlib.dll"));
            DeleteAllDllsFrom(managedLibrariesDirectory);
            CopyAllDlls(managedLibrariesDirectory, str6);
            foreach (string str14 in Directory.GetFiles(managedLibrariesDirectory))
            {
                if (str14.Contains(".mdb") && !File.Exists(str14.Replace(".mdb", "")))
                {
                    FileUtil.DeleteFileOrDirectory(str14);
                }
            }
            if (str2 != null)
            {
                FileUtil.DeleteFileOrDirectory(Path.Combine(managedLibrariesDirectory, str2));
            }
            FileUtil.DeleteFileOrDirectory(str6);
        }

        private static void ReplaceFile(string src, string dst)
        {
            if (File.Exists(dst))
            {
                FileUtil.DeleteFileOrDirectory(dst);
            }
            FileUtil.CopyFileOrDirectory(src, dst);
        }

        private static string StripperExe() => 
            "Tools/UnusedBytecodeStripper.exe";

        [CompilerGenerated]
        private sealed class <CollectAssembliesRecursive>c__AnonStorey1
        {
            internal AssemblyDefinition assembly;

            internal AssemblyDefinition <>m__0(AssemblyNameReference a) => 
                this.assembly.MainModule.AssemblyResolver.Resolve(a);
        }

        [CompilerGenerated]
        private sealed class <GenerateLinkXmlToPreserveDerivedTypes>c__AnonStorey0
        {
            internal DefaultAssemblyResolver resolver;
            internal RuntimeClassRegistry usedClasses;

            internal bool <>m__0(string s) => 
                this.usedClasses.IsDLLUsed(s);

            internal AssemblyDefinition <>m__1(string file)
            {
                ReaderParameters parameters = new ReaderParameters {
                    AssemblyResolver = this.resolver
                };
                return this.resolver.Resolve(Path.GetFileNameWithoutExtension(file), parameters);
            }
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

