using Mono.Cecil;
using NiceIO;
using System;
using System.Collections.Generic;

namespace Unity.IL2CPP.Common
{
	public interface IAssemblyResolver : Mono.Cecil.IAssemblyResolver
	{
		IEnumerable<NPath> GetSearchDirectories();

		bool IsAssemblyCached(AssemblyNameReference assemblyName);
	}
}
