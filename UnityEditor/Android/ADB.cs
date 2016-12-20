namespace UnityEditor.Android
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net;
    using System.Text;
    using System.Threading;
    using UnityEngine;

    internal class ADB
    {
        public static readonly IPAddress Address = IPAddress.Loopback;
        private const string ConsoleMessage = "See the Console for more details. ";
        private const string DevicesError = "Unable to list connected devices. Please make sure the Android SDK is installed and is properly configured in the Editor. See the Console for more details. ";
        private const string KillServerError = "Unable to kill the adb server. Please make sure the Android SDK is installed and is properly configured in the Editor. See the Console for more details. ";
        public static readonly int Port = 0x13ad;
        private const string SDKMessage = "Please make sure the Android SDK is installed and is properly configured in the Editor. ";
        private const string StartServerError = "Unable to start the adb server. Please make sure the Android SDK is installed and is properly configured in the Editor. See the Console for more details. ";

        public static List<string> Devices(Command.WaitingForProcessToExit waitingForProcessToExit)
        {
            int num = 2;
            string str = "";
            List<string> list = new List<string>();
            while (true)
            {
                string[] command = new string[] { "devices" };
                str = Run(command, waitingForProcessToExit, "Unable to list connected devices. Please make sure the Android SDK is installed and is properly configured in the Editor. See the Console for more details. ");
                if (str.Contains("error"))
                {
                    string[] textArray2 = new string[] { "devices" };
                    str = Run(textArray2, waitingForProcessToExit, "Unable to list connected devices. Please make sure the Android SDK is installed and is properly configured in the Editor. See the Console for more details. ");
                }
                char[] separator = new char[] { '\r', '\n' };
                foreach (string str2 in str.Split(separator))
                {
                    if (str2.Trim().EndsWith("device"))
                    {
                        list.Add(str2.Substring(0, str2.IndexOf('\t')));
                    }
                }
                if (list.Count > 0)
                {
                    return list;
                }
                KillServer(waitingForProcessToExit);
                if (--num == 0)
                {
                    break;
                }
                Console.WriteLine(string.Format("ADB - No device found, will retry {0} time(s).", num));
            }
            UnityEngine.Debug.LogWarning("ADB - No device found - output:\n" + str);
            return list;
        }

        public static void KillServer(Command.WaitingForProcessToExit waitingForProcessToExit)
        {
            string[] command = new string[] { "kill-server" };
            RunInternal(command, waitingForProcessToExit, "Unable to kill the adb server. Please make sure the Android SDK is installed and is properly configured in the Editor. See the Console for more details. ");
            for (int i = 0; i < 50; i++)
            {
                if (waitingForProcessToExit != null)
                {
                    waitingForProcessToExit(null);
                }
                Thread.Sleep(100);
            }
        }

        public static string Run(string[] command, Command.WaitingForProcessToExit waitingForProcessToExit, string errorMsg)
        {
            StartServer(waitingForProcessToExit);
            return RunInternal(command, waitingForProcessToExit, errorMsg);
        }

        private static string RunInternal(string[] command, Command.WaitingForProcessToExit waitingForProcessToExit, string errorMsg)
        {
            AndroidSDKTools instance = AndroidSDKTools.GetInstance();
            ProcessStartInfo psi = new ProcessStartInfo {
                FileName = instance.ADB,
                Arguments = string.Join(" ", command),
                WorkingDirectory = instance.SDKRootDir,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8
            };
            return Command.Run(psi, waitingForProcessToExit, errorMsg);
        }

        public static void StartServer(Command.WaitingForProcessToExit waitingForProcessToExit)
        {
            AndroidSDKTools instance = AndroidSDKTools.GetInstance();
            Process process2 = new Process {
                StartInfo = { 
                    FileName = instance.ADB,
                    Arguments = "start-server",
                    WorkingDirectory = instance.SDKRootDir,
                    CreateNoWindow = true,
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };
            using (Process process = process2)
            {
                process.Start();
                do
                {
                    if (waitingForProcessToExit != null)
                    {
                        waitingForProcessToExit(null);
                    }
                }
                while (!process.WaitForExit(100));
            }
        }
    }
}

