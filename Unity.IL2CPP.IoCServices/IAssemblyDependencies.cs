using Mono.Cecil;
using System;
using System.Collections.Generic;

namespace Unity.IL2CPP.IoCServices
{
	public interface IAssemblyDependencies
	{
		IEnumerable<AssemblyDefinition> GetReferencedAssembliesFor(AssemblyDefinition assembly);
	}
}
