namespace Unity.IL2CPP.Common
{
    using Mono.Cecil;
    using Mono.Cecil.Mdb;
    using Mono.CompilerServices.SymbolWriter;
    using NiceIO;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;

    public class AssemblyLoader : IAssemblyLoader
    {
        private readonly ReaderParameters _readerParameters;
        private readonly ReaderParameters _readerWithoutSymbolsParameters;
        private readonly AssemblyResolver _resolver;

        public AssemblyLoader(IEnumerable<NPath> searchDirectories, bool readSymbols = false, bool applyWindowsRuntimeProjections = false)
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
                ReadSymbols = readSymbols,
                SymbolReaderProvider = !readSymbols ? null : new MdbReaderProvider(),
                ApplyWindowsRuntimeProjections = applyWindowsRuntimeProjections
            };
            this._readerParameters = parameters;
            parameters = new ReaderParameters {
                ApplyWindowsRuntimeProjections = this._readerParameters.ApplyWindowsRuntimeProjections,
                AssemblyResolver = this._readerParameters.AssemblyResolver,
                MetadataImporterProvider = this._readerParameters.MetadataImporterProvider,
                MetadataResolver = this._readerParameters.MetadataResolver,
                ReadingMode = this._readerParameters.ReadingMode,
                ReadSymbols = false,
                SymbolReaderProvider = null,
                ReflectionImporterProvider = this._readerParameters.ReflectionImporterProvider,
                SymbolStream = this._readerParameters.SymbolStream
            };
            this._readerWithoutSymbolsParameters = parameters;
        }

        public void AddSearchDirectory(NPath path)
        {
            this._resolver.AddSearchDirectory(path);
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
            if (File.Exists(name))
            {
                AssemblyDefinition definition;
                try
                {
                    definition = AssemblyDefinition.ReadAssembly(name, this._readerParameters);
                }
                catch (FileNotFoundException exception)
                {
                    if (!exception.FileName.EndsWith(".mdb") || (this._readerParameters.SymbolReaderProvider == null))
                    {
                        throw;
                    }
                    definition = AssemblyDefinition.ReadAssembly(name, this._readerWithoutSymbolsParameters);
                }
                catch (MonoSymbolFileException)
                {
                    definition = AssemblyDefinition.ReadAssembly(name, this._readerWithoutSymbolsParameters);
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
                definition = this._resolver.Resolve(name, this._readerParameters);
            }
            catch
            {
                throw new AssemblyResolutionException(name);
            }
            return definition;
        }
    }
}

