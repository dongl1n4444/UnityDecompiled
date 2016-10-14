using System;

namespace Unity.IL2CPP.GenericSharing
{
	public class RuntimeGenericData
	{
		public readonly RuntimeGenericContextInfo InfoType;

		public RuntimeGenericData(RuntimeGenericContextInfo infoType)
		{
			this.InfoType = infoType;
		}
	}
}
