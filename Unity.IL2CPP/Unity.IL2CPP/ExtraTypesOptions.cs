using System;
using Unity.Options;

namespace Unity.IL2CPP
{
	[ProgramOptions(Group = "extra-types", CollectionSeparator = "|")]
	public sealed class ExtraTypesOptions
	{
		[HideFromHelp]
		public static string[] Name;

		[HideFromHelp]
		public static string[] File;
	}
}
