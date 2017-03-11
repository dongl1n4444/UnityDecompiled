namespace UnityEngine.TestTools.Utils
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;

    internal static class StackTraceFilter
    {
        private static string[] s_FilteredLogMessages = new string[] { "UnityEngine.DebugLogHandler:Internal_Log", "UnityEngine.DebugLogHandler:Log", "UnityEngine.Logger:Log", "UnityEngine.Debug" };
        private static string[] s_LastMessages = new string[] { "System.Reflection.MonoMethod:InternalInvoke(Object, Object[], Exception&)", "UnityEditor.TestTools.TestRunner.EditModeRunner:InvokeDelegator" };

        public static string Filter(string inputStackTrace)
        {
            foreach (string str in s_LastMessages)
            {
                int index = inputStackTrace.IndexOf(str);
                if (index != -1)
                {
                    inputStackTrace = inputStackTrace.Substring(0, index);
                }
            }
            char[] separator = new char[] { '\n' };
            string[] strArray2 = inputStackTrace.Split(separator);
            StringBuilder builder = new StringBuilder();
            string[] strArray3 = strArray2;
            for (int i = 0; i < strArray3.Length; i++)
            {
                <Filter>c__AnonStorey0 storey = new <Filter>c__AnonStorey0 {
                    line = strArray3[i]
                };
                if (!Enumerable.Any<string>(s_FilteredLogMessages, new Func<string, bool>(storey.<>m__0)))
                {
                    builder.AppendLine(storey.line);
                }
            }
            return builder.ToString();
        }

        [CompilerGenerated]
        private sealed class <Filter>c__AnonStorey0
        {
            internal string line;

            internal bool <>m__0(string s) => 
                this.line.StartsWith(s);
        }
    }
}

