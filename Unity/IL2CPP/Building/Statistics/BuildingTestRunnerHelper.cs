namespace Unity.IL2CPP.Building.Statistics
{
    using NiceIO;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using Unity.IL2CPP.Building;
    using Unity.IL2CPP.Portability;

    public static class BuildingTestRunnerHelper
    {
        private static readonly object _logLock = new object();
        public const string CompilationStatsFileNamePrefix = "TestRunner_CompilationStats_";

        public static NPath BuildAndLogStatsForTestRunner(this CppProgramBuilder builder)
        {
            IBuildStatistics statistics;
            return builder.BuildAndLogStatsForTestRunner(out statistics);
        }

        public static NPath BuildAndLogStatsForTestRunner(this CppProgramBuilder builder, out IBuildStatistics statistics)
        {
            NPath path = builder.Build(out statistics);
            SaveCompilationStats(statistics);
            return path;
        }

        public static IBuildStatistics LoadCompilationStats(NPath statsFile)
        {
            int totalFiles = 0;
            int cacheHits = 0;
            foreach (string str in statsFile.ReadAllLines())
            {
                char[] separator = new char[] { ',' };
                string[] strArray2 = str.Split(separator);
                totalFiles += int.Parse(strArray2[0]);
                cacheHits += int.Parse(strArray2[1]);
            }
            return new CppProgramBuildStatistics(totalFiles, cacheHits);
        }

        public static void SaveCompilationStats(IBuildStatistics stats)
        {
            if (IsRunningUnderTestRunner)
            {
                string[] append = new string[] { $"{"TestRunner_CompilationStats_"}{GetRunName}.log" };
                NPath path = StatsOutputDirectory.Combine(append);
                object obj2 = _logLock;
                lock (obj2)
                {
                    using (StreamWriter writer = new StreamWriter(path.ToString(), true))
                    {
                        writer.WriteLine("{0},{1}", stats.TotalFiles, stats.CacheHits);
                    }
                }
            }
        }

        private static string GetRunName =>
            Environment.GetEnvironmentVariable("IL2CPP_TEST_RUN_NAME");

        public static bool IsRunningUnderTestRunner =>
            !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("IL2CPP_TEST_RUNNING_UNDER_TEST_RUNNER"));

        public static bool SkipCleaningCacheAfterCppBuild
        {
            get
            {
                if (!IsRunningUnderTestRunner)
                {
                    return false;
                }
                return (UseGlobalCache || true);
            }
        }

        public static NPath StatsOutputDirectory
        {
            get
            {
                string[] append = new string[] { "results", "stats" };
                return Il2CppSpecificUtilities.GetIl2CppSolutionDirectory().ToNPath().Combine(append);
            }
        }

        public static bool UseGlobalCache
        {
            get
            {
                if (!IsRunningUnderTestRunner)
                {
                    return false;
                }
                return !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("IL2CPP_TEST_USE_GLOBAL_CACHE"));
            }
        }
    }
}

