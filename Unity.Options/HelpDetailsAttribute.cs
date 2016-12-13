using System;

namespace Unity.Options
{
	[AttributeUsage(AttributeTargets.Field)]
	public class HelpDetailsAttribute : Attribute
	{
		public string Summary
		{
			get;
			set;
		}

		public string CustomValueDescription
		{
			get;
			set;
		}

		public HelpDetailsAttribute(string summary, string customValueDescription = null)
		{
			this.Summary = summary;
			this.CustomValueDescription = customValueDescription;
		}
	}
}
