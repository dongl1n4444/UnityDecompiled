namespace Unity.IL2CPP.Common
{
    using Mono.Cecil;
    using Mono.Cecil.Mdb;
    using Mono.Cecil.Pdb;
    using Mono.CompilerServices.SymbolWriter;
    using NiceIO;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;

    public class AssemblyLoader : IAssemblyLoader, IDisposable
    {
        private readonly ReaderParameters _dllReaderParametersWithMdbSymbols;
        private readonly ReaderParameters _dllReaderParametersWithoutSymbols;
        private readonly ReaderParameters _dllReaderParametersWithPdbSymbols;
        private readonly AssemblyResolver _resolver;
        private AssemblyDefinition _windowsRuntimeMetadataAssembly;
        private readonly ReaderParameters _winmdReaderParametersWithoutSymbols;
        private readonly ReaderParameters _winmdReaderParametersWithPdbSymbols;

        public AssemblyLoader(IEnumerable<NPath> searchDirectories, bool applyWindowsRuntimeProjections = false)
        {
            this._resolver = new AssemblyResolver(this);
            foreach (NPath path in searchDirectories)
            {
                if (path != null)
                {
                    this._resolver.AddSearchDirectory(path);
                }
            }
            ReaderParameters parameters = new ReaderParameters {
                AssemblyResolver = this._resolver,
                MetadataResolver = new WindowsRuntimeAwareMetadataResolver(this._resolver),
                ReadSymbols = false,
                SymbolReaderProvider = null,
                ApplyWindowsRuntimeProjections = applyWindowsRuntimeProjections,
                ReadingMode = ReadingMode.Deferred
            };
            this._dllReaderParametersWithoutSymbols = parameters;
            this._dllReaderParametersWithPdbSymbols = CloneReaderParameters(this._dllReaderParametersWithoutSymbols);
            this._dllReaderParametersWithPdbSymbols.ReadSymbols = true;
            this._dllReaderParametersWithPdbSymbols.SymbolReaderProvider = new PdbReaderProvider();
            this._dllReaderParametersWithMdbSymbols = CloneReaderParameters(this._dllReaderParametersWithoutSymbols);
            this._dllReaderParametersWithMdbSymbols.ReadSymbols = true;
            this._dllReaderParametersWithMdbSymbols.SymbolReaderProvider = new MdbReaderProvider();
            this._winmdReaderParametersWithPdbSymbols = CloneReaderParameters(this._dllReaderParametersWithPdbSymbols);
            this._winmdReaderParametersWithPdbSymbols.ReadingMode = ReadingMode.Immediate;
            this._winmdReaderParametersWithoutSymbols = CloneReaderParameters(this._dllReaderParametersWithoutSymbols);
            this._winmdReaderParametersWithoutSymbols.ReadingMode = ReadingMode.Immediate;
        }

        public void AddSearchDirectory(NPath path)
        {
            this._resolver.AddSearchDirectory(path);
        }

        private AssemblyDefinition AddWindowsMetadataAssembly(AssemblyDefinition assembly)
        {
            if (this._windowsRuntimeMetadataAssembly == null)
            {
                AssemblyNameDefinition assemblyName = new AssemblyNameDefinition("WindowsRuntimeMetadata", new Version(0xff, 0xff, 0xff, 0xff)) {
                    Culture = "",
                    IsWindowsRuntime = true
                };
                ModuleParameters parameters = new ModuleParameters {
                    AssemblyResolver = this._dllReaderParametersWithMdbSymbols.AssemblyResolver,
                    MetadataImporterProvider = this._dllReaderParametersWithMdbSymbols.MetadataImporterProvider,
                    MetadataResolver = this._dllReaderParametersWithMdbSymbols.MetadataResolver,
                    ReflectionImporterProvider = this._dllReaderParametersWithMdbSymbols.ReflectionImporterProvider,
                    Runtime = TargetRuntime.Net_4_0
                };
                this._windowsRuntimeMetadataAssembly = AssemblyDefinition.CreateAssembly(assemblyName, "WindowsRuntimeMetadata", parameters);
            }
            List<TypeDefinition> list = new List<TypeDefinition>(assembly.MainModule.Types.Count);
            list.AddRange(assembly.MainModule.Types);
            assembly.MainModule.Types.Clear();
            foreach (TypeDefinition definition3 in list)
            {
                if (definition3.Name != "<Module>")
                {
                    this._windowsRuntimeMetadataAssembly.MainModule.Types.Add(definition3);
                }
            }
            foreach (AssemblyNameReference reference in assembly.MainModule.AssemblyReferences)
            {
                this._windowsRuntimeMetadataAssembly.MainModule.AssemblyReferences.Add(reference);
            }
            this._resolver.CacheAssembly(assembly.Name.Name, this._windowsRuntimeMetadataAssembly);
            return this._windowsRuntimeMetadataAssembly;
        }

        private ReaderParameters ChooseReaderParameters(NPath path, bool loadSymbols)
        {
            string[] extensions = new string[] { ".winmd" };
            bool flag = path.HasExtension(extensions);
            if (loadSymbols)
            {
                if (flag)
                {
                    return this._winmdReaderParametersWithPdbSymbols;
                }
                if (PlatformUtils.IsWindows() && path.ChangeExtension(".pdb").Exists(""))
                {
                    return this._dllReaderParametersWithPdbSymbols;
                }
                return this._dllReaderParametersWithMdbSymbols;
            }
            if (flag)
            {
                return this._winmdReaderParametersWithoutSymbols;
            }
            return this._dllReaderParametersWithoutSymbols;
        }

        private static ReaderParameters CloneReaderParameters(ReaderParameters original) => 
            new ReaderParameters { 
                ApplyWindowsRuntimeProjections = original.ApplyWindowsRuntimeProjections,
                AssemblyResolver = original.AssemblyResolver,
                MetadataImporterProvider = original.MetadataImporterProvider,
                MetadataResolver = original.MetadataResolver,
                ReadingMode = original.ReadingMode,
                ReadSymbols = original.ReadSymbols,
                ReflectionImporterProvider = original.ReflectionImporterProvider,
                SymbolReaderProvider = original.SymbolReaderProvider,
                SymbolStream = original.SymbolStream
            };

        public void Dispose()
        {
            this._resolver.Dispose();
        }

        private static AssemblyNameReference GetReference(IMetadataScope scope)
        {
            ModuleDefinition definition = scope as ModuleDefinition;
            if (definition != null)
            {
                return definition.Assembly.Name;
            }
            return (AssemblyNameReference) scope;
        }

        public bool IsAssemblyCached(string name) => 
            this._resolver.IsAssemblyCached(name);

        public AssemblyDefinition Load(string name)
        {
            NPath path = name.ToNPath();
            if (path.FileExists(""))
            {
                AssemblyDefinition definition;
                try
                {
                    definition = AssemblyDefinition.ReadAssembly(name, this.ChooseReaderParameters(path, true));
                }
                catch (Exception exception)
                {
                    if ((!(exception is FileNotFoundException) && !(exception is MonoSymbolFileException)) && !(exception is InvalidOperationException))
                    {
                        throw;
                    }
                    definition = AssemblyDefinition.ReadAssembly(name, this.ChooseReaderParameters(path, false));
                }
                if (definition.MainModule.MetadataKind == MetadataKind.WindowsMetadata)
                {
                    return this.AddWindowsMetadataAssembly(definition);
                }
                this._resolver.CacheAssembly(definition);
                return definition;
            }
            return this.Resolve(new AssemblyNameReference(name, null));
        }

        public AssemblyDefinition Resolve(IMetadataScope scope)
        {
            AssemblyDefinition definition;
            AssemblyNameReference name = GetReference(scope);
            try
            {
                definition = this._resolver.Resolve(name, this._dllReaderParametersWithMdbSymbols);
            }
            catch
            {
                throw new AssemblyResolutionException(name);
            }
            return definition;
        }
    }
}

