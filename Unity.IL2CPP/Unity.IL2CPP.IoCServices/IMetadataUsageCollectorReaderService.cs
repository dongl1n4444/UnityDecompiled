using Mono.Cecil;
using System;
using System.Collections.Generic;

namespace Unity.IL2CPP.IoCServices
{
	public interface IMetadataUsageCollectorReaderService
	{
		int UsageCount
		{
			get;
		}

		IEnumerable<TypeReference> GetTypeInfos();

		IEnumerable<TypeReference> GetIl2CppTypes();

		IEnumerable<MethodReference> GetInflatedMethods();

		IEnumerable<FieldReference> GetFieldInfos();

		IEnumerable<string> GetStringLiterals();

		IEnumerable<KeyValuePair<string, MetadataUsage>> GetUsages();
	}
}
