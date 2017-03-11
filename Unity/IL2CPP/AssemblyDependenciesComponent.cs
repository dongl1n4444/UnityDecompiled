namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Unity.Cecil.Visitor;
    using Unity.IL2CPP.IoCServices;

    public class AssemblyDependenciesComponent : IAssemblyDependencies
    {
        private Dictionary<AssemblyDefinition, IEnumerable<AssemblyDefinition>> _assemblyReferences = new Dictionary<AssemblyDefinition, IEnumerable<AssemblyDefinition>>();

        private static IEnumerable<AssemblyDefinition> CollectAssemblyDependencies(AssemblyDefinition assembly)
        {
            HashSet<AssemblyDefinition> source = new HashSet<AssemblyDefinition>();
            bool flag = false;
            foreach (AssemblyNameReference reference in assembly.MainModule.AssemblyReferences)
            {
                if (!reference.IsWindowsRuntime)
                {
                    try
                    {
                        source.Add(assembly.MainModule.AssemblyResolver.Resolve(reference));
                    }
                    catch (AssemblyResolutionException)
                    {
                    }
                }
                else
                {
                    flag = true;
                }
            }
            if (flag)
            {
                foreach (AssemblyDefinition definition in ResolveWindowsRuntimeReferences(assembly))
                {
                    source.Add(definition);
                }
            }
            return source.ToArray<AssemblyDefinition>();
        }

        public IEnumerable<AssemblyDefinition> GetReferencedAssembliesFor(AssemblyDefinition assembly)
        {
            IEnumerable<AssemblyDefinition> enumerable;
            if (!this._assemblyReferences.TryGetValue(assembly, out enumerable))
            {
                enumerable = CollectAssemblyDependencies(assembly);
                this._assemblyReferences.Add(assembly, enumerable);
            }
            return enumerable;
        }

        private static IEnumerable<AssemblyDefinition> ResolveWindowsRuntimeReferences(AssemblyDefinition assembly)
        {
            TypeReferenceVisitor visitor = new TypeReferenceVisitor(assembly.MainModule);
            assembly.Accept(visitor);
            return visitor.ResolvedAssemblies;
        }

        private class TypeReferenceVisitor : Unity.Cecil.Visitor.Visitor
        {
            private ModuleDefinition _module;
            private HashSet<AssemblyDefinition> _resolvedAssemblies = new HashSet<AssemblyDefinition>();

            public TypeReferenceVisitor(ModuleDefinition module)
            {
                this._module = module;
            }

            protected override void Visit(TypeReference typeReference, Unity.Cecil.Visitor.Context context)
            {
                if (typeReference.Module == this._module)
                {
                    AssemblyNameReference scope = typeReference.Scope as AssemblyNameReference;
                    if (scope != null)
                    {
                        TypeDefinition definition = typeReference.Resolve();
                        if (definition == null)
                        {
                            throw new InvalidProgramException($"Failed to resolve [{scope.Name}]{typeReference.FullName}.");
                        }
                        this._resolvedAssemblies.Add(definition.Module.Assembly);
                    }
                    base.Visit(typeReference, context);
                }
            }

            public IEnumerable<AssemblyDefinition> ResolvedAssemblies =>
                this._resolvedAssemblies;
        }
    }
}

