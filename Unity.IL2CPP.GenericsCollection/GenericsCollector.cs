using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Cecil.Visitor;

namespace Unity.IL2CPP.GenericsCollection
{
	public class GenericsCollector
	{
		[CompilerGenerated]
		private static Func<AssemblyDefinition, InflatedCollectionCollector> <>f__mg$cache0;

		public static InflatedCollectionCollector Collect(IEnumerable<AssemblyDefinition> assemblies)
		{
			if (GenericsCollector.<>f__mg$cache0 == null)
			{
				GenericsCollector.<>f__mg$cache0 = new Func<AssemblyDefinition, InflatedCollectionCollector>(GenericsCollector.CollectPerAssembly);
			}
			IEnumerable<InflatedCollectionCollector> collections = ParallelHelper.Select<AssemblyDefinition, InflatedCollectionCollector>(assemblies, GenericsCollector.<>f__mg$cache0, true, false);
			return GenericsCollector.MergeCollections(collections);
		}

		public static InflatedCollectionCollector Collect(TypeDefinition type)
		{
			InflatedCollectionCollector inflatedCollectionCollector = new InflatedCollectionCollector();
			GenericContextFreeVisitor visitor = new GenericContextFreeVisitor(inflatedCollectionCollector);
			type.Accept(visitor);
			return inflatedCollectionCollector;
		}

		public static void Collect(InflatedCollectionCollector generics, GenericInstanceType type)
		{
			GenericContextFreeVisitor visitor = new GenericContextFreeVisitor(generics);
			type.Accept(visitor);
		}

		private static InflatedCollectionCollector CollectPerAssembly(AssemblyDefinition assembly)
		{
			InflatedCollectionCollector inflatedCollectionCollector = new InflatedCollectionCollector();
			GenericContextFreeVisitor visitor = new GenericContextFreeVisitor(inflatedCollectionCollector);
			assembly.Accept(visitor);
			return inflatedCollectionCollector;
		}

		private static InflatedCollectionCollector MergeCollections(IEnumerable<InflatedCollectionCollector> collections)
		{
			InflatedCollectionCollector inflatedCollectionCollector = new InflatedCollectionCollector();
			foreach (InflatedCollectionCollector current in collections)
			{
				foreach (ArrayType current2 in current.Arrays.Items)
				{
					inflatedCollectionCollector.Arrays.Add(current2);
				}
				foreach (GenericInstanceType current3 in current.EmptyTypes.Items)
				{
					inflatedCollectionCollector.EmptyTypes.Add(current3);
				}
				foreach (GenericInstanceMethod current4 in current.Methods.Items)
				{
					inflatedCollectionCollector.Methods.Add(current4);
				}
				foreach (GenericInstanceType current5 in current.TypeDeclarations.Items)
				{
					inflatedCollectionCollector.TypeDeclarations.Add(current5);
				}
				foreach (GenericInstanceType current6 in current.TypeMethodDeclarations.Items)
				{
					inflatedCollectionCollector.TypeMethodDeclarations.Add(current6);
				}
				foreach (GenericInstanceType current7 in current.Types.Items)
				{
					inflatedCollectionCollector.Types.Add(current7);
				}
			}
			return inflatedCollectionCollector;
		}
	}
}
