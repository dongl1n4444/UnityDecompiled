using Mono.Cecil;
using System;
using Unity.IL2CPP.Common;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP.GenericsCollection
{
	public class InflatedCollectionCollector
	{
		[Inject]
		public static IIl2CppTypeCollectorWriterService Il2CppTypeCollector;

		public readonly InflatedCollection<GenericInstanceType> Types = new InflatedCollection<GenericInstanceType>(new TypeReferenceEqualityComparer());

		public readonly InflatedCollectionForTypeDeclarations TypeDeclarations = new InflatedCollectionForTypeDeclarations(InflatedCollectionCollector.Il2CppTypeCollector);

		public readonly InflatedCollection<GenericInstanceType> TypeMethodDeclarations = new InflatedCollection<GenericInstanceType>(new TypeReferenceEqualityComparer());

		public readonly InflatedCollection<GenericInstanceMethod> Methods = new InflatedCollection<GenericInstanceMethod>(new MethodReferenceComparer());

		public readonly InflatedCollection<ArrayType> Arrays = new InflatedCollection<ArrayType>(new TypeReferenceEqualityComparer());

		public readonly InflatedCollection<GenericInstanceType> EmptyTypes = new InflatedCollection<GenericInstanceType>(new TypeReferenceEqualityComparer());

		public ReadOnlyInflatedCollectionCollector AsReadOnly()
		{
			return new ReadOnlyInflatedCollectionCollector(this);
		}
	}
}
