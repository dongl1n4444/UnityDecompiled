using Mono.Cecil;
using System;

namespace Unity.IL2CPP.IoCServices
{
	public interface IWindowsRuntimeProjections
	{
		TypeReference ProjectToWindowsRuntime(TypeReference type);

		TypeReference ProjectToCLR(TypeReference type);
	}
}
