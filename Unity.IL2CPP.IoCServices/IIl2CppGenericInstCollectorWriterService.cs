using Mono.Cecil;
using System;
using System.Collections.Generic;

namespace Unity.IL2CPP.IoCServices
{
	public interface IIl2CppGenericInstCollectorWriterService
	{
		void Add(IList<TypeReference> types);
	}
}
