namespace Unity.IL2CPP.Running
{
    using NiceIO;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Building.ToolChains.Android;
    using Unity.IL2CPP.Common;
    using Unity.IL2CPP.Running.Android;

    internal class AndroidRunner : Runner
    {
        [CompilerGenerated]
        private static Func<string, string> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<string, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<string, string> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<string, bool> <>f__am$cache3;
        private const string DeviceDataDirectory = "/data/local/tmp/il2cpp-test/Data";
        private const string DeviceRootDirectory = "/data/local/tmp/il2cpp-test";
        private const string DeviceTempDirectory = "/data/local/tmp/il2cpp-test/tmp";
        private static readonly TimeSpan RunTimeout = TimeSpan.FromMinutes(15.0);

        private static void GenerateDebugScript(NPath localExecutable, string deviceRootDirectory, string deviceExecutableFileName, AndroidNDKUtilities ndk)
        {
            NPath parent = localExecutable.Parent;
            string[] append = new string[] { "debug" };
            NPath debugDirectory = parent.Combine(append);
            string[] textArray2 = new string[] { "lib" };
            NPath libDirectory = debugDirectory.Combine(textArray2);
            debugDirectory.CreateDirectory();
            NPath path4 = GenerateGdbInitFile(debugDirectory, libDirectory, parent, 0x4d2);
            string str = !PlatformUtils.IsWindows() ? string.Empty : ".exe";
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("#!/usr/bin/env perl");
            builder.AppendLine("use strict;");
            builder.AppendLine("use warnings;");
            builder.AppendLine($"my $adb = "$ENV{{ANDROID_SDK_ROOT}}/platform-tools/adb{str}";");
            builder.AppendLine($"my $gdb = "{ndk.GdbPath.ToString(SlashMode.Forward)}{str}";");
            builder.AppendLine($"my $hostport = {0x4d2};");
            builder.AppendLine($"my $deviceport = {0x4d2};");
            builder.AppendLine($"my $devicedir = "{deviceRootDirectory}";");
            builder.AppendLine($"my $deviceexe = "{deviceExecutableFileName}";");
            builder.AppendLine($"my $hostexe = "{localExecutable.ToString(SlashMode.Forward)}.debug";");
            builder.AppendLine($"my $lib = "{libDirectory.ToString(SlashMode.Forward)}";");
            builder.AppendLine($"my $gdbinit = "{path4.ToString(SlashMode.Forward)}";");
            builder.AppendLine("sub adb");
            builder.AppendLine("{");
            builder.AppendLine("    my $async = shift;");
            builder.AppendLine("    return system($adb, @_) unless $async;");
            builder.AppendLine("    if ($^O eq \"MSWin32\")");
            builder.AppendLine("    {");
            builder.AppendLine("        system(\"start \\\"debug.pl ADB (async)\\\" /b $adb\", @_)");
            builder.AppendLine("    }");
            builder.AppendLine("    else");
            builder.AppendLine("    {");
            builder.AppendLine("        system($adb, @_, \"&>/dev/null &\");");
            builder.AppendLine("    }");
            builder.AppendLine("}");
            builder.AppendLine("die \"ADB not found. Make sure environment variable ANDROID_SDK_ROOT is set\" unless -e $adb;");
            builder.AppendLine("adb(0, \"start-server\");");
            builder.AppendLine("adb(0, \"forward\", \"tcp:$hostport\", \"tcp:$deviceport\") and die \"Failed to forward debug port\";");
            builder.AppendLine("adb(1, \"shell\", \"cd $devicedir ; LD_LIBRARY_PATH=\\$PWD ./gdbserver :$deviceport ./$deviceexe\");");
            builder.AppendLine("if (!-d $lib)");
            builder.AppendLine("{");
            builder.AppendLine("    adb(0, \"pull\", \"/system/lib/\", \"$lib/\");");
            builder.AppendLine("    adb(0, \"pull\", \"/system/bin/linker\", \"$lib/linker\");");
            builder.AppendLine("}");
            builder.AppendLine("system($gdb, \"-x\", $gdbinit, $hostexe);");
            builder.AppendLine("adb(0, \"kill-server\");");
            string[] textArray3 = new string[] { "debug.pl" };
            File.WriteAllText(debugDirectory.Combine(textArray3).ToString(), builder.ToString());
        }

        private static NPath GenerateGdbInitFile(NPath debugDirectory, NPath libDirectory, NPath sourceDirectory, int debugPort)
        {
            string[] append = new string[] { "gdbinit" };
            NPath path = debugDirectory.Combine(append);
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("handle SIGPWR SIGXCPU SIG33 nostop noprint");
            builder.AppendLine($"set solib-search-path {libDirectory.ToString(SlashMode.Forward)}");
            builder.AppendLine($"set directories {sourceDirectory.ToString(SlashMode.Forward)}");
            builder.AppendLine($"target remote :{debugPort}");
            File.WriteAllText(path.ToString(), builder.ToString());
            return path;
        }

        public override IEnumerable<string> GetAdditionalIncludes() => 
            new string[] { "cxxabi.h", "dlfcn.h", "setjmp.h", "signal.h", "unwind.h" };

        private static string GetHandleSignal() => 
            "void HandleSignal(int signal, siginfo_t* info, void* context)\n{\n    fprintf(stderr, \"--- CRASH INFO ---\\n\");\n    const char* name;\n    switch (signal)\n    {\n        case SIGSEGV: name = \"SIGSEGV\"; break;\n        case SIGBUS: name = \"SIGBUS\"; break;\n        case SIGFPE: name = \"SIGFPE\"; break;\n        case SIGILL: name = \"SIGILL\"; break;\n        default: name = \"?\"; break;\n    }\n    fprintf(stderr, \"Signal name:   %s\\n\", name);\n    fprintf(stderr, \"Signal number: %d\\n\", info->si_signo);\n    fprintf(stderr, \"Signal code:   %d\\n\", info->si_code);\n    fprintf(stderr, \"Fault address: %p\\n\", info->si_addr);\n    const ucontext_t* ucontext = static_cast<ucontext_t*>(context);\n    #if defined(__arm__)\n    const void* pc = reinterpret_cast<void*>(ucontext->uc_mcontext.arm_pc);\n    #elif defined(__386__)\n    const void* pc = reinterpret_cast<void*>(ucontext->uc_mcontext.gregs[REG_EIP]);\n    #else\n    const void* pc = NULL;\n    #endif\n    fprintf(stderr, \"PC:            %p\\n\", pc);\n    void* buffer[64];\n    const int size = Unwind(buffer, sizeof(buffer) / sizeof(buffer[0]));\n    fprintf(stderr, \"Backtrace:\\n\");\n    int i = 0;\n    for (; (i < size) && (buffer[i] != pc); ++i) ;\n    if (i == size) i = 0;\n    for (int j = 1; i < size; ++i, ++j)\n    {\n        Dl_info dl_info;\n        if (!dladdr(buffer[i], &dl_info))\n            memset(&dl_info, 0, sizeof(dl_info));\n        const char* so = dl_info.dli_fname;\n        if (so)\n        {\n            const char* last = strrchr(so, '/');\n            if (last) so = last + 1;\n        }\n        char* demangledName = NULL;\n        intptr_t offset = 0;\n        if (dl_info.dli_sname)\n        {\n            int status;\n            demangledName = abi::__cxa_demangle(dl_info.dli_sname, NULL, 0, &status);\n            offset = reinterpret_cast<intptr_t>(buffer[i]) - reinterpret_cast<intptr_t>(dl_info.dli_saddr);\n        }\n        fprintf(stderr, \"(%d) %s(%s+0x%x) [%p]\\n\", j, so ? so : \"?\", demangledName ? demangledName : (dl_info.dli_sname ? dl_info.dli_sname :\"?\"), offset, buffer[i]);\n        free(demangledName);\n    }\n    fprintf(stderr, \"--- END OF CRASH INFO ---\\n\");\n    siglongjmp(s_env, 1);\n}\n";

        public override string GetNativeCrashHandler()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(GetUnwindState());
            builder.Append(GetUnwindCallback());
            builder.Append(GetUnwind());
            builder.AppendLine("sigjmp_buf s_env;");
            builder.Append(GetHandleSignal());
            builder.Append(GetSignalHelper());
            return builder.ToString();
        }

        public override string GetNativeCrashHandlerInitialization() => 
            "SignalHelper<SIGSEGV> sigsegv;\nSignalHelper<SIGBUS> sigbus;\nSignalHelper<SIGFPE> sigfpe;\nSignalHelper<SIGILL> sigill;\nif (sigsetjmp(s_env, 1))\n    return 1;\n";

        private static string[] GetNonEmptyLines(string text)
        {
            char[] separator = new char[] { '\n' };
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = l => l.Trim();
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = l => !string.IsNullOrEmpty(l);
            }
            return text.Split(separator).Select<string, string>(<>f__am$cache0).Where<string>(<>f__am$cache1).ToArray<string>();
        }

        private static string GetSignalHelper() => 
            "template <int signal>\nclass SignalHelper\n{\n    struct sigaction _old;\n    bool _installed;\npublic:\n    inline SignalHelper()\n    {\n        struct sigaction action;\n        memset(&action, 0, sizeof(action));\n        action.sa_sigaction = HandleSignal;\n        action.sa_flags = SA_SIGINFO;\n        _installed = (-1 != sigaction(signal, &action, &_old));\n    }\n    inline ~SignalHelper()\n    {\n        if (_installed)\n            sigaction(signal, &_old, NULL);\n    }\n};\n";

        private static string GetSymbolMapSuffix(Unity.IL2CPP.Common.Architecture architecture)
        {
            if (architecture is ARMv7Architecture)
            {
                return "-ARMv7";
            }
            if (!(architecture is x86Architecture))
            {
                throw new ArgumentException($"Unsupported architecture {architecture.Name}.", "architecture");
            }
            return "-x86";
        }

        public override string GetTemporaryDirectoryInitializer() => 
            "il2cpp_set_temp_dir(\"/data/local/tmp/il2cpp-test/tmp\");\n";

        private static string[] GetTokens(string text)
        {
            char[] separator = new char[] { ' ', '\t' };
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = t => t.Trim();
            }
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = t => !string.IsNullOrEmpty(t);
            }
            return text.Split(separator).Select<string, string>(<>f__am$cache2).Where<string>(<>f__am$cache3).ToArray<string>();
        }

        private static string GetUnwind() => 
            "int Unwind(void** buffer, int size)\n{\n    UnwindState state;\n    state.current = buffer;\n    state.end = buffer + size;\n    _Unwind_Backtrace(UnwindCallback, &state);\n    return state.current - buffer;\n}\n";

        private static string GetUnwindCallback() => 
            "_Unwind_Reason_Code UnwindCallback(struct _Unwind_Context* context, void* arg)\n{\n    UnwindState* state = static_cast<UnwindState*>(arg);\n    const uintptr_t ip = _Unwind_GetIP(context);\n    if (ip)\n    {\n        if (state->current != state->end)\n            *state->current++ = reinterpret_cast<void*>(ip);\n        else\n            return _URC_END_OF_STACK;\n    }\n    return _URC_NO_REASON;\n}\n";

        private static string GetUnwindState() => 
            "struct UnwindState\n{\n    void** current;\n    void** end;\n};\n";

        private static void HandleTimeout(AndroidDevice device, string executable)
        {
            string[] nonEmptyLines = GetNonEmptyLines(device.ExecuteShellCommand("ps", true));
            foreach (string str2 in nonEmptyLines)
            {
                if (str2.Contains(executable))
                {
                    string[] tokens = GetTokens(str2);
                    foreach (string str3 in tokens)
                    {
                        int num3;
                        if (int.TryParse(str3, out num3))
                        {
                            device.ExecuteShellCommand($"kill {num3}", true);
                            break;
                        }
                    }
                }
            }
        }

        public override Shell.ExecuteResult RunAndMakeExecuteResult(Unity.IL2CPP.Common.Architecture architecture, string executable) => 
            this.RunAndMakeExecuteResult(architecture, executable, string.Empty);

        private static Shell.ExecuteResult RunAndMakeExecuteResult(Unity.IL2CPP.Common.Architecture architecture, NPath executable, string arguments)
        {
            string str6;
            <RunAndMakeExecuteResult>c__AnonStorey0 storey = new <RunAndMakeExecuteResult>c__AnonStorey0 {
                device = AndroidDevice.GetDefault(architecture)
            };
            AndroidNDKUtilities ndk = new AndroidNDKUtilities(null, architecture);
            storey.device.ExecuteShellCommand($"rm -r "{"/data/local/tmp/il2cpp-test"}"", false);
            storey.device.ExecuteShellCommand($"mkdir -p "{"/data/local/tmp/il2cpp-test"}"", true);
            storey.device.ExecuteShellCommand($"mkdir -p "{"/data/local/tmp/il2cpp-test/tmp"}"", true);
            NPath parent = executable.Parent;
            string[] append = new string[] { "Data" };
            NPath path2 = parent.Combine(append);
            storey.device.ExecuteCommand($"push "{path2}" "{"/data/local/tmp/il2cpp-test/Data"}"", true);
            HashSet<string> set = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { 
                ".cpp",
                ".c",
                ".debug",
                ".h",
                ".map"
            };
            storey.exclude = set;
            IEnumerable<NPath> enumerable = parent.Files(false).Where<NPath>(new Func<NPath, bool>(storey.<>m__0));
            foreach (NPath path3 in enumerable)
            {
                string str = !string.Equals(path3.FileName, "SymbolMap", StringComparison.OrdinalIgnoreCase) ? "/data/local/tmp/il2cpp-test" : "/data/local/tmp/il2cpp-test/Data";
                storey.device.ExecuteCommand($"push "{path3}" "{str + '/' + path3.FileName}"", true);
            }
            string str2 = "/data/local/tmp/il2cpp-test/Data/SymbolMap";
            string symbolMapSuffix = GetSymbolMapSuffix(architecture);
            storey.device.ExecuteShellCommand($"mv "{str2}" "{str2 + symbolMapSuffix}"", true);
            storey.deviceExecutableFileName = executable.FileName;
            string str4 = "/data/local/tmp/il2cpp-test" + '/' + storey.deviceExecutableFileName;
            storey.device.ExecuteShellCommand($"chmod +x "{str4}"", true);
            NPath gdbServer = ndk.GdbServer;
            if (gdbServer.FileExists(""))
            {
                string str5 = "/data/local/tmp/il2cpp-test" + '/' + gdbServer.FileName;
                storey.device.ExecuteCommand($"push "{gdbServer}" "{str5}"", true);
                storey.device.ExecuteShellCommand($"chmod +x "{str5}"", true);
                GenerateDebugScript(executable, "/data/local/tmp/il2cpp-test", storey.deviceExecutableFileName, ndk);
            }
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            using (new Timer(new TimerCallback(storey.<>m__1), null, (int) (RunTimeout.TotalMilliseconds + 0.5), -1))
            {
                string command = $"cd "{"/data/local/tmp/il2cpp-test"}" ; LD_LIBRARY_PATH=$PWD "./{storey.deviceExecutableFileName}" {arguments} 1>{"stdout"} 2>{"stderr"} ; echo $?";
                str6 = storey.device.ExecuteShellCommand(command, true);
            }
            stopwatch.Stop();
            string str8 = storey.device.ExecuteExecOutCommand($"cat "{"/data/local/tmp/il2cpp-test" + '/' + "stdout"}"", true);
            string str9 = storey.device.ExecuteExecOutCommand($"cat "{"/data/local/tmp/il2cpp-test" + '/' + "stderr"}"", true);
            return new Shell.ExecuteResult { 
                StdOut = str8,
                StdErr = str9,
                ExitCode = int.Parse(str6.Trim()),
                Duration = TimeSpan.FromMilliseconds((double) stopwatch.ElapsedMilliseconds)
            };
        }

        public override Shell.ExecuteResult RunAndMakeExecuteResult(Unity.IL2CPP.Common.Architecture architecture, string executable, string arguments)
        {
            bool flag;
            Shell.ExecuteResult result;
            using (Mutex mutex = new Mutex(true, @"Global\AndroidRunnerBuild", out flag))
            {
                if (!flag)
                {
                    mutex.WaitOne();
                }
                try
                {
                    AndroidDebugBridge.StartServer(true);
                    try
                    {
                        result = RunAndMakeExecuteResult(architecture, new NPath(executable), arguments);
                    }
                    finally
                    {
                        AndroidDebugBridge.KillServer();
                    }
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
            return result;
        }

        public override bool SupportsRunningWithParameters =>
            true;

        [CompilerGenerated]
        private sealed class <RunAndMakeExecuteResult>c__AnonStorey0
        {
            internal AndroidDevice device;
            internal string deviceExecutableFileName;
            internal HashSet<string> exclude;

            internal bool <>m__0(NPath f) => 
                !this.exclude.Contains(f.ExtensionWithDot);

            internal void <>m__1(object state)
            {
                AndroidRunner.HandleTimeout(this.device, this.deviceExecutableFileName);
            }
        }
    }
}

