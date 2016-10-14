using NiceIO;
using System;
using Unity.IL2CPP.Metadata;

namespace Unity.IL2CPP.IoCServices
{
	public interface IRuntimeInvokerCollectorWriterService
	{
		TableInfo Write(NPath path);
	}
}
