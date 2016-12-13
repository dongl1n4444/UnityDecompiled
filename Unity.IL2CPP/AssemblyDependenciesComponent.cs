using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cecil.Visitor;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP
{
	internal class AssemblyDependenciesComponent : IAssemblyDependencies
	{
		private class TypeReferenceVisitor : Visitor
		{
			private ModuleDefinition _module;

			private HashSet<AssemblyDefinition> _resolvedAssemblies = new HashSet<AssemblyDefinition>();

			public IEnumerable<AssemblyDefinition> ResolvedAssemblies
			{
				get
				{
					return this._resolvedAssemblies;
				}
			}

			public TypeReferenceVisitor(ModuleDefinition module)
			{
				this._module = module;
			}

			protected override void Visit(TypeReference typeReference, Context context)
			{
				if (typeReference.Module == this._module)
				{
					AssemblyNameReference assemblyNameReference = typeReference.Scope as AssemblyNameReference;
					if (assemblyNameReference != null)
					{
						TypeDefinition typeDefinition = typeReference.Resolve();
						if (typeDefinition == null)
						{
							throw new InvalidProgramException(string.Format("Failed to resolve [{0}]{1}.", assemblyNameReference.Name, typeReference.FullName));
						}
						this._resolvedAssemblies.Add(typeDefinition.Module.Assembly);
					}
					base.Visit(typeReference, context);
				}
			}
		}

		private Dictionary<AssemblyDefinition, IEnumerable<AssemblyDefinition>> _assemblyReferences = new Dictionary<AssemblyDefinition, IEnumerable<AssemblyDefinition>>();

		public IEnumerable<AssemblyDefinition> GetReferencedAssembliesFor(AssemblyDefinition assembly)
		{
			IEnumerable<AssemblyDefinition> enumerable;
			IEnumerable<AssemblyDefinition> result;
			if (this._assemblyReferences.TryGetValue(assembly, out enumerable))
			{
				result = enumerable;
			}
			else
			{
				enumerable = AssemblyDependenciesComponent.CollectAssemblyDependencies(assembly);
				this._assemblyReferences.Add(assembly, enumerable);
				result = enumerable;
			}
			return result;
		}

		private static IEnumerable<AssemblyDefinition> CollectAssemblyDependencies(AssemblyDefinition assembly)
		{
			HashSet<AssemblyDefinition> hashSet = new HashSet<AssemblyDefinition>();
			bool flag = false;
			foreach (AssemblyNameReference current in assembly.MainModule.AssemblyReferences)
			{
				if (!current.IsWindowsRuntime)
				{
					try
					{
						hashSet.Add(assembly.MainModule.AssemblyResolver.Resolve(current));
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
				foreach (AssemblyDefinition current2 in AssemblyDependenciesComponent.ResolveWindowsRuntimeReferences(assembly))
				{
					hashSet.Add(current2);
				}
			}
			return hashSet.ToArray<AssemblyDefinition>();
		}

		private static IEnumerable<AssemblyDefinition> ResolveWindowsRuntimeReferences(AssemblyDefinition assembly)
		{
			AssemblyDependenciesComponent.TypeReferenceVisitor typeReferenceVisitor = new AssemblyDependenciesComponent.TypeReferenceVisitor(assembly.MainModule);
			assembly.Accept(typeReferenceVisitor);
			return typeReferenceVisitor.ResolvedAssemblies;
		}
	}
}
