namespace UnityEditor.Tizen
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Threading;
    using UnityEditor;
    using UnityEditor.Utils;
    using UnityEngine;

    internal class TizenUtilities
    {
        private const string ConsoleMessage = "See the Console for more details. ";
        private const string DevicesError = "Unable to list connected devices. Please make sure the Tizen SDK is installed and is properly configured in the Editor. See the Console for more details. ";
        internal static string emcli_exe;
        private const string KillServerError = "Unable to kill the sdb server. Please make sure the Tizen SDK is installed and is properly configured in the Editor. See the Console for more details. ";
        internal static string sdb_exe;
        internal static string sdk_tools;
        private const string SDKMessage = "Please make sure the Tizen SDK is installed and is properly configured in the Editor. ";
        private const string StartServerError = "Unable to start the sdb server. Please make sure the Tizen SDK is installed and is properly configured in the Editor. See the Console for more details. ";

        public static List<TizenDevice> AssertAnyDeviceReady(Command.WaitingForProcessToExit waitingForProcessToExit)
        {
            List<TizenDevice> devices = GetDevices(waitingForProcessToExit);
            if (devices.Count == 0)
            {
                throw new ApplicationException("No connected Tizen devices found! Please connect a device and try deploying again.");
            }
            return devices;
        }

        public static bool CreateTpkPackage(string stagingArea)
        {
            string args = " package -- \"" + Path.GetFullPath("Temp/StagingArea/build") + "\" -t tpk -s " + PlayerSettings.Tizen.signingProfileName;
            return IsValidResponse(ExecuteSystemProcess(GetTizenShellPath(), args, "/"), true);
        }

        private static string ExecuteSystemProcess(string command, string args, string workingdir)
        {
            return ExecuteSystemProcess(command, args, workingdir, false, "", "");
        }

        private static string ExecuteSystemProcess(string command, string args, string workingdir, bool displayProgress, string progressTitle, string progressInfo)
        {
            if (displayProgress)
            {
                EditorUtility.DisplayCancelableProgressBar(progressTitle, progressInfo, 0.25f);
            }
            ProcessStartInfo si = new ProcessStartInfo {
                FileName = command,
                Arguments = args,
                WorkingDirectory = workingdir,
                CreateNoWindow = true,
                UseShellExecute = true
            };
            Program program = new Program(si);
            program.Start();
            while (!program.WaitForExit(100))
            {
            }
            string str = StringConcat(program.GetStandardOutput()) + StringConcat(program.GetErrorOutput());
            program.Dispose();
            UnityEngine.Debug.Log(command + " " + args + "\n" + str);
            if (displayProgress)
            {
                EditorUtility.ClearProgressBar();
            }
            return str;
        }

        public static void ForwardPort(string srcPort, string destPort)
        {
            if (PlayerSettings.Tizen.deploymentTargetType == 0)
            {
                string args = " forward tcp:" + srcPort + " tcp:" + destPort;
                ExecuteSystemProcess(sdb_exe, args, "/");
            }
        }

        private static List<TizenDevice> GetDevices(Command.WaitingForProcessToExit waitingForProcessToExit)
        {
            int num = 2;
            string str = "";
            List<TizenDevice> list = new List<TizenDevice>();
            while (true)
            {
                string[] command = new string[] { "devices" };
                str = Run(command, waitingForProcessToExit, "Unable to list connected devices. Please make sure the Tizen SDK is installed and is properly configured in the Editor. See the Console for more details. ");
                if (str.Contains("error"))
                {
                    string[] textArray2 = new string[] { "devices" };
                    str = Run(textArray2, waitingForProcessToExit, "Unable to list connected devices. Please make sure the Tizen SDK is installed and is properly configured in the Editor. See the Console for more details. ");
                }
                char[] separator = new char[] { '\r', '\n' };
                foreach (string str2 in str.Split(separator))
                {
                    if (!str2.Trim().EndsWith("attached") && (str2.Length > 0))
                    {
                        char[] chArray2 = new char[] { ' ', '\t' };
                        list.Add(new TizenDevice(str2.Split(chArray2)[0]));
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
                Console.WriteLine(string.Format("SDB - No device found, will retry {0} time(s).", num));
            }
            UnityEngine.Debug.LogWarning("SDB - No device found - output:\n" + str);
            return list;
        }

        public static string GetTizenShellPath()
        {
            string str = Path.Combine(sdk_tools, "ide/bin/tizen");
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                return (str + ".bat");
            }
            return (str + ".sh");
        }

        private static string GetValidVersionString()
        {
            char[] chArray2;
            string bundleVersion = PlayerSettings.bundleVersion;
            char[] separator = new char[] { '.' };
            for (string[] strArray = bundleVersion.Split(separator); strArray.Length <= 2; strArray = bundleVersion.Split(chArray2))
            {
                bundleVersion = bundleVersion + ".0";
                chArray2 = new char[] { '.' };
            }
            return bundleVersion;
        }

        public static bool InstallTpkPackage(string packageFile)
        {
            string args = "";
            if (PlayerSettings.Tizen.deploymentTargetType == 1)
            {
                args = " -e install \"" + Path.GetFullPath("Temp/StagingArea/build/") + packageFile + "\"";
            }
            else
            {
                string[] textArray1 = new string[] { " -s ", PlayerSettings.Tizen.deploymentTarget, " install \"", Path.GetFullPath("Temp/StagingArea/build/"), packageFile, "\"" };
                args = string.Concat(textArray1);
            }
            return IsValidResponse(ExecuteSystemProcess(sdb_exe, args, "/"), true);
        }

        internal static bool IsMobileDevice(string model)
        {
            return model.Contains("SM-");
        }

        private static bool IsValidResponse(string output, bool acceptNoError)
        {
            if (((acceptNoError && !output.Contains("error")) && (!output.Contains("Error") && !output.Contains("Exception"))) && !output.Contains("fail"))
            {
                return true;
            }
            UnityEngine.Debug.LogError(output);
            return false;
        }

        public static void KillServer(Command.WaitingForProcessToExit waitingForProcessToExit)
        {
            string[] command = new string[] { "kill-server" };
            RunInternal(command, waitingForProcessToExit, "Unable to kill the sdb server. Please make sure the Tizen SDK is installed and is properly configured in the Editor. See the Console for more details. ");
            for (int i = 0; i < 50; i++)
            {
                waitingForProcessToExit(null);
                Thread.Sleep(100);
            }
        }

        public static bool LaunchTpkPackage(string id, string stagingArea)
        {
            if (PlayerSettings.Tizen.deploymentTargetType == 0)
            {
                return IsValidResponse(ExecuteSystemProcess(GetTizenShellPath(), "run -s " + PlayerSettings.Tizen.deploymentTarget + " -p " + id, "/"), true);
            }
            return ((PlayerSettings.Tizen.deploymentTargetType == 1) && IsValidResponse(ExecuteSystemProcess(GetTizenShellPath(), "run -p " + id, "/"), true));
        }

        public static string[] ListDevices()
        {
            string args = " devices";
            string str2 = ExecuteSystemProcess(sdb_exe, args, "/");
            List<string> list = new List<string>();
            char[] separator = new char[] { '\n', '\r' };
            string[] strArray = str2.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            foreach (string str3 in strArray)
            {
                if (!str3.Trim().EndsWith("attached") && (str3.Length > 0))
                {
                    string[] strArray3 = str3.Split((string[]) null, StringSplitOptions.RemoveEmptyEntries);
                    if (IsMobileDevice(strArray3[2]))
                    {
                        list.Add("Model: " + strArray3[2] + " DUID: " + strArray3[0]);
                    }
                }
            }
            return list.ToArray();
        }

        public static string[] ListEmulators()
        {
            string args = " list-vm";
            string str2 = ExecuteSystemProcess(emcli_exe, args, "/");
            List<string> list = new List<string>();
            char[] separator = new char[] { '\n', '\r' };
            string[] strArray = str2.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            foreach (string str3 in strArray)
            {
                list.Add(str3);
            }
            return list.ToArray();
        }

        public static void PrepareToolPaths()
        {
            string str = EditorPrefs.GetString("TizenSdkRoot");
            while (string.IsNullOrEmpty(str) || !TizenSdkRoot.IsSdkDir(str))
            {
                str = TizenSdkRoot.Browse(str);
            }
            EditorPrefs.SetString("TizenSdkRoot", str);
            sdk_tools = Path.Combine(str, "tools");
            sdb_exe = "sdb";
            sdb_exe = Path.Combine(sdk_tools, sdb_exe);
            emcli_exe = "em-cli";
            emcli_exe = Path.Combine(sdk_tools, Path.Combine("emulator", Path.Combine("bin", emcli_exe)));
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                sdb_exe = sdb_exe + ".exe";
                emcli_exe = emcli_exe + ".bat";
            }
        }

        public static string Run(string[] command, Command.WaitingForProcessToExit waitingForProcessToExit, string errorMsg)
        {
            StartServer(waitingForProcessToExit);
            return RunInternal(command, waitingForProcessToExit, errorMsg);
        }

        private static string RunInternal(string[] command, Command.WaitingForProcessToExit waitingForProcessToExit, string errorMsg)
        {
            ProcessStartInfo psi = new ProcessStartInfo {
                FileName = sdb_exe,
                Arguments = string.Join(" ", command),
                WorkingDirectory = sdk_tools,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8
            };
            return Command.Run(psi, waitingForProcessToExit, errorMsg);
        }

        public static void ShowErrDlgAndThrow(string title, string message)
        {
            ShowErrDlgAndThrow(title, message, new UnityException(title + "\n" + message));
        }

        public static void ShowErrDlgAndThrow(string title, string message, Exception ex)
        {
            EditorUtility.DisplayDialog(title, message, "Ok");
            throw ex;
        }

        public static void StartServer(Command.WaitingForProcessToExit waitingForProcessToExit)
        {
            Process process2 = new Process {
                StartInfo = { 
                    FileName = sdb_exe,
                    Arguments = "start-server",
                    WorkingDirectory = sdk_tools,
                    CreateNoWindow = true,
                    UseShellExecute = Application.platform == RuntimePlatform.WindowsEditor
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

        private static string StringConcat(IEnumerable<string> strings)
        {
            StringBuilder builder = new StringBuilder("");
            foreach (string str in strings)
            {
                builder.Append(str + Environment.NewLine);
            }
            return builder.ToString();
        }

        public static string StringFromMinOSVersion()
        {
            int minOSVersion = (int) PlayerSettings.Tizen.minOSVersion;
            if ((minOSVersion == 0) || (minOSVersion != 1))
            {
                return "2.3";
            }
            return "2.4";
        }

        public static bool ValidateSigningProfile(string stagingArea)
        {
            if (string.IsNullOrEmpty(PlayerSettings.Tizen.signingProfileName))
            {
                ShowErrDlgAndThrow("Signing failure!", "Please enter your signing profile name in Tizen Player Settings.");
            }
            string args = " cli-config -l";
            char[] separator = new char[] { '\n', '\r' };
            string[] strArray = ExecuteSystemProcess(GetTizenShellPath(), args, "/").Split(separator);
            foreach (string str3 in strArray)
            {
                if (str3.Contains("default.profiles.path"))
                {
                    char[] chArray2 = new char[] { '=' };
                    string[] strArray3 = str3.Split(chArray2);
                    if (!File.Exists(strArray3[1]))
                    {
                        ShowErrDlgAndThrow("Signing failure!", "Profiles path in Tizen CLI configuration does not exist.");
                    }
                    if (!File.ReadAllText(strArray3[1]).Contains(PlayerSettings.Tizen.signingProfileName))
                    {
                        ShowErrDlgAndThrow("Signing failure!", "Signing profile name does not exist. Check the spelling or create a new profile using the Tizen IDE.");
                    }
                    return true;
                }
            }
            ShowErrDlgAndThrow("Signing failure!", "Default profile path is not set in the Tizen CLI configuration.");
            return false;
        }

        public static void WriteCapabilitiesToManifest(TextWriter writer)
        {
            PlayerSettings.TizenCapability[] values = (PlayerSettings.TizenCapability[]) Enum.GetValues(typeof(PlayerSettings.TizenCapability));
            foreach (PlayerSettings.TizenCapability capability in values)
            {
                if (PlayerSettings.Tizen.GetCapability(capability))
                {
                    switch (capability)
                    {
                        case PlayerSettings.TizenCapability.Location:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/location</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.DataSharing:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/datasharing</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.NetworkGet:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/network.get</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.WifiDirect:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/wifidirect</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.CallHistoryRead:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/callhistory.read</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.Power:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/power</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.ContactWrite:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/contact.write</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.MessageWrite:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/message.write</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.ContentWrite:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/content.write</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.Push:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/push</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.AccountRead:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/account.read</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.ExternalStorage:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/externalstorage</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.Recorder:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/recorder</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.PackageManagerInfo:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/packagemanager.info</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.NFCCardEmulation:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/nfc.cardemulation</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.CalendarWrite:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/calendar.write</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.WindowPrioritySet:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/window.priority.set</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.VolumeSet:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/volume.set</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.CallHistoryWrite:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/callhistory.write</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.AlarmSet:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/alarm.set</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.Call:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/call</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.Email:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/email</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.ContactRead:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/contact.read</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.Shortcut:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/shortcut</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.KeyManager:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/keymanager</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.LED:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/led</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.NetworkProfile:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/network.profile</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.AlarmGet:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/alarm.get</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.Display:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/display</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.CalendarRead:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/calendar.read</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.NFC:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/nfc</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.AccountWrite:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/account.write</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.Bluetooth:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/bluetooth</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.Notification:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/notification</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.NetworkSet:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/network.set</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.ExternalStorageAppData:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/externalstorage.appdata</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.Download:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/download</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.Telephony:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/telephony</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.MessageRead:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/message.read</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.MediaStorage:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/mediastorage</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.Internet:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/internet</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.Camera:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/camera</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.Haptic:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/haptic</privilege>");
                            break;

                        case PlayerSettings.TizenCapability.AppManagerLaunch:
                            writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/appmanager.launch</privilege>");
                            break;
                    }
                }
            }
        }

        public enum DeploymentTargetType
        {
            All = 2,
            Emulator = 1,
            Mobile = 0,
            None = -1
        }
    }
}

