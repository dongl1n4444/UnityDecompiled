using Mono.Cecil;
using System;

namespace Unity.IL2CPP.GenericSharing
{
	public class RuntimeGenericInflatedTypeData : RuntimeGenericTypeData
	{
		public readonly TypeReference Data;

		public RuntimeGenericInflatedTypeData(RuntimeGenericContextInfo infoType, TypeReference genericType, TypeReference data) : base(infoType, genericType)
		{
			this.Data = data;
		}
	}
}
