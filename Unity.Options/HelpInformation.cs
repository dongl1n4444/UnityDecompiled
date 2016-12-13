using System;
using System.Reflection;

namespace Unity.Options
{
	public class HelpInformation
	{
		public string Summary;

		public FieldInfo FieldInfo;

		public string CustomValueDescription;

		public bool HasSummary
		{
			get
			{
				return !string.IsNullOrEmpty(this.Summary);
			}
		}

		public bool HasCustomValueDescription
		{
			get
			{
				return !string.IsNullOrEmpty(this.CustomValueDescription);
			}
		}
	}
}
