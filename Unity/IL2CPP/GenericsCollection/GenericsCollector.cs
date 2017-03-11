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
        public static InflatedCollectionCollector Collect(IInteropDataCollector interopDataCollector, TypeDefinition type)
        {
            InflatedCollectionCollector generics = new InflatedCollectionCollector();
            GenericContextFreeVisitor visitor = new GenericContextFreeVisitor(generics);
            type.Accept(visitor);
            return generics;
        }

        public static InflatedCollectionCollector Collect(IInteropDataCollector interopDataCollector, IEnumerable<AssemblyDefinition> assemblies)
        {
            <Collect>c__AnonStorey0 storey = new <Collect>c__AnonStorey0 {
                interopDataCollector = interopDataCollector
            };
            return MergeCollections(ParallelHelper.Select<AssemblyDefinition, InflatedCollectionCollector>(assemblies, new Func<AssemblyDefinition, InflatedCollectionCollector>(storey.<>m__0), true, false));
        }

        public static void Collect(InflatedCollectionCollector generics, IInteropDataCollector interopDataCollector, GenericInstanceType type)
        {
            GenericContextFreeVisitor visitor = new GenericContextFreeVisitor(generics);
            type.Accept(visitor);
        }

        private static InflatedCollectionCollector CollectPerAssembly(IInteropDataCollector interopDataCollector, AssemblyDefinition assembly)
        {
            InflatedCollectionCollector generics = new InflatedCollectionCollector();
            GenericContextFreeVisitor visitor = new GenericContextFreeVisitor(generics);
            assembly.Accept(visitor);
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

        [CompilerGenerated]
        private sealed class <Collect>c__AnonStorey0
        {
            internal IInteropDataCollector interopDataCollector;

            internal InflatedCollectionCollector <>m__0(AssemblyDefinition assembly) => 
                GenericsCollector.CollectPerAssembly(this.interopDataCollector, assembly);
        }
    }
}

