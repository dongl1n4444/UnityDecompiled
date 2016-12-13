using Mono.Cecil;
using System;

namespace Unity.IL2CPP.IoCServices
{
	public interface IRuntimeInvokerCollectorReaderService
	{
		int GetIndex(MethodReference method);
	}
}
