using System;
using Unity.IL2CPP.Common;
using Unity.Options;

namespace Unity.IL2CPP
{
	[ProgramOptions]
	public class CodeGenOptions
	{
		[HelpDetails("Enables generation of null checks", null)]
		public static bool EmitNullChecks = false;

		[HelpDetails("Enables generation of stacktrace sentries in C++ code at the start of every managed method. This enables support for stacktraces for platforms that do not have system APIs to walk the stack (for example, one such platform is WebGL)", null)]
		public static bool EnableStacktrace = false;

		[HideFromHelp]
		public static bool EnableDeepProfiler = false;

		[HelpDetails("Enables conversion statistics", null)]
		public static bool EnableStats = false;

		[HelpDetails("Enables generation of array bounds checks", null)]
		public static bool EnableArrayBoundsCheck = false;

		[HelpDetails("Enables generation of divide by zero checks", null)]
		public static bool EnableDivideByZeroCheck = false;

		[HideFromHelp]
		public static bool EnableErrorMessageTest = false;

		[HideFromHelp]
		public static bool EnableSymbolLoading = false;

		[HideFromHelp]
		public static bool EnablePrimitiveValueTypeGenericSharing = false;

		[HideFromHelp]
		public static DotNetProfile Dotnetprofile;

		[HideFromHelp]
		public static ProfilerOptions ProfilerOptions = ProfilerOptions.MethodEnterExit;

		[HideFromHelp]
		public static bool DevelopmentMode = false;

		[HideFromHelp]
		public static bool DumpRuntimeStats = false;

		[HideFromHelp]
		public static bool ForceSerial = false;

		[HideFromHelp]
		public static bool ForceSerialBetaOnly = false;

		public static bool EmitComments
		{
			get
			{
				return true;
			}
		}

		public static void SetToDefaults()
		{
			CodeGenOptions.EmitNullChecks = false;
			CodeGenOptions.EnableStacktrace = false;
			CodeGenOptions.EnableDeepProfiler = false;
			CodeGenOptions.EnableStats = false;
			CodeGenOptions.EnableArrayBoundsCheck = false;
			CodeGenOptions.EnableErrorMessageTest = false;
			CodeGenOptions.EnableDivideByZeroCheck = false;
			CodeGenOptions.EnableSymbolLoading = false;
			CodeGenOptions.EnablePrimitiveValueTypeGenericSharing = false;
			CodeGenOptions.Dotnetprofile = DotNetProfile.Net20;
			CodeGenOptions.ProfilerOptions = ProfilerOptions.MethodEnterExit;
			CodeGenOptions.DevelopmentMode = false;
			CodeGenOptions.DumpRuntimeStats = false;
			CodeGenOptions.ForceSerial = ParallelHelper.ForceSerialByDefault;
			CodeGenOptions.ForceSerialBetaOnly = false;
		}
	}
}
