namespace Unity.IL2CPP.Running
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Common;

    internal class DesktopRunner : Runner
    {
        private RuntimePlatform _platform;

        public DesktopRunner(RuntimePlatform platform)
        {
            this._platform = platform;
        }

        public override string GetActivateSignature()
        {
            if (this._platform is WindowsDesktopRuntimePlatform)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("int WINAPI wWinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPWSTR lpCmdLine, int nShowCmd)");
                builder.AppendLine("{");
                builder.AppendLine("\tint argc;");
                builder.AppendLine("\twchar_t** argv = CommandLineToArgvW(GetCommandLineW(), &argc);");
                builder.AppendLine("\tint returnValue = EntryPoint(argc, argv);");
                builder.AppendLine("\tLocalFree(argv);");
                builder.AppendLine("\treturn returnValue;");
                builder.AppendLine("}");
                return builder.ToString();
            }
            return base.GetActivateSignature();
        }

        public override IEnumerable<string> GetAdditionalIncludes()
        {
            if (this._platform is WindowsDesktopRuntimePlatform)
            {
                return new string[] { "windows.h" };
            }
            return base.GetAdditionalIncludes();
        }

        public override Shell.ExecuteResult RunAndMakeExecuteResult(string executable)
        {
            return this.RunAndMakeExecuteResult(executable, string.Empty);
        }

        public override Shell.ExecuteResult RunAndMakeExecuteResult(string executable, string arguments)
        {
            Shell.ExecuteArgs executeArgs = new Shell.ExecuteArgs {
                Executable = executable,
                Arguments = arguments,
                WorkingDirectory = Path.GetDirectoryName(executable),
                EnvVars = null
            };
            return Shell.Execute(executeArgs, null);
        }

        public override bool SupportsRunningWithParameters
        {
            get
            {
                return true;
            }
        }
    }
}

