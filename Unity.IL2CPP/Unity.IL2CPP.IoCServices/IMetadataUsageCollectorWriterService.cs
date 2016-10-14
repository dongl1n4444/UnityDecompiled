using System;

namespace Unity.IL2CPP.IoCServices
{
	public interface IMetadataUsageCollectorWriterService
	{
		void Add(string identifier, MetadataUsage usage);
	}
}
