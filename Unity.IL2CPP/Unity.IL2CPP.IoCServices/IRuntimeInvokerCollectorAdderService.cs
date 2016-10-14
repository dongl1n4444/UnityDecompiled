using Mono.Cecil;
using System;

namespace Unity.IL2CPP.IoCServices
{
	public interface IRuntimeInvokerCollectorAdderService
	{
		string Add(MethodReference method);
	}
}
