using Mono.Cecil;
using System;

namespace Unity.IL2CPP.IoCServices
{
	public interface IIl2CppGenericMethodCollectorWriterService
	{
		void Add(MethodReference method);
	}
}
