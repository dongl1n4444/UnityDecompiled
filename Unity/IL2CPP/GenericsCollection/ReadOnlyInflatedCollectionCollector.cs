namespace Unity.IL2CPP.GenericsCollection
{
    using System;
    using Unity.IL2CPP.Common;

    public class ReadOnlyInflatedCollectionCollector
    {
        public readonly ReadOnlyHashSet<ArrayType> Arrays;
        public readonly ReadOnlyHashSet<GenericInstanceType> EmptyTypes;
        public readonly ReadOnlyHashSet<GenericInstanceMethod> Methods;
        public readonly ReadOnlyHashSet<GenericInstanceType> TypeDeclarations;
        public readonly ReadOnlyHashSet<GenericInstanceType> TypeMethodDeclarations;
        public readonly ReadOnlyHashSet<GenericInstanceType> Types;
        public readonly ReadOnlyHashSet<GenericInstanceType> WindowsRuntimeCCWs;

        public ReadOnlyInflatedCollectionCollector(InflatedCollectionCollector inflatedCollectionCollector)
        {
            this.Types = inflatedCollectionCollector.Types.Items;
            this.TypeDeclarations = inflatedCollectionCollector.TypeDeclarations.Items;
            this.TypeMethodDeclarations = inflatedCollectionCollector.TypeMethodDeclarations.Items;
            this.Methods = inflatedCollectionCollector.Methods.Items;
            this.Arrays = inflatedCollectionCollector.Arrays.Items;
            this.EmptyTypes = inflatedCollectionCollector.EmptyTypes.Items;
            this.WindowsRuntimeCCWs = inflatedCollectionCollector.WindowsRuntimeCCWs.Items;
        }
    }
}

