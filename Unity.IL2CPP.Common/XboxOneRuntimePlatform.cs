using System;

namespace Unity.IL2CPP.Common
{
	public class XboxOneRuntimePlatform : RuntimePlatform
	{
		public override string Name
		{
			get
			{
				return "XboxOne";
			}
		}

		public override bool ExecutesOnHostMachine
		{
			get
			{
				return false;
			}
		}
	}
}
