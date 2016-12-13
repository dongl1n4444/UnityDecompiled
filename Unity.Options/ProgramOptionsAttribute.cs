using System;

namespace Unity.Options
{
	public class ProgramOptionsAttribute : Attribute
	{
		public string Group
		{
			get;
			set;
		}

		public string CollectionSeparator
		{
			get;
			set;
		}
	}
}
