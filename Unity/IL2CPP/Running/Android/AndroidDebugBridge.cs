namespace Unity.IL2CPP.Running.Android
{
    using NiceIO;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Common;

    internal static class AndroidDebugBridge
    {
        private static bool _ethernet;
        private static readonly string Executable;

        static AndroidDebugBridge()
        {
            string environmentVariable = Environment.GetEnvironmentVariable("ANDROID_SDK_ROOT");
            if (string.IsNullOrEmpty(environmentVariable))
            {
                throw new Exception("Environment variable ANDROID_SDK_ROOT is not set.");
            }
            string[] append = new string[] { "platform-tools" };
            NPath path = new NPath(environmentVariable).Combine(append);
            string str2 = !PlatformUtils.IsWindows() ? string.Empty : ".exe";
            string[] textArray2 = new string[] { "adb" + str2 };
            Executable = path.Combine(textArray2).ToString();
        }

        public static void Connect()
        {
            string environmentVariable = Environment.GetEnvironmentVariable("ANDROID_DEVICE_CONNECTION");
            _ethernet = !string.IsNullOrEmpty(environmentVariable) && ExecuteCommand($"connect {environmentVariable}", false).Contains("connected");
        }

        public static void Disconnect()
        {
            ExecuteCommand("disconnect", false);
            _ethernet = false;
        }

        public static string ExecuteCommand(string command, bool throwOnError = true)
        {
            Shell.ExecuteArgs executeArgs = new Shell.ExecuteArgs {
                Executable = Executable,
                Arguments = command
            };
            Shell.ExecuteResult result = Shell.Execute(executeArgs, null);
            if (result.ExitCode == 0)
            {
                return result.StdOut;
            }
            if (throwOnError)
            {
                throw new Exception($"ADB command '{command}' failed with error:{Environment.NewLine}{result.StdErr}");
            }
            return result.StdErr;
        }

        public static AndroidDevice[] GetAllDevices()
        {
            string str = null;
            for (int i = 3; i > 0; i--)
            {
                StartServer(true);
                string text = ExecuteCommand("devices", false);
                if (!text.Contains("error"))
                {
                    str = null;
                    List<AndroidDevice> list = new List<AndroidDevice>();
                    foreach (string str3 in GetNonEmptyLines(text))
                    {
                        char[] separator = new char[] { ' ', '\t' };
                        string[] strArray2 = str3.Trim().Split(separator);
                        if (strArray2.Length >= 2)
                        {
                            string str4 = strArray2[0];
                            string str5 = strArray2[strArray2.Length - 1];
                            if (str5 != "device")
                            {
                                if ((str5 == "unauthorized") && (str == null))
                                {
                                    str = $"Android device '{str4}' is unauthorized.";
                                }
                            }
                            else
                            {
                                list.Add(new AndroidDevice(str4));
                            }
                        }
                    }
                    if (list.Count > 0)
                    {
                        return list.ToArray();
                    }
                    if (str == null)
                    {
                        str = "No Android devices connected.";
                    }
                }
                if (_ethernet && (i == 3))
                {
                    Disconnect();
                }
                else
                {
                    KillServer();
                }
                if (i > 0)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(5.0));
                }
            }
            if (str == null)
            {
            }
            throw new Exception("ADB failed to retrieve Android device list.");
        }

        public static string[] GetNonEmptyLines(string text)
        {
            List<string> list = new List<string>();
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                char ch = text[i];
                if ((ch != '\r') && (ch != '\n'))
                {
                    builder.Append(ch);
                }
                else
                {
                    if (((ch == '\r') && ((i + 1) < text.Length)) && (text[i + 1] == '\n'))
                    {
                        i++;
                    }
                    if (builder.Length != 0)
                    {
                        list.Add(builder.ToString());
                        builder.Clear();
                    }
                }
            }
            if (builder.Length > 0)
            {
                list.Add(builder.ToString());
            }
            return list.ToArray();
        }

        public static void KillServer()
        {
            if (_ethernet)
            {
                Disconnect();
            }
            ExecuteCommand("kill-server", false);
        }

        public static void StartServer(bool ethernet = true)
        {
            ExecuteCommand("start-server", true);
            if (ethernet)
            {
                Connect();
            }
        }
    }
}

