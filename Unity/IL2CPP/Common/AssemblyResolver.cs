namespace Unity.IL2CPP.Common
{
    using Mono.Cecil;
    using NiceIO;
    using System;
    using System.Collections.Generic;

    public sealed class AssemblyResolver : Unity.IL2CPP.Common.IAssemblyResolver, Mono.Cecil.IAssemblyResolver
    {
        private readonly Dictionary<string, AssemblyDefinition> _assemblies;
        private readonly AssemblyLoader _assemblyLoader;
        private readonly HashSet<NPath> _searchDirectories;

        public AssemblyResolver(Dictionary<string, AssemblyDefinition> assemblyCache)
        {
            this._searchDirectories = new HashSet<NPath>();
            this._assemblies = assemblyCache;
        }

        public AssemblyResolver(AssemblyLoader assemblyLoader) : this(new Dictionary<string, AssemblyDefinition>(StringComparer.InvariantCultureIgnoreCase))
        {
            this._assemblyLoader = assemblyLoader;
        }

        public void AddSearchDirectory(NPath directory)
        {
            this._searchDirectories.Add(directory);
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

        public IEnumerable<NPath> GetSearchDirectories() => 
            this._searchDirectories;

        public bool IsAssemblyCached(AssemblyNameReference assemblyName) => 
            this._assemblies.ContainsKey(assemblyName.Name);

        public bool IsAssemblyCached(string name) => 
            this._assemblies.ContainsKey(name);

        public AssemblyDefinition Resolve(AssemblyNameReference name)
        {
            AssemblyDefinition definition;
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (!this._assemblies.TryGetValue(name.Name, out definition))
            {
                definition = this.ResolveInternal(name);
            }
            return definition;
        }

        public AssemblyDefinition Resolve(string fullName)
        {
            if (fullName == null)
            {
                throw new ArgumentNullException("fullName");
            }
            return this.Resolve(AssemblyNameReference.Parse(fullName));
        }

        public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters) => 
            this.Resolve(name);

        public AssemblyDefinition Resolve(string fullName, ReaderParameters parameters) => 
            this.Resolve(fullName);

        private AssemblyDefinition ResolveInternal(AssemblyNameReference name)
        {
            string[] strArray = !name.IsWindowsRuntime ? new string[] { ".exe", ".dll" } : new string[] { ".winmd", ".dll" };
            foreach (NPath path in this._searchDirectories)
            {
                foreach (string str in strArray)
                {
                    string[] append = new string[] { name.Name + str };
                    NPath path2 = path.Combine(append);
                    if (path2.FileExists(""))
                    {
                        return this._assemblyLoader.Load(path2.ToString());
                    }
                }
            }
            throw new AssemblyResolutionException(name);
        }
    }
}

