using Mono.Cecil;
using NiceIO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

namespace Unity.IL2CPP
{
	public class AssemblyConverter
	{
		[Inject]
		public static ITypeProviderInitializerService TypeProviderInitializer;

		[Inject]
		public static ITypeProviderService TypeProvider;

		[Inject]
		public static IWindowsRuntimeProjectionsInitializer WindowsRuntimeProjectionsInitializer;

		[Inject]
		public static IVirtualCallCollectorService VirtualCallCollector;

		[Inject]
		public static IAssemblyDependencies AssemblyDependencies;

		private readonly NPath _outputDir;

		private readonly NPath _dataFolder;

		private readonly NPath[] _assemblies;

		private readonly AssemblyLoader _assemblyLoader;

		private readonly IDebuggerSupport _debuggerSupport;

		private readonly VTableBuilder _vTableBuilder = new VTableBuilder();

		private readonly VirtualInvokerCollector _virtualInvokerCollector = new VirtualInvokerCollector(false);

		private readonly VirtualInvokerCollector _genericVirtualInvokerCollector = new VirtualInvokerCollector(true);

		private readonly InterfaceInvokerCollector _interfaceInvokerCollector = new InterfaceInvokerCollector(false);

		private readonly InterfaceInvokerCollector _genericInterfaceInvokerCollector = new InterfaceInvokerCollector(true);

		private readonly StringLiteralCollector _stringLiteralCollector = new StringLiteralCollector();

		private List<AssemblyDefinition> _assembliesOrderedByDependency = new List<AssemblyDefinition>();

		protected AssemblyConverter(NPath[] assemblies, NPath outputDir, NPath dataFolder, DotNetProfile dotNetProfile)
		{
			this._assemblies = assemblies;
			this._outputDir = outputDir;
			this._dataFolder = dataFolder;
			this._debuggerSupport = DebuggerSupportFactory.GetDebuggerSupport();
			NPath[] searchDirectories = new NPath[]
			{
				assemblies[0].Parent,
				MonoInstall.SmartProfilePath(dotNetProfile)
			};
			bool applyWindowsRuntimeProjections = CodeGenOptions.Dotnetprofile == DotNetProfile.Net45;
			this._assemblyLoader = new AssemblyLoader(searchDirectories, DebuggerOptions.Enabled || CodeGenOptions.EnableSymbolLoading, applyWindowsRuntimeProjections);
		}

		public static IEnumerable<NPath> ConvertAssemblies(IEnumerable<string> assemblyDirectories, IEnumerable<NPath> explicitAssemblies, NPath outputDir, NPath dataFolder)
		{
			List<NPath> list = new List<NPath>();
			if (assemblyDirectories != null)
			{
				foreach (string current in assemblyDirectories)
				{
					list.AddRange(AssemblyConverter.GetAssembliesInDirectory(current.ToNPath()));
				}
			}
			if (explicitAssemblies != null)
			{
				list.AddRange(explicitAssemblies);
			}
			AssemblyConverter.ConvertAssemblies(list.ToArray(), outputDir, dataFolder);
			return list;
		}

		public static void ConvertAssemblies(NPath[] assemblies, NPath outputDir, NPath dataFolder)
		{
			try
			{
				new AssemblyConverter(assemblies, outputDir, dataFolder, CodeGenOptions.Dotnetprofile).Apply();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ErrorMessageWriter.FormatMessage(ErrorInformation.CurrentlyProcessing, ex.Message));
				throw;
			}
		}

		private static IEnumerable<NPath> GetAssembliesInDirectory(NPath assemblyDirectory)
		{
			return from f in assemblyDirectory.Files(false)
			where f.HasExtension(new string[]
			{
				"dll",
				"exe"
			})
			select f;
		}

		private void Apply()
		{
			InflatedCollectionCollector inflatedCollectionCollector;
			TypeDefinition[] allTypeDefinitions;
			using (TinyProfiler.Section("PreProcessStage", ""))
			{
				this.PreProcessStage(out inflatedCollectionCollector, out allTypeDefinitions);
			}
			MethodCollector methodCollector = new MethodCollector();
			AttributeCollection attributeCollection = new AttributeCollection();
			MetadataCollector metadataCollector = new MetadataCollector();
			using (TinyProfiler.Section("MetadataCollector", ""))
			{
				metadataCollector.AddAssemblies(this._assembliesOrderedByDependency);
			}
			using (TinyProfiler.Section("AllAssemblyConversion", ""))
			{
				foreach (AssemblyDefinition current in this._assembliesOrderedByDependency)
				{
					using (TinyProfiler.Section("Convert", current.Name.Name))
					{
						this.Convert(current, inflatedCollectionCollector, attributeCollection, methodCollector, metadataCollector);
					}
				}
			}
			methodCollector.Complete();
			using (TinyProfiler.Section("WriteGenerics", ""))
			{
				this.WriteGenerics(inflatedCollectionCollector.AsReadOnly(), allTypeDefinitions, metadataCollector);
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
				SourceWriter.WriteCollectedMetadata(inflatedCollectionCollector, this._assembliesOrderedByDependency, this._outputDir, this._dataFolder, metadataCollector, attributeCollection, this._vTableBuilder, methodCollector);
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

		private void CollectAssembliesToConvert()
		{
			this._assembliesOrderedByDependency = this.CollectAssembliesRecursive(from path in this._assemblies
			select this._assemblyLoader.Load(path.ToString())).ToList<AssemblyDefinition>();
			this._assembliesOrderedByDependency.Sort(new AssemblyDependencyComparer(AssemblyDependencyComparer.MaximumDepthForEachAssembly(this._assembliesOrderedByDependency)));
		}

		private HashSet<AssemblyDefinition> CollectAssembliesRecursive(IEnumerable<AssemblyDefinition> assemblies)
		{
			HashSet<AssemblyDefinition> hashSet = new HashSet<AssemblyDefinition>(assemblies, new AssemblyDefinitionComparer());
			int num = 0;
			while (hashSet.Count > num)
			{
				num = hashSet.Count;
				hashSet.UnionWith(hashSet.ToArray<AssemblyDefinition>().SelectMany(new Func<AssemblyDefinition, IEnumerable<AssemblyDefinition>>(AssemblyConverter.AssemblyDependencies.GetReferencedAssembliesFor)));
			}
			if (!hashSet.Any((AssemblyDefinition a) => a.Name.Name == "mscorlib"))
			{
				hashSet.UnionWith(new AssemblyDefinition[]
				{
					this._assemblyLoader.Load("mscorlib")
				});
			}
			return hashSet;
		}

		private void PreProcessStage(out InflatedCollectionCollector genericsCollectionCollector, out TypeDefinition[] allTypeDefinitions)
		{
			using (TinyProfiler.Section("Collect assemblies to convert", ""))
			{
				this.CollectAssembliesToConvert();
			}
			AssemblyDefinition assemblyDefinition = this._assembliesOrderedByDependency.Single((AssemblyDefinition ad) => ad.Name.Name == "mscorlib");
			AssemblyConverter.TypeProviderInitializer.Initialize(assemblyDefinition);
			AssemblyConverter.WindowsRuntimeProjectionsInitializer.Initialize(assemblyDefinition.MainModule, CodeGenOptions.Dotnetprofile);
			this._outputDir.EnsureDirectoryExists("");
			AssemblyDefinition[] array = (from a in this._assembliesOrderedByDependency
			where a.MainModule.Kind == ModuleKind.Windows || a.MainModule.Kind == ModuleKind.Console
			select a).ToArray<AssemblyDefinition>();
			if (array.Length > 0)
			{
				AssemblyDefinition assemblyDefinition2 = array.FirstOrDefault((AssemblyDefinition a) => a.EntryPoint != null);
				new DriverWriter(assemblyDefinition2 ?? array.First<AssemblyDefinition>()).Write(this._outputDir);
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
			foreach (AssemblyDefinition current in this._assembliesOrderedByDependency)
			{
				this._stringLiteralCollector.Process(current);
			}
			foreach (AssemblyDefinition current2 in this._assembliesOrderedByDependency)
			{
				this._debuggerSupport.Analyze(current2);
			}
			foreach (AssemblyDefinition current3 in this._assembliesOrderedByDependency)
			{
				current3.Accept(new GenericSharingVisitor());
			}
			using (TinyProfiler.Section("WriteResources", ""))
			{
				this.WriteEmbeddedResourcesForEachAssembly();
			}
			foreach (AssemblyDefinition current4 in this._assembliesOrderedByDependency)
			{
				using (TinyProfiler.Section("VirtualInvokeCollector", current4.Name.Name))
				{
					this._virtualInvokerCollector.Process(current4);
				}
			}
			foreach (AssemblyDefinition current5 in this._assembliesOrderedByDependency)
			{
				using (TinyProfiler.Section("GenericVirtualInvokerCollector", current5.Name.Name))
				{
					this._genericVirtualInvokerCollector.Process(current5);
				}
			}
			foreach (AssemblyDefinition current6 in this._assembliesOrderedByDependency)
			{
				using (TinyProfiler.Section("InterfaceInvokerCollector", current6.Name.Name))
				{
					this._interfaceInvokerCollector.Process(current6);
				}
			}
			foreach (AssemblyDefinition current7 in this._assembliesOrderedByDependency)
			{
				using (TinyProfiler.Section("GenericInterfaceInvokerCollector", current7.Name.Name))
				{
					this._genericInterfaceInvokerCollector.Process(current7);
				}
			}
			using (TinyProfiler.Section("GenericsCollector.Collect", ""))
			{
				genericsCollectionCollector = GenericsCollector.Collect(this._assembliesOrderedByDependency);
			}
			this.AddExtraTypes(genericsCollectionCollector);
			allTypeDefinitions = AssemblyConverter.GetAllTypes(this._assembliesOrderedByDependency.SelectMany((AssemblyDefinition a) => a.MainModule.Types)).ToArray<TypeDefinition>();
			using (TinyProfiler.Section("CollectGenericVirtualMethods.Collect", ""))
			{
				this.CollectGenericVirtualMethods(genericsCollectionCollector, allTypeDefinitions);
			}
		}

		private void WriteEmbeddedResourcesForEachAssembly()
		{
			NPath nPath = this._dataFolder.Combine(new string[]
			{
				"Resources"
			});
			nPath.CreateDirectory();
			foreach (AssemblyDefinition current in this._assembliesOrderedByDependency)
			{
				if (current.MainModule.Resources.Any<Resource>())
				{
					string path = nPath.Combine(new string[]
					{
						string.Format("{0}-resources.dat", current.MainModule.Name)
					}).ToString();
					using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
					{
						ResourceWriter.WriteEmbeddedResources(current, fileStream);
					}
				}
			}
		}

		private void PatchEnumsNestedInGenericTypes()
		{
			PatchEnumsNestedInGenericTypesVisitor visitor = new PatchEnumsNestedInGenericTypesVisitor();
			foreach (AssemblyDefinition current in this._assembliesOrderedByDependency)
			{
				using (TinyProfiler.Section("PatchEnumsNestedInGenericTypes in assembly", current.Name.Name))
				{
					current.Accept(visitor);
				}
			}
		}

		private void InjectBaseTypesAndFinalizersIntoComAndWindowsRuntimeTypes()
		{
			InjectBaseTypesAndFinalizersIntoComAndWindowsRuntimeTypesVisitor visitor = new InjectBaseTypesAndFinalizersIntoComAndWindowsRuntimeTypesVisitor();
			foreach (AssemblyDefinition current in this._assembliesOrderedByDependency)
			{
				using (TinyProfiler.Section("ModifyCOMAndWindowsRuntimeTypes in assembly", current.Name.Name))
				{
					current.Accept(visitor);
				}
			}
		}

		private void ApplyDefaultMarshalAsAttribute()
		{
			ApplyDefaultMarshalAsAttributeVisitor visitor = new ApplyDefaultMarshalAsAttributeVisitor();
			foreach (AssemblyDefinition current in this._assembliesOrderedByDependency)
			{
				using (TinyProfiler.Section("ApplyDefaultMarshalAsAttributeVisitor in assembly", current.Name.Name))
				{
					current.Accept(visitor);
				}
			}
		}

		private void AddExtraTypes(InflatedCollectionCollector genericsCollectionCollector)
		{
			ExtraTypesSupport extraTypesSupport = new ExtraTypesSupport(genericsCollectionCollector, this._assembliesOrderedByDependency);
			foreach (string current in AssemblyConverter.BuildExtraTypesList())
			{
				TypeNameParseInfo typeNameParseInfo = TypeNameParser.Parse(current);
				if (typeNameParseInfo == null)
				{
					Console.WriteLine("WARNING: Cannot parse type name {0} from the extra types list. Skipping.", current);
				}
				else if (!extraTypesSupport.AddType(current, typeNameParseInfo))
				{
					Console.WriteLine("WARNING: Cannot add extra type {0}. Skipping.", current);
				}
			}
		}

		private static IEnumerable<string> BuildExtraTypesList()
		{
			HashSet<string> hashSet = new HashSet<string>();
			if (ExtraTypesOptions.Name != null)
			{
				string[] name = ExtraTypesOptions.Name;
				for (int i = 0; i < name.Length; i++)
				{
					string item = name[i];
					hashSet.Add(item);
				}
			}
			if (ExtraTypesOptions.File != null)
			{
				string[] file = ExtraTypesOptions.File;
				for (int j = 0; j < file.Length; j++)
				{
					string text = file[j];
					try
					{
						foreach (string current in from l in File.ReadAllLines(text)
						select l.Trim() into l
						where l.Length > 0
						select l)
						{
							if (!current.StartsWith(";") && !current.StartsWith("#") && !current.StartsWith("//"))
							{
								hashSet.Add(current);
							}
						}
					}
					catch (Exception)
					{
						Console.WriteLine("WARNING: Cannot open extra file list {0}. Skipping.", text);
					}
				}
			}
			return hashSet;
		}

		private void CollectGenericVirtualMethods(InflatedCollectionCollector allGenerics, IEnumerable<TypeDefinition> allTypeDefinitions)
		{
			GenericVirtualMethodCollector genericVirtualMethodCollector = new GenericVirtualMethodCollector();
			genericVirtualMethodCollector.Collect(allGenerics, allTypeDefinitions, this._vTableBuilder);
		}

		private void WriteGenerics(ReadOnlyInflatedCollectionCollector allGenerics, IEnumerable<TypeDefinition> allTypeDefinitions, IMetadataCollection metadataCollection)
		{
			SourceWriter sourceWriter = new SourceWriter(this._vTableBuilder, this._debuggerSupport, this._outputDir);
			sourceWriter.WriteGenerics(allGenerics, allTypeDefinitions, metadataCollection);
		}

		private NPath GetPathFor(string filename, string extension)
		{
			return this._outputDir.Combine(new string[]
			{
				filename
			}).ChangeExtension(extension);
		}

		private void Convert(AssemblyDefinition assemblyDefinition, InflatedCollectionCollector allGenerics, AttributeCollection attributeCollection, MethodCollector methodCollector, IMetadataCollection metadataCollection)
		{
			TypeDefinition[] typeList = AssemblyConverter.GetAllTypes(assemblyDefinition.MainModule.Types).ToArray<TypeDefinition>();
			SourceWriter sourceWriter = new SourceWriter(this._vTableBuilder, this._debuggerSupport, this._outputDir);
			sourceWriter.Write(assemblyDefinition, allGenerics, this._outputDir, typeList, attributeCollection, methodCollector, metadataCollection);
		}

		[DebuggerHidden]
		public static IEnumerable<TypeDefinition> GetAllTypes(IEnumerable<TypeDefinition> typeDefinitions)
		{
			AssemblyConverter.<GetAllTypes>c__Iterator0 <GetAllTypes>c__Iterator = new AssemblyConverter.<GetAllTypes>c__Iterator0();
			<GetAllTypes>c__Iterator.typeDefinitions = typeDefinitions;
			AssemblyConverter.<GetAllTypes>c__Iterator0 expr_0E = <GetAllTypes>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
