using Mono.Cecil;
using Mono.Cecil.Mdb;
using Mono.CompilerServices.SymbolWriter;
using NiceIO;
using System;
using System.Collections.Generic;
using System.IO;

namespace Unity.IL2CPP.Common
{
	public class AssemblyLoader : IAssemblyLoader
	{
		private readonly AssemblyResolver _resolver;

		private readonly ReaderParameters _readerParameters;

		private readonly ReaderParameters _readerWithoutSymbolsParameters;

		public AssemblyLoader(IEnumerable<NPath> searchDirectories, bool readSymbols = false, bool applyWindowsRuntimeProjections = false)
		{
			this._resolver = new AssemblyResolver(this);
			foreach (NPath current in searchDirectories)
			{
				if (current != null)
				{
					this._resolver.AddSearchDirectory(current);
				}
			}
			this._readerParameters = new ReaderParameters
			{
				AssemblyResolver = this._resolver,
				MetadataResolver = new WindowsRuntimeAwareMetadataResolver(this._resolver),
				ReadSymbols = readSymbols,
				SymbolReaderProvider = ((!readSymbols) ? null : new MdbReaderProvider()),
				ApplyWindowsRuntimeProjections = applyWindowsRuntimeProjections
			};
			this._readerWithoutSymbolsParameters = new ReaderParameters
			{
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
		}

		public AssemblyDefinition Load(string name)
		{
			AssemblyDefinition result;
			if (File.Exists(name))
			{
				AssemblyDefinition assemblyDefinition;
				try
				{
					assemblyDefinition = AssemblyDefinition.ReadAssembly(name, this._readerParameters);
				}
				catch (FileNotFoundException ex)
				{
					if (!ex.FileName.EndsWith(".mdb") || this._readerParameters.SymbolReaderProvider == null)
					{
						throw;
					}
					assemblyDefinition = AssemblyDefinition.ReadAssembly(name, this._readerWithoutSymbolsParameters);
				}
				catch (MonoSymbolFileException)
				{
					assemblyDefinition = AssemblyDefinition.ReadAssembly(name, this._readerWithoutSymbolsParameters);
				}
				this._resolver.CacheAssembly(assemblyDefinition);
				result = assemblyDefinition;
			}
			else
			{
				result = this.Resolve(new AssemblyNameReference(name, null));
			}
			return result;
		}

		public AssemblyDefinition Resolve(IMetadataScope scope)
		{
			AssemblyNameReference reference = AssemblyLoader.GetReference(scope);
			AssemblyDefinition result;
			try
			{
				result = this._resolver.Resolve(reference, this._readerParameters);
			}
			catch
			{
				throw new AssemblyResolutionException(reference);
			}
			return result;
		}

		public bool IsAssemblyCached(string name)
		{
			return this._resolver.IsAssemblyCached(name);
		}

		private static AssemblyNameReference GetReference(IMetadataScope scope)
		{
			ModuleDefinition moduleDefinition = scope as ModuleDefinition;
			AssemblyNameReference result;
			if (moduleDefinition != null)
			{
				result = moduleDefinition.Assembly.Name;
			}
			else
			{
				result = (AssemblyNameReference)scope;
			}
			return result;
		}

		public void AddSearchDirectory(NPath path)
		{
			this._resolver.AddSearchDirectory(path);
		}
	}
}
