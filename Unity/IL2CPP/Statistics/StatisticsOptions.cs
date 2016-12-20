namespace Unity.IL2CPP.Statistics
{
    using System;
    using Unity.Options;

    [ProgramOptions]
    public class StatisticsOptions
    {
        [HideFromHelp]
        public static bool EnableUniqueStatsOutputDir = false;
        [HideFromHelp]
        public static bool IncludeGenFilesWithStats = false;
        [HelpDetails("The directory where statistics information will be written", "path")]
        public static string StatsOutputDir = string.Empty;
        [HideFromHelp]
        public static string StatsRunName = string.Empty;

        public static void SetToDefaults()
        {
            StatsOutputDir = string.Empty;
            EnableUniqueStatsOutputDir = false;
            IncludeGenFilesWithStats = false;
            StatsRunName = string.Empty;
        }
    }
}

