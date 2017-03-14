namespace Unity.IL2CPP.Running
{
    using System;
    using System.Collections.Generic;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Building.Platforms;
    using Unity.IL2CPP.Common;

    public abstract class Runner
    {
        private static Dictionary<Type, Runner> _platformRunners = new Dictionary<Type, Runner>();

        static Runner()
        {
            _platformRunners.Add(typeof(AndroidRuntimePlatform), new AndroidRunner());
            _platformRunners.Add(typeof(MacOSXRuntimePlatform), new DesktopRunner(new MacOSXRuntimePlatform()));
            _platformRunners.Add(typeof(WindowsDesktopRuntimePlatform), new DesktopRunner(new WindowsDesktopRuntimePlatform()));
            _platformRunners.Add(typeof(LinuxRuntimePlatform), new DesktopRunner(new LinuxRuntimePlatform()));
            _platformRunners.Add(typeof(TizenRuntimePlatform), new TizenRunner());
            _platformRunners.Add(typeof(WinRTRuntimePlatform), new WinRTRunner());
        }

        protected Runner()
        {
        }

        public static Runner For(RuntimePlatform platform)
        {
            Runner runner;
            PlatformSupport.For(platform).RegisterRunner();
            _platformRunners.TryGetValue(platform.GetType(), out runner);
            if (runner == null)
            {
                throw new ArgumentException($"Can't find runner for platform {platform}!");
            }
            return runner;
        }

        public virtual string GetActivateSignature() => 
            "int main(int argc, const char* argv[]) { return EntryPoint(argc, argv); }";

        public virtual IEnumerable<string> GetAdditionalIncludes() => 
            new string[0];

        public virtual string GetNativeCrashHandler() => 
            string.Empty;

        public virtual string GetNativeCrashHandlerInitialization() => 
            string.Empty;

        public virtual string GetTemporaryDirectoryInitializer() => 
            string.Empty;

        public virtual string Run(Unity.IL2CPP.Common.Architecture architecture, string executable)
        {
            Shell.ExecuteResult result = this.RunAndMakeExecuteResult(architecture, executable);
            string str = result.StdErr.Trim();
            string str2 = result.StdOut.Trim();
            string str3 = (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(str2)) ? string.Empty : Environment.NewLine;
            string str4 = str + str3 + str2;
            if (result.ExitCode != 0)
            {
                throw new RunnerFailedException(string.Format("Process {0} ended with exitcode {1}{3}{2}{3}", new object[] { executable, result.ExitCode, str4, Environment.NewLine }));
            }
            return str4;
        }

        public abstract Shell.ExecuteResult RunAndMakeExecuteResult(Unity.IL2CPP.Common.Architecture architecture, string executable);
        public virtual Shell.ExecuteResult RunAndMakeExecuteResult(Unity.IL2CPP.Common.Architecture architecture, string executable, string arguments)
        {
            if (!string.IsNullOrEmpty(arguments))
            {
                throw new NotSupportedException("Running with arguments in not supported on this platform!");
            }
            return this.RunAndMakeExecuteResult(architecture, executable);
        }

        public static void SetPlatformRunner(Type platform, Runner runner)
        {
            if (!_platformRunners.ContainsKey(platform))
            {
                _platformRunners.Add(platform, runner);
            }
        }

        public virtual bool SupportsRunningWithParameters =>
            false;
    }
}

