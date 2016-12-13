using Mono.Cecil;
using System;

namespace Unity.IL2CPP.IoCServices
{
	public interface ITypeProviderInitializerService
	{
		void Initialize(AssemblyDefinition mscorlib);
	}
}
