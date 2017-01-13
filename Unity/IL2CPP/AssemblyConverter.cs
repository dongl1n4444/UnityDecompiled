namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using NiceIO;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Unity.Cecil.Visitor;
    using Unity.IL2CPP.Common;
    using Unity.IL2CPP.Debugger;
    using Unity.IL2CPP.GenericsCollection;
    using Unity.IL2CPP.GenericSharing;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Metadata;
    using Unity.IL2CPP.StringLiterals;
    using Unity.TinyProfiling;

    public class AssemblyConverter
    {
        private readonly NPath[] _assemblies;
        private List<AssemblyDefinition> _assembliesOrderedByDependency = new List<AssemblyDefinition>();
        private readonly AssemblyLoader _assemblyLoader;
        private readonly NPath _dataFolder;
        private readonly IDebuggerSupport _debuggerSupport;
        private readonly InterfaceInvokerCollector _genericInterfaceInvokerCollector = new InterfaceInvokerCollector(true);
        private readonly VirtualInvokerCollector _genericVirtualInvokerCollector = new VirtualInvokerCollector(true);
        private readonly InterfaceInvokerCollector _interfaceInvokerCollector = new InterfaceInvokerCollector(false);
        private readonly NPath _outputDir;
        private readonly StringLiteralCollector _stringLiteralCollector = new StringLiteralCollector();
        private readonly VirtualInvokerCollector _virtualInvokerCollector = new VirtualInvokerCollector(false);
        private readonly VTableBuilder _vTableBuilder = new VTableBuilder();
        [CompilerGenerated]
        private static Func<NPath, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<AssemblyDefinition, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<AssemblyDefinition, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<AssemblyDefinition, bool> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<AssemblyDefinition, bool> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<AssemblyDefinition, IEnumerable<TypeDefinition>> <>f__am$cache5;
        [CompilerGenerated]
        private static Func<string, string> <>f__am$cache6;
        [CompilerGenerated]
        private static Func<string, bool> <>f__am$cache7;
        [Inject]
        public static IAssemblyDependencies AssemblyDependencies;
        [Inject]
        public static ITypeProviderService TypeProvider;
        [Inject]
        public static ITypeProviderInitializerService TypeProviderInitializer;
        [Inject]
        public static IVirtualCallCollectorService VirtualCallCollector;
        [Inject]
        public static IWindowsRuntimeProjectionsInitializer WindowsRuntimeProjectionsInitializer;

        protected AssemblyConverter(NPath[] assemblies, NPath outputDir, NPath dataFolder, DotNetProfile dotNetProfile)
        {
            this._assemblies = assemblies;
            this._outputDir = outputDir;
            this._dataFolder = dataFolder;
            this._debuggerSupport = DebuggerSupportFactory.GetDebuggerSupport();
            NPath[] searchDirectories = new NPath[] { assemblies[0].Parent, MonoInstall.SmartProfilePath(dotNetProfile) };
            bool applyWindowsRuntimeProjections = CodeGenOptions.Dotnetprofile == DotNetProfile.Net45;
            this._assemblyLoader = new AssemblyLoader(searchDirectories, DebuggerOptions.Enabled || CodeGenOptions.EnableSymbolLoading, applyWindowsRuntimeProjections);
        }

        private void AddExtraTypes(InflatedCollectionCollector genericsCollectionCollector)
        {
            ExtraTypesSupport support = new ExtraTypesSupport(genericsCollectionCollector, this._assembliesOrderedByDependency);
            foreach (string str in BuildExtraTypesList())
            {
                TypeNameParseInfo typeNameInfo = Unity.IL2CPP.Common.TypeNameParser.Parse(str);
                if (typeNameInfo == null)
                {
                    Console.WriteLine("WARNING: Cannot parse type name {0} from the extra types list. Skipping.", str);
                }
                else if (!support.AddType(str, typeNameInfo))
                {
                    Console.WriteLine("WARNING: Cannot add extra type {0}. Skipping.", str);
                }
            }
        }

        private void Apply()
        {
            InflatedCollectionCollector collector;
            TypeDefinition[] definitionArray;
            using (TinyProfiler.Section("PreProcessStage", ""))
            {
                this.PreProcessStage(out collector, out definitionArray);
            }
            MethodCollector methodCollector = new MethodCollector();
            AttributeCollection attributeCollection = new AttributeCollection();
            MetadataCollector metadataCollection = new MetadataCollector();
            using (TinyProfiler.Section("MetadataCollector", ""))
            {
                metadataCollection.AddAssemblies(this._assembliesOrderedByDependency);
            }
            using (TinyProfiler.Section("AllAssemblyConversion", ""))
            {
                foreach (AssemblyDefinition definition in this._assembliesOrderedByDependency)
                {
                    using (TinyProfiler.Section("Convert", definition.Name.Name))
                    {
                        this.Convert(definition, collector, attributeCollection, methodCollector, metadataCollection);
                    }
                }
            }
            methodCollector.Complete();
            using (TinyProfiler.Section("WriteGenerics", ""))
            {
                this.WriteGenerics(collector.AsReadOnly(), definitionArray, metadataCollection);
            }
            using (TinyProfiler.Section("VariousInvokers", ""))
            {
                this._virtualInvokerCollector.Write(this.GetPathFor("GeneratedVirtualInvokers", "h"));
                this._genericVirtualInvokerCollector.Write(this.GetPathFor("GeneratedGenericVirtualInvokers", "h"));
                this._interfaceInvokerCollector.Write(this.GetPathFor("GeneratedInterfaceInvokers", "h"));
                this._genericInterfaceInvokerCollector.Write(this.GetPathFor("GeneratedGenericInterfaceInvokers", "h"));
            }
            using (TinyProfiler.Section("Metadata", "Global"))
            {
                SourceWriter.WriteCollectedMetadata(collector, this._assembliesOrderedByDependency, this._outputDir, this._dataFolder, metadataCollection, attributeCollection, this._vTableBuilder, methodCollector);
            }
            using (TinyProfiler.Section("Copy Etc", ""))
            {
                if (CodeGenOptions.Dotnetprofile == DotNetProfile.Net45)
                {
                    MonoInstall.BleedingEdge.ConfigPath.Copy(this._dataFolder);
                }
                else
                {
                    MonoInstall.TwoSix.ConfigPath.Copy(this._dataFolder);
                }
            }
            this._debuggerSupport.GenerateSupportFilesIfNeeded(this._outputDir);
        }

        private void ApplyDefaultMarshalAsAttribute()
        {
            ApplyDefaultMarshalAsAttributeVisitor visitor = new ApplyDefaultMarshalAsAttributeVisitor();
            foreach (AssemblyDefinition definition in this._assembliesOrderedByDependency)
            {
                using (TinyProfiler.Section("ApplyDefaultMarshalAsAttributeVisitor in assembly", definition.Name.Name))
                {
                    definition.Accept(visitor);
                }
            }
        }

        private static IEnumerable<string> BuildExtraTypesList()
        {
            HashSet<string> set = new HashSet<string>();
            if (ExtraTypesOptions.Name != null)
            {
                foreach (string str in ExtraTypesOptions.Name)
                {
                    set.Add(str);
                }
            }
            if (ExtraTypesOptions.File != null)
            {
                foreach (string str2 in ExtraTypesOptions.File)
                {
                    try
                    {
                        if (<>f__am$cache6 == null)
                        {
                            <>f__am$cache6 = l => l.Trim();
                        }
                        if (<>f__am$cache7 == null)
                        {
                            <>f__am$cache7 = l => l.Length > 0;
                        }
                        foreach (string str3 in File.ReadAllLines(str2).Select<string, string>(<>f__am$cache6).Where<string>(<>f__am$cache7))
                        {
                            if ((!str3.StartsWith(";") && !str3.StartsWith("#")) && !str3.StartsWith("//"))
                            {
                                set.Add(str3);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("WARNING: Cannot open extra file list {0}. Skipping.", str2);
                    }
                }
            }
            return set;
        }

        private HashSet<AssemblyDefinition> CollectAssembliesRecursive(IEnumerable<AssemblyDefinition> assemblies)
        {
            HashSet<AssemblyDefinition> source = new HashSet<AssemblyDefinition>(assemblies, new AssemblyDefinitionComparer());
            int count = 0;
            while (source.Count > count)
            {
                count = source.Count;
                source.UnionWith(source.ToArray<AssemblyDefinition>().SelectMany<AssemblyDefinition, AssemblyDefinition>(new Func<AssemblyDefinition, IEnumerable<AssemblyDefinition>>(AssemblyDependencies.GetReferencedAssembliesFor)));
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = a => a.Name.Name == "mscorlib";
            }
            if (!source.Any<AssemblyDefinition>(<>f__am$cache1))
            {
                AssemblyDefinition[] other = new AssemblyDefinition[] { this._assemblyLoader.Load("mscorlib") };
                source.UnionWith(other);
            }
            return source;
        }

        private void CollectAssembliesToConvert()
        {
            this._assembliesOrderedByDependency = this.CollectAssembliesRecursive(from path in this._assemblies select this._assemblyLoader.Load(path.ToString())).ToList<AssemblyDefinition>();
            this._assembliesOrderedByDependency.Sort(new AssemblyDependencyComparer(AssemblyDependencyComparer.MaximumDepthForEachAssembly(this._assembliesOrderedByDependency)));
        }

        private void CollectGenericVirtualMethods(InflatedCollectionCollector allGenerics, IEnumerable<TypeDefinition> allTypeDefinitions)
        {
            new GenericVirtualMethodCollector().Collect(allGenerics, allTypeDefinitions, this._vTableBuilder);
        }

        private void Convert(AssemblyDefinition assemblyDefinition, InflatedCollectionCollector allGenerics, AttributeCollection attributeCollection, MethodCollector methodCollector, IMetadataCollection metadataCollection)
        {
            TypeDefinition[] typeList = GetAllTypes(assemblyDefinition.MainModule.Types).ToArray<TypeDefinition>();
            new SourceWriter(this._vTableBuilder, this._debuggerSupport, this._outputDir).Write(assemblyDefinition, allGenerics, this._outputDir, typeList, attributeCollection, methodCollector, metadataCollection);
        }

        public static void ConvertAssemblies(NPath[] assemblies, NPath outputDir, NPath dataFolder)
        {
            try
            {
                new AssemblyConverter(assemblies, outputDir, dataFolder, CodeGenOptions.Dotnetprofile).Apply();
            }
            catch (Exception exception)
            {
                Console.WriteLine(ErrorMessageWriter.FormatMessage(ErrorInformation.CurrentlyProcessing, exception.Message));
                throw;
            }
        }

        public static IEnumerable<NPath> ConvertAssemblies(IEnumerable<string> assemblyDirectories, IEnumerable<NPath> explicitAssemblies, NPath outputDir, NPath dataFolder)
        {
            List<NPath> list = new List<NPath>();
            if (assemblyDirectories != null)
            {
                foreach (string str in assemblyDirectories)
                {
                    list.AddRange(GetAssembliesInDirectory(str.ToNPath()));
                }
            }
            if (explicitAssemblies != null)
            {
                list.AddRange(explicitAssemblies);
            }
            ConvertAssemblies(list.ToArray(), outputDir, dataFolder);
            return list;
        }

        [DebuggerHidden]
        public static IEnumerable<TypeDefinition> GetAllTypes(IEnumerable<TypeDefinition> typeDefinitions) => 
            new <GetAllTypes>c__Iterator0 { 
                typeDefinitions = typeDefinitions,
                $PC = -2
            };

        private static IEnumerable<NPath> GetAssembliesInDirectory(NPath assemblyDirectory)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = f => f.HasExtension(new string[] { "dll", "exe" });
            }
            return assemblyDirectory.Files(false).Where<NPath>(<>f__am$cache0);
        }

        private NPath GetPathFor(string filename, string extension)
        {
            string[] append = new string[] { filename };
            return this._outputDir.Combine(append).ChangeExtension(extension);
        }

        private void InjectBaseTypesAndFinalizersIntoComAndWindowsRuntimeTypes()
        {
            InjectBaseTypesAndFinalizersIntoComAndWindowsRuntimeTypesVisitor visitor = new InjectBaseTypesAndFinalizersIntoComAndWindowsRuntimeTypesVisitor();
            foreach (AssemblyDefinition definition in this._assembliesOrderedByDependency)
            {
                using (TinyProfiler.Section("ModifyCOMAndWindowsRuntimeTypes in assembly", definition.Name.Name))
                {
                    definition.Accept(visitor);
                }
            }
        }

        private void PatchEnumsNestedInGenericTypes()
        {
            PatchEnumsNestedInGenericTypesVisitor visitor = new PatchEnumsNestedInGenericTypesVisitor();
            foreach (AssemblyDefinition definition in this._assembliesOrderedByDependency)
            {
                using (TinyProfiler.Section("PatchEnumsNestedInGenericTypes in assembly", definition.Name.Name))
                {
                    definition.Accept(visitor);
                }
            }
        }

        private void PreProcessStage(out InflatedCollectionCollector genericsCollectionCollector, out TypeDefinition[] allTypeDefinitions)
        {
            using (TinyProfiler.Section("Collect assemblies to convert", ""))
            {
                this.CollectAssembliesToConvert();
            }
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = ad => ad.Name.Name == "mscorlib";
            }
            AssemblyDefinition mscorlib = this._assembliesOrderedByDependency.Single<AssemblyDefinition>(<>f__am$cache2);
            TypeProviderInitializer.Initialize(mscorlib);
            WindowsRuntimeProjectionsInitializer.Initialize(mscorlib.MainModule, CodeGenOptions.Dotnetprofile);
            this._outputDir.EnsureDirectoryExists("");
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = a => (a.MainModule.Kind == ModuleKind.Windows) || (a.MainModule.Kind == ModuleKind.Console);
            }
            AssemblyDefinition[] source = this._assembliesOrderedByDependency.Where<AssemblyDefinition>(<>f__am$cache3).ToArray<AssemblyDefinition>();
            if (source.Length > 0)
            {
                if (<>f__am$cache4 == null)
                {
                    <>f__am$cache4 = a => a.EntryPoint != null;
                }
                AssemblyDefinition local1 = source.FirstOrDefault<AssemblyDefinition>(<>f__am$cache4);
                if (local1 == null)
                {
                }
                new DriverWriter(source.First<AssemblyDefinition>()).Write(this._outputDir);
            }
            using (TinyProfiler.Section("PatchEnumsNestedInGenericTypes", ""))
            {
                this.PatchEnumsNestedInGenericTypes();
            }
            using (TinyProfiler.Section("Inject base types and finalizers into COM and Windows Runtime types", ""))
            {
                this.InjectBaseTypesAndFinalizersIntoComAndWindowsRuntimeTypes();
            }
            using (TinyProfiler.Section("ApplyDefaultMarshalAsAttribute", ""))
            {
                this.ApplyDefaultMarshalAsAttribute();
            }
            foreach (AssemblyDefinition definition3 in this._assembliesOrderedByDependency)
            {
                this._stringLiteralCollector.Process(definition3);
            }
            foreach (AssemblyDefinition definition4 in this._assembliesOrderedByDependency)
            {
                this._debuggerSupport.Analyze(definition4);
            }
            foreach (AssemblyDefinition definition5 in this._assembliesOrderedByDependency)
            {
                definition5.Accept(new GenericSharingVisitor());
            }
            using (TinyProfiler.Section("WriteResources", ""))
            {
                this.WriteEmbeddedResourcesForEachAssembly();
            }
            foreach (AssemblyDefinition definition6 in this._assembliesOrderedByDependency)
            {
                using (TinyProfiler.Section("VirtualInvokeCollector", definition6.Name.Name))
                {
                    this._virtualInvokerCollector.Process(definition6);
                }
            }
            foreach (AssemblyDefinition definition7 in this._assembliesOrderedByDependency)
            {
                using (TinyProfiler.Section("GenericVirtualInvokerCollector", definition7.Name.Name))
                {
                    this._genericVirtualInvokerCollector.Process(definition7);
                }
            }
            foreach (AssemblyDefinition definition8 in this._assembliesOrderedByDependency)
            {
                using (TinyProfiler.Section("InterfaceInvokerCollector", definition8.Name.Name))
                {
                    this._interfaceInvokerCollector.Process(definition8);
                }
            }
            foreach (AssemblyDefinition definition9 in this._assembliesOrderedByDependency)
            {
                using (TinyProfiler.Section("GenericInterfaceInvokerCollector", definition9.Name.Name))
                {
                    this._genericInterfaceInvokerCollector.Process(definition9);
                }
            }
            using (TinyProfiler.Section("GenericsCollector.Collect", ""))
            {
                genericsCollectionCollector = GenericsCollector.Collect(this._assembliesOrderedByDependency);
            }
            this.AddExtraTypes(genericsCollectionCollector);
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = a => a.MainModule.Types;
            }
            allTypeDefinitions = GetAllTypes(this._assembliesOrderedByDependency.SelectMany<AssemblyDefinition, TypeDefinition>(<>f__am$cache5)).ToArray<TypeDefinition>();
            using (TinyProfiler.Section("CollectGenericVirtualMethods.Collect", ""))
            {
                this.CollectGenericVirtualMethods(genericsCollectionCollector, allTypeDefinitions);
            }
        }

        private void WriteEmbeddedResourcesForEachAssembly()
        {
            string[] append = new string[] { "Resources" };
            NPath path = this._dataFolder.Combine(append);
            path.CreateDirectory();
            foreach (AssemblyDefinition definition in this._assembliesOrderedByDependency)
            {
                if (definition.MainModule.Resources.Any<Resource>())
                {
                    string[] textArray2 = new string[] { $"{definition.MainModule.Name}-resources.dat" };
                    using (FileStream stream = new FileStream(path.Combine(textArray2).ToString(), FileMode.Create, FileAccess.Write))
                    {
                        ResourceWriter.WriteEmbeddedResources(definition, stream);
                    }
                }
            }
        }

        private void WriteGenerics(ReadOnlyInflatedCollectionCollector allGenerics, IEnumerable<TypeDefinition> allTypeDefinitions, IMetadataCollection metadataCollection)
        {
            new SourceWriter(this._vTableBuilder, this._debuggerSupport, this._outputDir).WriteGenerics(allGenerics, allTypeDefinitions, metadataCollection);
        }

        [CompilerGenerated]
        private sealed class <GetAllTypes>c__Iterator0 : IEnumerable, IEnumerable<TypeDefinition>, IEnumerator, IDisposable, IEnumerator<TypeDefinition>
        {
            internal TypeDefinition $current;
            internal bool $disposing;
            internal IEnumerator<TypeDefinition> $locvar0;
            internal IEnumerator<TypeDefinition> $locvar1;
            internal int $PC;
            internal TypeDefinition <nestedType>__1;
            internal TypeDefinition <typeDefinition>__0;
            internal IEnumerable<TypeDefinition> typeDefinitions;

            [DebuggerHidden]
            public void Dispose()
            {
                uint num = (uint) this.$PC;
                this.$disposing = true;
                this.$PC = -1;
                switch (num)
                {
                    case 1:
                    case 2:
                        try
                        {
                            switch (num)
                            {
                                case 1:
                                    try
                                    {
                                    }
                                    finally
                                    {
                                        if (this.$locvar1 != null)
                                        {
                                            this.$locvar1.Dispose();
                                        }
                                    }
                                    return;
                            }
                        }
                        finally
                        {
                            if (this.$locvar0 != null)
                            {
                                this.$locvar0.Dispose();
                            }
                        }
                        break;
                }
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                bool flag = false;
                switch (num)
                {
                    case 0:
                        this.$locvar0 = this.typeDefinitions.GetEnumerator();
                        num = 0xfffffffd;
                        break;

                    case 1:
                    case 2:
                        break;

                    default:
                        goto Label_0151;
                }
                try
                {
                    switch (num)
                    {
                        case 1:
                            goto Label_0083;
                    }
                    while (this.$locvar0.MoveNext())
                    {
                        this.<typeDefinition>__0 = this.$locvar0.Current;
                        this.$locvar1 = AssemblyConverter.GetAllTypes(this.<typeDefinition>__0.NestedTypes).GetEnumerator();
                        num = 0xfffffffd;
                    Label_0083:
                        try
                        {
                            while (this.$locvar1.MoveNext())
                            {
                                this.<nestedType>__1 = this.$locvar1.Current;
                                this.$current = this.<nestedType>__1;
                                if (!this.$disposing)
                                {
                                    this.$PC = 1;
                                }
                                flag = true;
                                goto Label_0153;
                            }
                        }
                        finally
                        {
                            if (!flag)
                            {
                            }
                            if (this.$locvar1 != null)
                            {
                                this.$locvar1.Dispose();
                            }
                        }
                        this.$current = this.<typeDefinition>__0;
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                        flag = true;
                        goto Label_0153;
                    }
                }
                finally
                {
                    if (!flag)
                    {
                    }
                    if (this.$locvar0 != null)
                    {
                        this.$locvar0.Dispose();
                    }
                }
                this.$PC = -1;
            Label_0151:
                return false;
            Label_0153:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<TypeDefinition> IEnumerable<TypeDefinition>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new AssemblyConverter.<GetAllTypes>c__Iterator0 { typeDefinitions = this.typeDefinitions };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<Mono.Cecil.TypeDefinition>.GetEnumerator();

            TypeDefinition IEnumerator<TypeDefinition>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }
    }
}

