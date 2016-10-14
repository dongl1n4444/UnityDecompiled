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

namespace Unity.IL2CPP.Statistics
{
	public static class StatisticsGenerator
	{
		private class TimedSectionSummaryComparer : IComparer<TinyProfiler.TimedSection>
		{
			public int Compare(TinyProfiler.TimedSection x, TinyProfiler.TimedSection y)
			{
				return x.Summary.CompareTo(y.Summary);
			}
		}

		private const string StatisticsLogFileName = "statistics.txt";

		[CompilerGenerated]
		private static Func<NPath, bool> <>f__mg$cache0;

		public static NPath DetermineAndSetupOutputDirectory()
		{
			NPath nPath = StatisticsOptions.StatsOutputDir.ToNPath();
			if (!nPath.Exists(""))
			{
				nPath.CreateDirectory();
			}
			if (StatisticsOptions.EnableUniqueStatsOutputDir)
			{
				string text = Path.GetRandomFileName();
				string statsRunName = StatisticsOptions.StatsRunName;
				if (!string.IsNullOrEmpty(statsRunName))
				{
					text = statsRunName + "_-_" + text;
				}
				nPath = nPath.Combine(new string[]
				{
					text
				});
				nPath.CreateDirectory();
			}
			return nPath;
		}

		public static void Generate(NPath statsOutputDirectory, NPath generatedCppDirectory, IStatsService statsService, ProfilerSnapshot profilerSnapshot, IEnumerable<string> commandLineArguments, IEnumerable<NPath> assembliesConverted)
		{
			StatisticsGenerator.WriteSingleLog(statsOutputDirectory, generatedCppDirectory, statsService, profilerSnapshot, commandLineArguments, assembliesConverted);
			StatisticsGenerator.WriteBasicDataToStdout(statsOutputDirectory, profilerSnapshot);
			if (StatisticsOptions.IncludeGenFilesWithStats)
			{
				StatisticsGenerator.CopyGeneratedFilesIntoStatsOutput(statsOutputDirectory, generatedCppDirectory);
			}
		}

		private static void WriteSingleLog(NPath statsOutputDirectory, NPath generatedCppDirectory, IStatsService statsService, ProfilerSnapshot profilerSnapshot, IEnumerable<string> commandLineArguments, IEnumerable<NPath> assembliesConverted)
		{
			using (Unity.IL2CPP.Portability.StreamWriter streamWriter = new Unity.IL2CPP.Portability.StreamWriter(statsOutputDirectory.Combine(new string[]
			{
				"statistics.txt"
			}).ToString()))
			{
				StatisticsGenerator.WriteGeneralLog(streamWriter, commandLineArguments, assembliesConverted);
				StatisticsGenerator.WriteStatsLog(streamWriter, statsService);
				StatisticsGenerator.WriteProfilerLog(streamWriter, profilerSnapshot);
				StatisticsGenerator.WriteGeneratedFileLog(streamWriter, generatedCppDirectory);
			}
		}

		private static void WriteGeneralLog(TextWriter writer, IEnumerable<string> commandLineArguments, IEnumerable<NPath> assembliesConverted)
		{
			writer.WriteLine("----IL2CPP Arguments----");
			writer.WriteLine(commandLineArguments.SeparateWithSpaces());
			writer.WriteLine();
			writer.WriteLine("----Assemblies Converted-----");
			foreach (NPath current in assembliesConverted)
			{
				writer.WriteLine("\t{0}", current);
			}
			writer.WriteLine();
		}

		private static void WriteGeneratedFileLog(TextWriter writer, NPath generatedOutputDirectory)
		{
			List<FileInfo> list = StatisticsGenerator.CollectGeneratedFileInfo(generatedOutputDirectory);
			writer.WriteLine("----Generated Files----");
			foreach (FileInfo current in list)
			{
				writer.WriteLine(current.Name + "\t" + current.Length);
			}
			writer.WriteLine();
		}

		private static void WriteStatsLog(TextWriter writer, IStatsService statsService)
		{
			statsService.WriteStats(writer);
			writer.WriteLine();
		}

		private static void WriteProfilerLog(TextWriter writer, ProfilerSnapshot profilerSnapshot)
		{
			StatisticsGenerator.WriteAssemblyConversionTimes(writer, profilerSnapshot);
			StatisticsGenerator.WriteMiscTimes(writer, profilerSnapshot);
		}

		private static void WriteAssemblyConversionTimes(TextWriter writer, ProfilerSnapshot profilerSnapshot)
		{
			writer.WriteLine("----Assembly Conversion Times----");
			List<TinyProfiler.TimedSection> list = profilerSnapshot.GetSectionsByLabel("Convert").ToList<TinyProfiler.TimedSection>();
			list.Sort(new StatisticsGenerator.TimedSectionSummaryComparer());
			foreach (TinyProfiler.TimedSection current in list)
			{
				writer.WriteLine("\t{0}", current.Summary);
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

		private static void WriteBasicDataToStdout(NPath statsOutputDirectory, ProfilerSnapshot profilerSnapshot)
		{
			StatisticsGenerator.WriteAssemblyConversionTimes(Console.Out, profilerSnapshot);
			StatisticsGenerator.WriteMiscTimes(Console.Out, profilerSnapshot);
			Console.WriteLine("Statistics written to : {0}", statsOutputDirectory);
		}

		private static void CopyGeneratedFilesIntoStatsOutput(NPath statsOutputDirectory, NPath generatedCppDirectory)
		{
			NPath nPath = statsOutputDirectory.Combine(new string[]
			{
				"generated"
			});
			if (nPath.Exists(""))
			{
				nPath.Delete(DeleteMode.Normal);
			}
			if (!nPath.Exists(""))
			{
				nPath.CreateDirectory();
			}
			generatedCppDirectory.CopyFiles(nPath, true, null);
		}

		private static List<FileInfo> CollectGeneratedFileInfo(NPath outputDirectory)
		{
			List<FileInfo> result;
			if (!outputDirectory.Exists(""))
			{
				result = new List<FileInfo>();
			}
			else
			{
				IEnumerable<NPath> arg_41_0 = outputDirectory.Files(false);
				if (StatisticsGenerator.<>f__mg$cache0 == null)
				{
					StatisticsGenerator.<>f__mg$cache0 = new Func<NPath, bool>(StatisticsGenerator.IsGeneratedFile);
				}
				List<string> list = (from f in arg_41_0.Where(StatisticsGenerator.<>f__mg$cache0)
				select f.ToString()).ToList<string>();
				list.Sort();
				result = (from f in list
				select new FileInfo(f.ToString())).ToList<FileInfo>();
			}
			return result;
		}

		private static bool IsGeneratedFile(NPath path)
		{
			return path.HasExtension(new string[]
			{
				"cpp",
				"h"
			});
		}
	}
}
