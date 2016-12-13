using Mono.Cecil;
using System;

namespace Unity.IL2CPP.Common
{
	public interface IAssemblyLoader
	{
		AssemblyDefinition Load(string name);
	}
}
