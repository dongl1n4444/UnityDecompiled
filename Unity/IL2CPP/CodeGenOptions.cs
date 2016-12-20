namespace Unity.IL2CPP
{
    using System;
    using Unity.IL2CPP.Common;
    using Unity.Options;

    [ProgramOptions]
    public class CodeGenOptions
    {
        [HideFromHelp]
        public static bool DevelopmentMode = false;
        [HideFromHelp]
        public static DotNetProfile Dotnetprofile;
        [HideFromHelp]
        public static bool DumpRuntimeStats = false;
        [HelpDetails("Enables generation of null checks", null)]
        public static bool EmitNullChecks = false;
        [HelpDetails("Enables generation of array bounds checks", null)]
        public static bool EnableArrayBoundsCheck = false;
        [HideFromHelp]
        public static bool EnableDeepProfiler = false;
        [HelpDetails("Enables generation of divide by zero checks", null)]
        public static bool EnableDivideByZeroCheck = false;
        [HideFromHelp]
        public static bool EnableErrorMessageTest = false;
        [HideFromHelp]
        public static bool EnablePrimitiveValueTypeGenericSharing = false;
        [HelpDetails("Enables generation of stacktrace sentries in C++ code at the start of every managed method. This enables support for stacktraces for platforms that do not have system APIs to walk the stack (for example, one such platform is WebGL)", null)]
        public static bool EnableStacktrace = false;
        [HelpDetails("Enables conversion statistics", null)]
        public static bool EnableStats = false;
        [HideFromHelp]
        public static bool EnableSymbolLoading = false;
        [HideFromHelp]
        public static bool ForceSerial = false;
        [HideFromHelp]
        public static bool ForceSerialBetaOnly = false;
        [HideFromHelp]
        public static Unity.IL2CPP.ProfilerOptions ProfilerOptions = Unity.IL2CPP.ProfilerOptions.MethodEnterExit;

        public static void SetToDefaults()
        {
            EmitNullChecks = false;
            EnableStacktrace = false;
            EnableDeepProfiler = false;
            EnableStats = false;
            EnableArrayBoundsCheck = false;
            EnableErrorMessageTest = false;
            EnableDivideByZeroCheck = false;
            EnableSymbolLoading = false;
            EnablePrimitiveValueTypeGenericSharing = false;
            Dotnetprofile = DotNetProfile.Net20;
            ProfilerOptions = Unity.IL2CPP.ProfilerOptions.MethodEnterExit;
            DevelopmentMode = false;
            DumpRuntimeStats = false;
            ForceSerial = Unity.IL2CPP.ParallelHelper.ForceSerialByDefault;
            ForceSerialBetaOnly = false;
        }

        public static bool EmitComments
        {
            get
            {
                return true;
            }
        }
    }
}

