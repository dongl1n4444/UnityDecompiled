namespace Unity.IL2CPP.Common
{
    using Mono.Cecil;
    using Mono.Collections.Generic;
    using NiceIO;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    internal class WindowsRuntimeAwareMetadataResolver : MetadataResolver
    {
        private readonly Unity.IL2CPP.Common.IAssemblyResolver _assemblyResolver;
        private readonly HashSet<NPath> _loadedWinmdPaths;
        private readonly HashSet<AssemblyDefinition> _loadedWinmds;
        [CompilerGenerated]
        private static Func<NPath, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<NPath, IEnumerable<NPath>> <>f__am$cache1;

        public WindowsRuntimeAwareMetadataResolver(Unity.IL2CPP.Common.IAssemblyResolver assemblyResolver) : base(assemblyResolver)
        {
            this._loadedWinmdPaths = new HashSet<NPath>();
            this._loadedWinmds = new HashSet<AssemblyDefinition>();
            this._assemblyResolver = assemblyResolver;
        }

        private TypeDefinition FindTypeInUnknownWinmd(AssemblyNameReference assemblyNameReference, TypeReference type)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = d => d.Exists("");
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = d => d.Files("*.winmd", false);
            }
            IEnumerable<NPath> enumerable = from winmd in this._assemblyResolver.GetSearchDirectories().Where<NPath>(<>f__am$cache0).SelectMany<NPath, NPath>(<>f__am$cache1)
                where !this._loadedWinmdPaths.Contains(winmd)
                select winmd;
            foreach (NPath path in enumerable)
            {
                AssemblyNameReference name = new AssemblyNameReference(path.FileNameWithoutExtension, new Version()) {
                    IsWindowsRuntime = true
                };
                this._loadedWinmds.Add(this._assemblyResolver.Resolve(name));
                this._loadedWinmdPaths.Add(path);
            }
            foreach (AssemblyDefinition definition in this._loadedWinmds)
            {
                TypeDefinition definition2 = GetType(definition.MainModule, type);
                if (definition2 != null)
                {
                    return definition2;
                }
            }
            return null;
        }

        private static TypeDefinition GetNestedType(TypeDefinition self, string fullname)
        {
            if (self.HasNestedTypes)
            {
                Collection<TypeDefinition> nestedTypes = self.NestedTypes;
                for (int i = 0; i < nestedTypes.Count; i++)
                {
                    TypeDefinition type = nestedTypes[i];
                    if (TypeFullName(type) == fullname)
                    {
                        return type;
                    }
                }
            }
            return null;
        }

        private static TypeDefinition GetType(ModuleDefinition module, TypeReference reference)
        {
            TypeDefinition typeDefinition = GetTypeDefinition(module, reference);
            if (typeDefinition != null)
            {
                return typeDefinition;
            }
            if (module.HasExportedTypes)
            {
                Collection<ExportedType> exportedTypes = module.ExportedTypes;
                for (int i = 0; i < exportedTypes.Count; i++)
                {
                    ExportedType type = exportedTypes[i];
                    if ((type.Name == reference.Name) && (type.Namespace == reference.Namespace))
                    {
                        return type.Resolve();
                    }
                }
            }
            return null;
        }

        private static TypeDefinition GetTypeDefinition(ModuleDefinition module, TypeReference type)
        {
            if (!type.IsNested)
            {
                return module.GetType(type.Namespace, type.Name);
            }
            TypeDefinition self = type.DeclaringType.Resolve();
            if (self == null)
            {
                return null;
            }
            return GetNestedType(self, TypeFullName(type));
        }

        public override TypeDefinition Resolve(TypeReference type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            type = type.GetElementType();
            IMetadataScope scope = type.Scope;
            if (scope == null)
            {
                return null;
            }
            AssemblyNameReference assemblyName = scope as AssemblyNameReference;
            if (assemblyName != null)
            {
                if (!assemblyName.IsWindowsRuntime || this._assemblyResolver.IsAssemblyCached(assemblyName))
                {
                    AssemblyDefinition definition2 = this._assemblyResolver.Resolve(assemblyName);
                    if (definition2 == null)
                    {
                        return null;
                    }
                    TypeDefinition definition3 = GetType(definition2.MainModule, type);
                    if (definition3 != null)
                    {
                        return definition3;
                    }
                    if ((type.Module.MetadataKind != MetadataKind.Ecma335) && (assemblyName.Name == "mscorlib"))
                    {
                        return this.FindTypeInUnknownWinmd(assemblyName, type);
                    }
                    if (!assemblyName.IsWindowsRuntime)
                    {
                        throw new InvalidOperationException($"Unable to resolve [{assemblyName.Name}]{type.FullName}.");
                    }
                }
                return this.FindTypeInUnknownWinmd(assemblyName, type);
            }
            ModuleDefinition module = scope as ModuleDefinition;
            if (module != null)
            {
                return GetType(module, type);
            }
            ModuleReference reference2 = scope as ModuleReference;
            if (reference2 != null)
            {
                Collection<ModuleDefinition> modules = type.Module.Assembly.Modules;
                for (int i = 0; i < modules.Count; i++)
                {
                    ModuleDefinition definition5 = modules[i];
                    if (definition5.Name == reference2.Name)
                    {
                        return GetType(definition5, type);
                    }
                }
            }
            throw new InvalidOperationException("type.Scope isn't a valid metadata scope!");
        }

        private static string TypeFullName(TypeReference type) => 
            (!string.IsNullOrEmpty(type.Namespace) ? (type.Namespace + '.' + type.Name) : type.Name);
    }
}

