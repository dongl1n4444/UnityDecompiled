using System;
using Unity.Options;

namespace Unity.IL2CPP.Statistics
{
	[ProgramOptions]
	public class StatisticsOptions
	{
		[HelpDetails("The directory where statistics information will be written", "path")]
		public static string StatsOutputDir = string.Empty;

		[HideFromHelp]
		public static bool EnableUniqueStatsOutputDir = false;

		[HideFromHelp]
		public static bool IncludeGenFilesWithStats = false;

		[HideFromHelp]
		public static string StatsRunName = string.Empty;

		public static void SetToDefaults()
		{
			StatisticsOptions.StatsOutputDir = string.Empty;
			StatisticsOptions.EnableUniqueStatsOutputDir = false;
			StatisticsOptions.IncludeGenFilesWithStats = false;
			StatisticsOptions.StatsRunName = string.Empty;
		}
	}
}
