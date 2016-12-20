namespace Unity.IL2CPP.GenericsCollection
{
    using System;
    using Unity.IL2CPP;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    public class InflatedCollectionCollector
    {
        public readonly InflatedCollection<ArrayType> Arrays = new InflatedCollection<ArrayType>(new TypeReferenceEqualityComparer());
        public readonly InflatedCollection<GenericInstanceType> EmptyTypes = new InflatedCollection<GenericInstanceType>(new TypeReferenceEqualityComparer());
        [Inject]
        public static IIl2CppTypeCollectorWriterService Il2CppTypeCollector;
        public readonly InflatedCollection<GenericInstanceMethod> Methods = new InflatedCollection<GenericInstanceMethod>(new MethodReferenceComparer());
        public readonly InflatedCollectionForTypeDeclarations TypeDeclarations = new InflatedCollectionForTypeDeclarations(Il2CppTypeCollector);
        public readonly InflatedCollection<GenericInstanceType> TypeMethodDeclarations = new InflatedCollection<GenericInstanceType>(new TypeReferenceEqualityComparer());
        public readonly InflatedCollection<GenericInstanceType> Types = new InflatedCollection<GenericInstanceType>(new TypeReferenceEqualityComparer());

        public ReadOnlyInflatedCollectionCollector AsReadOnly()
        {
            return new ReadOnlyInflatedCollectionCollector(this);
        }
    }
}

