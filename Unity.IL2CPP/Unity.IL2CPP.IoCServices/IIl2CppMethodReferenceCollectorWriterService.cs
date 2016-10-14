using Mono.Cecil;
using System;
using Unity.IL2CPP.Metadata;

namespace Unity.IL2CPP.IoCServices
{
	public interface IIl2CppMethodReferenceCollectorWriterService
	{
		uint GetOrCreateIndex(MethodReference method, IMetadataCollection metadataCollection);
	}
}
