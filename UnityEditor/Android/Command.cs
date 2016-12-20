namespace UnityEditor.Android
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Text;
    using UnityEditor.Utils;

    internal class Command
    {
        public static string Run(ProcessStartInfo psi, WaitingForProcessToExit waitingForProcessToExit, string errorMsg)
        {
            psi.EnvironmentVariables.Remove("_JAVA_OPTIONS");
            psi.EnvironmentVariables.Remove("JAVA_TOOL_OPTIONS");
            psi.EnvironmentVariables.Remove("CLASSPATH");
            using (Program program = new Program(psi))
            {
                program.Start();
                do
                {
                    if (waitingForProcessToExit != null)
                    {
                        waitingForProcessToExit(program);
                    }
                }
                while (!program.WaitForExit(100));
                if (program.ExitCode != 0)
                {
                    throw new CommandInvokationFailure(errorMsg, program);
                }
                StringBuilder builder = new StringBuilder("");
                foreach (string str in program.GetStandardOutput())
                {
                    builder.Append(str + Environment.NewLine);
                }
                foreach (string str2 in program.GetErrorOutput())
                {
                    builder.Append(str2 + Environment.NewLine);
                }
                return builder.ToString();
            }
        }

        public delegate void WaitingForProcessToExit(Program program);
    }
}

