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

        public ReadOnlyInflatedCollectionCollector(InflatedCollectionCollector inflatedCollectionCollector)
        {
            this.Types = inflatedCollectionCollector.Types.Items;
            this.TypeDeclarations = inflatedCollectionCollector.TypeDeclarations.Items;
            this.TypeMethodDeclarations = inflatedCollectionCollector.TypeDeclarations.Items;
            this.Methods = inflatedCollectionCollector.Methods.Items;
            this.Arrays = inflatedCollectionCollector.Arrays.Items;
            this.EmptyTypes = inflatedCollectionCollector.EmptyTypes.Items;
        }
    }
}

