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
            _platformRunners.Add(typeof(MacOSXRuntimePlatform), new DesktopRunner(new MacOSXRuntimePlatform()));
            _platformRunners.Add(typeof(WindowsDesktopRuntimePlatform), new DesktopRunner(new WindowsDesktopRuntimePlatform()));
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

        public virtual string Run(string executable)
        {
            Shell.ExecuteResult result = this.RunAndMakeExecuteResult(executable);
            string str = result.StdErr.Trim() + result.StdOut.Trim();
            if (result.ExitCode != 0)
            {
                throw new ApplicationException(string.Format("Process {0} ended with exitcode {1}{3}{2}{3}", new object[] { executable, result.ExitCode, str, Environment.NewLine }));
            }
            return str;
        }

        public abstract Shell.ExecuteResult RunAndMakeExecuteResult(string executable);
        public virtual Shell.ExecuteResult RunAndMakeExecuteResult(string executable, string arguments)
        {
            if (!string.IsNullOrEmpty(arguments))
            {
                throw new NotSupportedException("Running with arguments in not supported on this platform!");
            }
            return this.RunAndMakeExecuteResult(executable);
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

