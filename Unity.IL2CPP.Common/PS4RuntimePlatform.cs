using System;

namespace Unity.IL2CPP.Common
{
	public class PS4RuntimePlatform : RuntimePlatform
	{
		public override string Name
		{
			get
			{
				return "PS4";
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
