using System;

namespace Unity.IL2CPP.Common
{
	public class PSP2RuntimePlatform : RuntimePlatform
	{
		public override string Name
		{
			get
			{
				return "PSP2";
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
