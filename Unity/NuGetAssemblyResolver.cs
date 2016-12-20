namespace Unity
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor.Scripting.Compilers;

    internal class NuGetAssemblyResolver : SearchPathAssemblyResolver
    {
        private readonly Dictionary<string, AssemblyDefinition> _assemblies = new Dictionary<string, AssemblyDefinition>(StringComparer.InvariantCulture);
        private readonly Dictionary<string, string> _references;

        public NuGetAssemblyResolver(string projectLockFile)
        {
            NuGetPackageResolver resolver = new NuGetPackageResolver {
                ProjectLockFile = projectLockFile
            };
            resolver.Resolve();
            string[] resolvedReferences = resolver.ResolvedReferences;
            this._references = new Dictionary<string, string>(resolvedReferences.Length, StringComparer.InvariantCultureIgnoreCase);
            foreach (string str in resolvedReferences)
            {
                string str3;
                string fileName = Path.GetFileName(str);
                if (this._references.TryGetValue(fileName, out str3))
                {
                    throw new Exception(string.Format("Reference \"{0}\" already added as \"{1}\".", str, str3));
                }
                this._references.Add(fileName, str);
            }
        }

        public bool IsFrameworkAssembly(AssemblyNameReference name)
        {
            string str;
            string key = name.Name + (!name.IsWindowsRuntime ? ".dll" : ".winmd");
            return this._references.TryGetValue(key, out str);
        }

        public override AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
        {
            AssemblyDefinition definition;
            string str2;
            if (this._assemblies.TryGetValue(name.Name, out definition))
            {
                return definition;
            }
            string key = name.Name + (!name.IsWindowsRuntime ? ".dll" : ".winmd");
            if (this._references.TryGetValue(key, out str2))
            {
                definition = AssemblyDefinition.ReadAssembly(str2, parameters);
                if (string.Equals(definition.Name.Name, name.Name, StringComparison.InvariantCulture))
                {
                    this._assemblies.Add(name.Name, definition);
                    return definition;
                }
            }
            return base.Resolve(name, parameters);
        }
    }
}

