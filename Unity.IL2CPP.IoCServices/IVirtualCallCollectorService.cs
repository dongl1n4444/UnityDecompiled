using Mono.Cecil;
using NiceIO;
using System;

namespace Unity.IL2CPP.IoCServices
{
	public interface IVirtualCallCollectorService
	{
		int Count
		{
			get;
		}

		void AddMethod(MethodReference method);

		UnresolvedVirtualsTablesInfo WriteUnresolvedStubs(NPath outputDir);
	}
}
