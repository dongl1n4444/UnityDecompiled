using Mono.Cecil;
using System;

namespace Unity.IL2CPP.GenericSharing
{
	public class RuntimeGenericMethodData : RuntimeGenericData
	{
		public MethodReference Data;

		public MethodReference GenericMethod;

		public RuntimeGenericMethodData(RuntimeGenericContextInfo infoType, MethodReference data, MethodReference genericMethod) : base(infoType)
		{
			this.Data = data;
			this.GenericMethod = genericMethod;
		}
	}
}
