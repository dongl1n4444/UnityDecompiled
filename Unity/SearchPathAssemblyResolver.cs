namespace Unity
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;

    internal class SearchPathAssemblyResolver : IAssemblyResolver
    {
        private readonly Dictionary<string, AssemblyDefinition> _assemblies = new Dictionary<string, AssemblyDefinition>(StringComparer.InvariantCulture);
        private readonly List<string> _searchPaths = new List<string>();

        public void AddAssembly(AssemblyDefinition assembly)
        {
            string name = assembly.Name.Name;
            if (this._assemblies.ContainsKey(name))
            {
                throw new Exception($"Assembly "{name}" is already registered.");
            }
            this._assemblies.Add(name, assembly);
        }

        public void AddSearchDirectory(string path)
        {
            <AddSearchDirectory>c__AnonStorey0 storey = new <AddSearchDirectory>c__AnonStorey0 {
                path = path
            };
            if (!Enumerable.Any<string>(this._searchPaths, new Func<string, bool>(storey, (IntPtr) this.<>m__0)))
            {
                this._searchPaths.Add(storey.path);
            }
        }

        public AssemblyDefinition Resolve(AssemblyNameReference name)
        {
            ReaderParameters parameters = new ReaderParameters {
                AssemblyResolver = this
            };
            return this.Resolve(name, parameters);
        }

        public AssemblyDefinition Resolve(string fullName)
        {
            ReaderParameters parameters = new ReaderParameters {
                AssemblyResolver = this
            };
            return this.Resolve(fullName, parameters);
        }

        public virtual AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
        {
            AssemblyDefinition definition;
            if (this._assemblies.TryGetValue(name.Name, out definition))
            {
                return definition;
            }
            foreach (string str in this._searchPaths)
            {
                string str2 = name.Name + (!name.IsWindowsRuntime ? ".dll" : ".winmd");
                string path = Path.Combine(str, str2);
                if (File.Exists(path))
                {
                    definition = AssemblyDefinition.ReadAssembly(path, parameters);
                    if (string.Equals(definition.Name.Name, name.Name, StringComparison.InvariantCulture))
                    {
                        this._assemblies.Add(name.Name, definition);
                        return definition;
                    }
                }
            }
            throw new AssemblyResolutionException(name);
        }

        public AssemblyDefinition Resolve(string fullName, ReaderParameters parameters) => 
            this.Resolve(AssemblyNameReference.Parse(fullName), parameters);

        [CompilerGenerated]
        private sealed class <AddSearchDirectory>c__AnonStorey0
        {
            internal string path;

            internal bool <>m__0(string p) => 
                string.Equals(p, this.path, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}

