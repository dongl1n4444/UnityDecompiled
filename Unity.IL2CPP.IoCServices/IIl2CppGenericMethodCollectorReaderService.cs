using Mono.Cecil;
using System;
using System.Collections.Generic;

namespace Unity.IL2CPP.IoCServices
{
	public interface IIl2CppGenericMethodCollectorReaderService
	{
		IDictionary<MethodReference, uint> Items
		{
			get;
		}

		bool HasIndex(MethodReference method);

		uint GetIndex(MethodReference method);
	}
}
