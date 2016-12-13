using Mono.Cecil;
using System;
using Unity.IL2CPP.Common;

namespace Unity.IL2CPP.IoCServices
{
	public interface IWindowsRuntimeProjectionsInitializer
	{
		void Initialize(ModuleDefinition mscorlib, DotNetProfile dotNetProfile);
	}
}
