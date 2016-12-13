using Mono.Cecil;
using System;

namespace Unity.IL2CPP.IoCServices
{
	public interface IGuidProvider
	{
		Guid GuidFor(TypeReference type);
	}
}
