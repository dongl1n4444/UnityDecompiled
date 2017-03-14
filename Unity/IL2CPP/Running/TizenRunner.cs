namespace Unity.IL2CPP.Running
{
    using NiceIO;
    using System;
    using System.IO;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Common;
    using Unity.TinyProfiling;

    internal class TizenRunner : Runner
    {
        private static void MakeSureRunnerIsBuilt()
        {
            using (TinyProfiler.Section("MakeSureRunnerIsBuilt", ""))
            {
                Console.WriteLine("Making sure Tizen runner is built.");
            }
        }

        public override Shell.ExecuteResult RunAndMakeExecuteResult(Unity.IL2CPP.Common.Architecture architecture, string executable) => 
            this.RunAndMakeExecuteResult(architecture, executable, string.Empty);

        public override Shell.ExecuteResult RunAndMakeExecuteResult(Unity.IL2CPP.Common.Architecture architecture, string executable, string arguments)
        {
            Shell.ExecuteArgs executeArgs = new Shell.ExecuteArgs {
                Executable = executable,
                Arguments = arguments,
                WorkingDirectory = Path.GetDirectoryName(executable),
                EnvVars = null
            };
            return Shell.Execute(executeArgs, null);
        }

        private static NPath TizenRunnerExecutablePath
        {
            get
            {
                string[] append = new string[] { "Unity.IL2CPP.Running.Tizen.exe" };
                return CommonPaths.Il2CppRoot.Combine(append);
            }
        }

        private static NPath TizenRunnerProjectFolderPath
        {
            get
            {
                string[] append = new string[] { "Unity.IL2CPP.Running.TizenRunner" };
                return CommonPaths.Il2CppRoot.Combine(append);
            }
        }

        private static NPath TizenRunnerSolutionPath
        {
            get
            {
                string[] append = new string[] { "Unity.IL2CPP.Running.TizenRunner.sln" };
                return TizenRunnerProjectFolderPath.Combine(append);
            }
        }
    }
}

