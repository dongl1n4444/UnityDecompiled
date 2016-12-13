using Mono.Cecil;
using NiceIO;
using System;
using System.Collections.Generic;

namespace Unity.IL2CPP.Common
{
	public sealed class AssemblyResolver : IAssemblyResolver, Mono.Cecil.IAssemblyResolver
	{
		private readonly Dictionary<string, AssemblyDefinition> _assemblies;

		private readonly HashSet<NPath> _searchDirectories = new HashSet<NPath>();

		private readonly AssemblyLoader _assemblyLoader;

		public AssemblyResolver(AssemblyLoader assemblyLoader) : this(new Dictionary<string, AssemblyDefinition>(StringComparer.InvariantCultureIgnoreCase))
		{
			this._assemblyLoader = assemblyLoader;
		}

		public AssemblyResolver(Dictionary<string, AssemblyDefinition> assemblyCache)
		{
			this._assemblies = assemblyCache;
		}

		public AssemblyDefinition Resolve(AssemblyNameReference name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			AssemblyDefinition result;
			if (!this._assemblies.TryGetValue(name.Name, out result))
			{
				result = this.ResolveInternal(name);
			}
			return result;
		}

		public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
		{
			return this.Resolve(name);
		}

		public AssemblyDefinition Resolve(string fullName)
		{
			if (fullName == null)
			{
				throw new ArgumentNullException("fullName");
			}
			return this.Resolve(AssemblyNameReference.Parse(fullName));
		}

		public AssemblyDefinition Resolve(string fullName, ReaderParameters parameters)
		{
			return this.Resolve(fullName);
		}

		public void CacheAssembly(AssemblyDefinition assembly)
		{
			if (this._assemblies.ContainsKey(assembly.Name.Name))
			{
				throw new Exception(string.Format("Duplicate assembly found. These modules contain assemblies with same names:{0}\t{1}{0}\t{2}", Environment.NewLine, this._assemblies[assembly.Name.Name].MainModule.FullyQualifiedName, assembly.MainModule.FullyQualifiedName));
			}
			this._assemblies.Add(assembly.Name.Name, assembly);
			this._searchDirectories.Add(assembly.MainModule.FullyQualifiedName.ToNPath().Parent);
		}

		public bool IsAssemblyCached(AssemblyNameReference assemblyName)
		{
			return this._assemblies.ContainsKey(assemblyName.Name);
		}

		public bool IsAssemblyCached(string name)
		{
			return this._assemblies.ContainsKey(name);
		}

		public void AddSearchDirectory(NPath directory)
		{
			this._searchDirectories.Add(directory);
		}

		public IEnumerable<NPath> GetSearchDirectories()
		{
			return this._searchDirectories;
		}

		private AssemblyDefinition ResolveInternal(AssemblyNameReference name)
		{
			string[] arg_3D_0;
			if (name.IsWindowsRuntime)
			{
				string[] expr_12 = new string[2];
				expr_12[0] = ".winmd";
				arg_3D_0 = expr_12;
				expr_12[1] = ".dll";
			}
			else
			{
				string[] expr_2D = new string[2];
				expr_2D[0] = ".exe";
				arg_3D_0 = expr_2D;
				expr_2D[1] = ".dll";
			}
			string[] array = arg_3D_0;
			foreach (NPath current in this._searchDirectories)
			{
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string str = array2[i];
					NPath nPath = current.Combine(new string[]
					{
						name.Name + str
					});
					if (nPath.FileExists(""))
					{
						return this._assemblyLoader.Load(nPath.ToString());
					}
				}
			}
			throw new AssemblyResolutionException(name);
		}
	}
}
