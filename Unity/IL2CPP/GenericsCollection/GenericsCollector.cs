namespace Unity.IL2CPP.GenericsCollection
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Unity.Cecil.Visitor;
    using Unity.IL2CPP;

    public class GenericsCollector
    {
        [CompilerGenerated]
        private static Func<AssemblyDefinition, InflatedCollectionCollector> <>f__mg$cache0;

        public static InflatedCollectionCollector Collect(TypeDefinition type)
        {
            InflatedCollectionCollector generics = new InflatedCollectionCollector();
            GenericContextFreeVisitor visitor = new GenericContextFreeVisitor(generics);
            Unity.Cecil.Visitor.Extensions.Accept(type, visitor);
            return generics;
        }

        public static InflatedCollectionCollector Collect(IEnumerable<AssemblyDefinition> assemblies)
        {
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new Func<AssemblyDefinition, InflatedCollectionCollector>(null, (IntPtr) CollectPerAssembly);
            }
            return MergeCollections(ParallelHelper.Select<AssemblyDefinition, InflatedCollectionCollector>(assemblies, <>f__mg$cache0, true, false));
        }

        public static void Collect(InflatedCollectionCollector generics, GenericInstanceType type)
        {
            GenericContextFreeVisitor visitor = new GenericContextFreeVisitor(generics);
            Unity.Cecil.Visitor.Extensions.Accept(type, visitor);
        }

        private static InflatedCollectionCollector CollectPerAssembly(AssemblyDefinition assembly)
        {
            InflatedCollectionCollector generics = new InflatedCollectionCollector();
            GenericContextFreeVisitor visitor = new GenericContextFreeVisitor(generics);
            Unity.Cecil.Visitor.Extensions.Accept(assembly, visitor);
            return generics;
        }

        private static InflatedCollectionCollector MergeCollections(IEnumerable<InflatedCollectionCollector> collections)
        {
            InflatedCollectionCollector collector = new InflatedCollectionCollector();
            foreach (InflatedCollectionCollector collector2 in collections)
            {
                foreach (ArrayType type in collector2.Arrays.Items)
                {
                    collector.Arrays.Add(type);
                }
                foreach (GenericInstanceType type2 in collector2.EmptyTypes.Items)
                {
                    collector.EmptyTypes.Add(type2);
                }
                foreach (GenericInstanceMethod method in collector2.Methods.Items)
                {
                    collector.Methods.Add(method);
                }
                foreach (GenericInstanceType type3 in collector2.TypeDeclarations.Items)
                {
                    collector.TypeDeclarations.Add(type3);
                }
                foreach (GenericInstanceType type4 in collector2.TypeMethodDeclarations.Items)
                {
                    collector.TypeMethodDeclarations.Add(type4);
                }
                foreach (GenericInstanceType type5 in collector2.Types.Items)
                {
                    collector.Types.Add(type5);
                }
            }
            return collector;
        }
    }
}

