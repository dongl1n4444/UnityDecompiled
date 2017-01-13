namespace Unity.IL2CPP.Statistics
{
    using NiceIO;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP.Common;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Portability;
    using Unity.TinyProfiling;

    public static class StatisticsGenerator
    {
        [CompilerGenerated]
        private static Func<NPath, string> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<string, FileInfo> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<NPath, bool> <>f__mg$cache0;
        private const string StatisticsLogFileName = "statistics.txt";

        private static List<FileInfo> CollectGeneratedFileInfo(NPath outputDirectory)
        {
            if (!outputDirectory.Exists(""))
            {
                return new List<FileInfo>();
            }
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new Func<NPath, bool>(StatisticsGenerator.IsGeneratedFile);
            }
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = f => f.ToString();
            }
            List<string> source = outputDirectory.Files(false).Where<NPath>(<>f__mg$cache0).Select<NPath, string>(<>f__am$cache0).ToList<string>();
            source.Sort();
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = f => new FileInfo(f.ToString());
            }
            return source.Select<string, FileInfo>(<>f__am$cache1).ToList<FileInfo>();
        }

        private static void CopyGeneratedFilesIntoStatsOutput(NPath statsOutputDirectory, NPath generatedCppDirectory)
        {
            string[] append = new string[] { "generated" };
            NPath destination = statsOutputDirectory.Combine(append);
            if (destination.Exists(""))
            {
                destination.Delete(DeleteMode.Normal);
            }
            if (!destination.Exists(""))
            {
                destination.CreateDirectory();
            }
            generatedCppDirectory.CopyFiles(destination, true, null);
        }

        public static NPath DetermineAndSetupOutputDirectory()
        {
            NPath path = StatisticsOptions.StatsOutputDir.ToNPath();
            if (!path.Exists(""))
            {
                path.CreateDirectory();
            }
            if (StatisticsOptions.EnableUniqueStatsOutputDir)
            {
                string randomFileName = Path.GetRandomFileName();
                string statsRunName = StatisticsOptions.StatsRunName;
                if (!string.IsNullOrEmpty(statsRunName))
                {
                    randomFileName = statsRunName + "_-_" + randomFileName;
                }
                string[] append = new string[] { randomFileName };
                path = path.Combine(append);
                path.CreateDirectory();
            }
            return path;
        }

        public static void Generate(NPath statsOutputDirectory, NPath generatedCppDirectory, IStatsService statsService, ProfilerSnapshot profilerSnapshot, IEnumerable<string> commandLineArguments, IEnumerable<NPath> assembliesConverted)
        {
            WriteSingleLog(statsOutputDirectory, generatedCppDirectory, statsService, profilerSnapshot, commandLineArguments, assembliesConverted);
            WriteBasicDataToStdout(statsOutputDirectory, profilerSnapshot);
            if (StatisticsOptions.IncludeGenFilesWithStats)
            {
                CopyGeneratedFilesIntoStatsOutput(statsOutputDirectory, generatedCppDirectory);
            }
        }

        private static bool IsGeneratedFile(NPath path)
        {
            string[] extensions = new string[] { "cpp", "h" };
            return path.HasExtension(extensions);
        }

        private static void WriteAssemblyConversionTimes(TextWriter writer, ProfilerSnapshot profilerSnapshot)
        {
            writer.WriteLine("----Assembly Conversion Times----");
            List<TinyProfiler.TimedSection> list = profilerSnapshot.GetSectionsByLabel("Convert").ToList<TinyProfiler.TimedSection>();
            list.Sort(new TimedSectionSummaryComparer());
            foreach (TinyProfiler.TimedSection section in list)
            {
                writer.WriteLine("\t{0}", section.Summary);
            }
            writer.WriteLine();
        }

        private static void WriteBasicDataToStdout(NPath statsOutputDirectory, ProfilerSnapshot profilerSnapshot)
        {
            WriteAssemblyConversionTimes(Console.Out, profilerSnapshot);
            WriteMiscTimes(Console.Out, profilerSnapshot);
            Console.WriteLine("Statistics written to : {0}", statsOutputDirectory);
        }

        private static void WriteGeneralLog(TextWriter writer, IEnumerable<string> commandLineArguments, IEnumerable<NPath> assembliesConverted)
        {
            writer.WriteLine("----IL2CPP Arguments----");
            writer.WriteLine(commandLineArguments.SeparateWithSpaces());
            writer.WriteLine();
            writer.WriteLine("----Assemblies Converted-----");
            foreach (NPath path in assembliesConverted)
            {
                writer.WriteLine("\t{0}", path);
            }
            writer.WriteLine();
        }

        private static void WriteGeneratedFileLog(TextWriter writer, NPath generatedOutputDirectory)
        {
            List<FileInfo> list = CollectGeneratedFileInfo(generatedOutputDirectory);
            writer.WriteLine("----Generated Files----");
            foreach (FileInfo info in list)
            {
                writer.WriteLine(info.Name + "\t" + info.Length);
            }
            writer.WriteLine();
        }

        private static void WriteMiscTimes(TextWriter writer, ProfilerSnapshot profilerSnapshot)
        {
            writer.WriteLine("----Misc Timing----");
            writer.WriteLine("\tPreProcessStage: {0}", profilerSnapshot.GetSectionsByLabel("PreProcessStage").First<TinyProfiler.TimedSection>().Duration);
            writer.WriteLine("\tGenericsCollector.Collect: {0}", profilerSnapshot.GetSectionsByLabel("GenericsCollector.Collect").First<TinyProfiler.TimedSection>().Duration);
            writer.WriteLine("\tWriteGenerics: {0}", profilerSnapshot.GetSectionsByLabel("WriteGenerics").First<TinyProfiler.TimedSection>().Duration);
            writer.WriteLine("\tAllAssemblyConversion: {0}", profilerSnapshot.GetSectionsByLabel("AllAssemblyConversion").First<TinyProfiler.TimedSection>().Duration);
            writer.WriteLine();
        }

        private static void WriteProfilerLog(TextWriter writer, ProfilerSnapshot profilerSnapshot)
        {
            WriteAssemblyConversionTimes(writer, profilerSnapshot);
            WriteMiscTimes(writer, profilerSnapshot);
        }

        private static void WriteSingleLog(NPath statsOutputDirectory, NPath generatedCppDirectory, IStatsService statsService, ProfilerSnapshot profilerSnapshot, IEnumerable<string> commandLineArguments, IEnumerable<NPath> assembliesConverted)
        {
            string[] append = new string[] { "statistics.txt" };
            using (Unity.IL2CPP.Portability.StreamWriter writer = new Unity.IL2CPP.Portability.StreamWriter(statsOutputDirectory.Combine(append).ToString()))
            {
                WriteGeneralLog(writer, commandLineArguments, assembliesConverted);
                WriteStatsLog(writer, statsService);
                WriteProfilerLog(writer, profilerSnapshot);
                WriteGeneratedFileLog(writer, generatedCppDirectory);
            }
        }

        private static void WriteStatsLog(TextWriter writer, IStatsService statsService)
        {
            statsService.WriteStats(writer);
            writer.WriteLine();
        }

        private class TimedSectionSummaryComparer : IComparer<TinyProfiler.TimedSection>
        {
            public int Compare(TinyProfiler.TimedSection x, TinyProfiler.TimedSection y) => 
                x.Summary.CompareTo(y.Summary);
        }
    }
}

