using System;

namespace Unity.IL2CPP.Common
{
	public class UnsupportedRuntimePlatform : RuntimePlatform
	{
		public override string Name
		{
			get
			{
				return "Unsupported";
			}
		}
	}
}
