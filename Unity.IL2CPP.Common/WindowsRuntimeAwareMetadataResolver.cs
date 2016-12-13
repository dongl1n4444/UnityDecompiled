using Mono.Cecil;
using Mono.Collections.Generic;
using NiceIO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.IL2CPP.Common
{
	internal class WindowsRuntimeAwareMetadataResolver : MetadataResolver
	{
		private readonly IAssemblyResolver _assemblyResolver;

		private readonly HashSet<NPath> _loadedWinmdPaths = new HashSet<NPath>();

		private readonly HashSet<AssemblyDefinition> _loadedWinmds = new HashSet<AssemblyDefinition>();

		public WindowsRuntimeAwareMetadataResolver(IAssemblyResolver assemblyResolver) : base(assemblyResolver)
		{
			this._assemblyResolver = assemblyResolver;
		}

		public override TypeDefinition Resolve(TypeReference type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			type = type.GetElementType();
			IMetadataScope scope = type.Scope;
			TypeDefinition result;
			if (scope == null)
			{
				result = null;
			}
			else
			{
				AssemblyNameReference assemblyNameReference = scope as AssemblyNameReference;
				if (assemblyNameReference != null)
				{
					if (assemblyNameReference.IsWindowsRuntime && !this._assemblyResolver.IsAssemblyCached(assemblyNameReference))
					{
						result = this.FindTypeInUnknownWinmd(assemblyNameReference, type);
					}
					else
					{
						AssemblyDefinition assemblyDefinition = this._assemblyResolver.Resolve(assemblyNameReference);
						if (assemblyDefinition == null)
						{
							result = null;
						}
						else
						{
							TypeDefinition type2 = WindowsRuntimeAwareMetadataResolver.GetType(assemblyDefinition.MainModule, type);
							if (type2 != null)
							{
								result = type2;
							}
							else if (type.Module.MetadataKind != MetadataKind.Ecma335 && assemblyNameReference.Name == "mscorlib")
							{
								result = this.FindTypeInUnknownWinmd(assemblyNameReference, type);
							}
							else
							{
								if (!assemblyNameReference.IsWindowsRuntime)
								{
									throw new InvalidOperationException(string.Format("Unable to resolve [{0}]{1}.", assemblyNameReference.Name, type.FullName));
								}
								result = this.FindTypeInUnknownWinmd(assemblyNameReference, type);
							}
						}
					}
				}
				else
				{
					ModuleDefinition moduleDefinition = scope as ModuleDefinition;
					if (moduleDefinition == null)
					{
						ModuleReference moduleReference = scope as ModuleReference;
						if (moduleReference != null)
						{
							Collection<ModuleDefinition> modules = type.Module.Assembly.Modules;
							for (int i = 0; i < modules.Count; i++)
							{
								ModuleDefinition moduleDefinition2 = modules[i];
								if (moduleDefinition2.Name == moduleReference.Name)
								{
									result = WindowsRuntimeAwareMetadataResolver.GetType(moduleDefinition2, type);
									return result;
								}
							}
						}
						throw new InvalidOperationException("type.Scope isn't a valid metadata scope!");
					}
					result = WindowsRuntimeAwareMetadataResolver.GetType(moduleDefinition, type);
				}
			}
			return result;
		}

		private TypeDefinition FindTypeInUnknownWinmd(AssemblyNameReference assemblyNameReference, TypeReference type)
		{
			IEnumerable<NPath> enumerable = from winmd in (from d in this._assemblyResolver.GetSearchDirectories()
			where d.Exists("")
			select d).SelectMany((NPath d) => d.Files("*.winmd", false))
			where !this._loadedWinmdPaths.Contains(winmd)
			select winmd;
			foreach (NPath current in enumerable)
			{
				this._loadedWinmds.Add(this._assemblyResolver.Resolve(new AssemblyNameReference(current.FileNameWithoutExtension, new Version())
				{
					IsWindowsRuntime = true
				}));
				this._loadedWinmdPaths.Add(current);
			}
			TypeDefinition result;
			foreach (AssemblyDefinition current2 in this._loadedWinmds)
			{
				TypeDefinition type2 = WindowsRuntimeAwareMetadataResolver.GetType(current2.MainModule, type);
				if (type2 != null)
				{
					result = type2;
					return result;
				}
			}
			result = null;
			return result;
		}

		private static TypeDefinition GetType(ModuleDefinition module, TypeReference reference)
		{
			TypeDefinition typeDefinition = WindowsRuntimeAwareMetadataResolver.GetTypeDefinition(module, reference);
			TypeDefinition result;
			if (typeDefinition != null)
			{
				result = typeDefinition;
			}
			else if (!module.HasExportedTypes)
			{
				result = null;
			}
			else
			{
				Collection<ExportedType> exportedTypes = module.ExportedTypes;
				for (int i = 0; i < exportedTypes.Count; i++)
				{
					ExportedType exportedType = exportedTypes[i];
					if (!(exportedType.Name != reference.Name))
					{
						if (!(exportedType.Namespace != reference.Namespace))
						{
							result = exportedType.Resolve();
							return result;
						}
					}
				}
				result = null;
			}
			return result;
		}

		private static TypeDefinition GetTypeDefinition(ModuleDefinition module, TypeReference type)
		{
			TypeDefinition result;
			if (!type.IsNested)
			{
				result = module.GetType(type.Namespace, type.Name);
			}
			else
			{
				TypeDefinition typeDefinition = type.DeclaringType.Resolve();
				if (typeDefinition == null)
				{
					result = null;
				}
				else
				{
					result = WindowsRuntimeAwareMetadataResolver.GetNestedType(typeDefinition, WindowsRuntimeAwareMetadataResolver.TypeFullName(type));
				}
			}
			return result;
		}

		private static TypeDefinition GetNestedType(TypeDefinition self, string fullname)
		{
			TypeDefinition result;
			if (!self.HasNestedTypes)
			{
				result = null;
			}
			else
			{
				Collection<TypeDefinition> nestedTypes = self.NestedTypes;
				for (int i = 0; i < nestedTypes.Count; i++)
				{
					TypeDefinition typeDefinition = nestedTypes[i];
					if (WindowsRuntimeAwareMetadataResolver.TypeFullName(typeDefinition) == fullname)
					{
						result = typeDefinition;
						return result;
					}
				}
				result = null;
			}
			return result;
		}

		private static string TypeFullName(TypeReference type)
		{
			return (!string.IsNullOrEmpty(type.Namespace)) ? (type.Namespace + '.' + type.Name) : type.Name;
		}
	}
}
