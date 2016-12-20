namespace UnityEditor.Android
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using UnityEditor;
    using UnityEditor.Utils;

    internal class AndroidDevice
    {
        private const string ConsoleMessage = "See the Console for more details. ";
        private const string ExternalStorageRootError = "Unable to determine external storagePlease make sure the Android SDK is installed and is properly configured in the Editor. See the Console for more details. ";
        private const string FeaturesError = "Unable to retrieve device features information. Please make sure the Android SDK is installed and is properly configured in the Editor. See the Console for more details. ";
        private const string ForwardError = "Unable to forward network traffic to device. Please make sure the Android SDK is installed and is properly configured in the Editor. See the Console for more details. ";
        private const string GetSDCardError = "Unable to obtain sd card directory. Please make sure the Android SDK is installed and is properly configured in the Editor. See the Console for more details. ";
        private const string InstallError = "Unable to install APK to device. Please make sure the Android SDK is installed and is properly configured in the Editor. See the Console for more details. ";
        private const string LaunchError = "Unable to launch application. Please make sure the Android SDK is installed and is properly configured in the Editor. See the Console for more details. ";
        private readonly string m_DeviceId;
        private string m_ExternalStorage;
        private List<string> m_Features;
        private readonly string m_ProductModel;
        private PropertiesTable<string> m_Properties;
        private const string MakePathError = "Unable to create directory on phone. Please make sure the Android SDK is installed and is properly configured in the Editor. See the Console for more details. ";
        private const string MemInfoError = "Unable to retrieve /proc/meminfo from device. Please make sure the Android SDK is installed and is properly configured in the Editor. See the Console for more details. ";
        private const string PropertiesError = "Unable to retrieve device properties. Please make sure the Android SDK is installed and is properly configured in the Editor. See the Console for more details. ";
        private const string PullError = "Unable to pull remote file from device. Please make sure the Android SDK is installed and is properly configured in the Editor. See the Console for more details. ";
        private const string PushError = "Unable to push local file to device. Please make sure the Android SDK is installed and is properly configured in the Editor. See the Console for more details. ";
        private const string SDKMessage = "Please make sure the Android SDK is installed and is properly configured in the Editor. ";
        private const string SetPropertyError = "Unable to set device property. Please make sure the Android SDK is installed and is properly configured in the Editor. See the Console for more details. ";
        private const string UninstallError = "Unable to uninstall APK from device. Please make sure the Android SDK is installed and is properly configured in the Editor. See the Console for more details. ";

        public AndroidDevice(string deviceId)
        {
            this.m_DeviceId = deviceId;
            this.m_ProductModel = this.Properties["ro.product.model"];
        }

        public string Describe()
        {
            return string.Format("{0} [{1}]", this.Id, this.ProductModel);
        }

        private string Exec(string[] command, Command.WaitingForProcessToExit waitingForProcessToExit, string errorMsg)
        {
            <Exec>c__AnonStorey0 storey = new <Exec>c__AnonStorey0 {
                waitingForProcessToExit = waitingForProcessToExit,
                $this = this
            };
            try
            {
                string[] array = new string[2 + command.Length];
                string str = string.Format("\"{0}\"", this.m_DeviceId);
                new string[] { "-s", str }.CopyTo(array, 0);
                command.CopyTo(array, 2);
                return ADB.Run(array, new Command.WaitingForProcessToExit(storey.<>m__0), errorMsg);
            }
            catch (RetryInvokationException)
            {
                return this.Exec(command, storey.waitingForProcessToExit, errorMsg);
            }
        }

        public string Forward(string pc, string device, Command.WaitingForProcessToExit waitingForProcessToExit)
        {
            string[] command = new string[] { "forward", Quote(pc), Quote(device) };
            return this.Exec(command, waitingForProcessToExit, "Unable to forward network traffic to device. Please make sure the Android SDK is installed and is properly configured in the Editor. See the Console for more details. ");
        }

        public string GetSDCardPath(Command.WaitingForProcessToExit waitingForProcessToExit)
        {
            string[] command = new string[] { "shell", "echo", "$EXTERNAL_STORAGE" };
            return this.Exec(command, waitingForProcessToExit, "Unable to obtain sd card directory. Please make sure the Android SDK is installed and is properly configured in the Editor. See the Console for more details. ");
        }

        public string Install(string apkfile, Command.WaitingForProcessToExit waitingForProcessToExit)
        {
            string[] command = new string[] { "install", "-r", Quote(Path.GetFullPath(apkfile)) };
            return this.Exec(command, waitingForProcessToExit, "Unable to install APK to device. Please make sure the Android SDK is installed and is properly configured in the Editor. See the Console for more details. ");
        }

        public string Launch(string package, string activity, Command.WaitingForProcessToExit waitingForProcessToExit)
        {
            string[] command = new string[] { "shell", "am", "start", "-a", "android.intent.action.MAIN", "-c", "android.intent.category.LAUNCHER", "-f", "0x10200000", "-n", Quote(string.Format("{0}/{1}", package, activity)) };
            return this.Exec(command, waitingForProcessToExit, "Unable to launch application. Please make sure the Android SDK is installed and is properly configured in the Editor. See the Console for more details. ");
        }

        public string MakePath(string path, Command.WaitingForProcessToExit waitingForProcessToExit)
        {
            string[] command = new string[] { "shell", "mkdir", "-p", Quote(path) };
            return this.Exec(command, waitingForProcessToExit, "Unable to create directory on phone. Please make sure the Android SDK is installed and is properly configured in the Editor. See the Console for more details. ");
        }

        public string Pull(string src, string dst, Command.WaitingForProcessToExit waitingForProcessToExit)
        {
            string[] command = new string[] { "pull", Quote(src), Quote(Path.GetFullPath(dst)) };
            return this.Exec(command, waitingForProcessToExit, "Unable to pull remote file from device. Please make sure the Android SDK is installed and is properly configured in the Editor. See the Console for more details. ");
        }

        public string Push(string src, string dst, Command.WaitingForProcessToExit waitingForProcessToExit)
        {
            string[] command = new string[] { "push", Quote(Path.GetFullPath(src)), Quote(dst) };
            return this.Exec(command, waitingForProcessToExit, "Unable to push local file to device. Please make sure the Android SDK is installed and is properly configured in the Editor. See the Console for more details. ");
        }

        private static string Quote(string val)
        {
            return string.Format("\"{0}\"", val);
        }

        public string SetProperty(string key, string val, Command.WaitingForProcessToExit waitingForProcessToExit)
        {
            this.m_Properties = null;
            string[] command = new string[] { "shell", "setprop", Quote(key), Quote(val) };
            return this.Exec(command, waitingForProcessToExit, "Unable to set device property. Please make sure the Android SDK is installed and is properly configured in the Editor. See the Console for more details. ");
        }

        public string Uninstall(string package, Command.WaitingForProcessToExit waitingForProcessToExit)
        {
            string[] command = new string[] { "uninstall", Quote(package) };
            return this.Exec(command, waitingForProcessToExit, "Unable to uninstall APK from device. Please make sure the Android SDK is installed and is properly configured in the Editor. See the Console for more details. ");
        }

        public string ExternalStorageRoot
        {
            get
            {
                if (this.m_ExternalStorage != null)
                {
                    return this.m_ExternalStorage;
                }
                Regex regex = new Regex(@"^\s*(.+?)\s*$", RegexOptions.Multiline);
                string[] command = new string[] { "shell", "echo", "$EXTERNAL_STORAGE" };
                Match match = regex.Match(this.Exec(command, null, "Unable to determine external storagePlease make sure the Android SDK is installed and is properly configured in the Editor. See the Console for more details. "));
                return (this.m_ExternalStorage = match.Groups[1].Value);
            }
        }

        public List<string> Features
        {
            get
            {
                if (this.m_Features != null)
                {
                    return this.m_Features;
                }
                List<string> list2 = new List<string>();
                Regex regex = new Regex(@"^\s*(\w.*?)\s*$", RegexOptions.Multiline);
                string[] command = new string[] { "shell", "dumpsys", "package", "f" };
                IEnumerator enumerator = regex.Matches(this.Exec(command, null, "Unable to retrieve device features information. Please make sure the Android SDK is installed and is properly configured in the Editor. See the Console for more details. ")).GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        Match current = (Match) enumerator.Current;
                        list2.Add(current.Groups[1].Value);
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
                return (this.m_Features = list2);
            }
        }

        public string Id
        {
            get
            {
                return this.m_DeviceId;
            }
        }

        public PropertiesTable<ulong> MemInfo
        {
            get
            {
                Dictionary<string, ulong> dictionary = new Dictionary<string, ulong>();
                Regex regex = new Regex(@"^(.+?):\s*?(\d+).*$", RegexOptions.Multiline);
                string[] command = new string[] { "shell", "cat", "/proc/meminfo" };
                IEnumerator enumerator = regex.Matches(this.Exec(command, null, "Unable to retrieve /proc/meminfo from device. Please make sure the Android SDK is installed and is properly configured in the Editor. See the Console for more details. ")).GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        Match current = (Match) enumerator.Current;
                        dictionary[current.Groups[1].Value] = ulong.Parse(current.Groups[2].Value) * ((ulong) 0x400L);
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
                return new PropertiesTable<ulong>(dictionary, 0L);
            }
        }

        public string ProductModel
        {
            get
            {
                return this.m_ProductModel;
            }
        }

        public PropertiesTable<string> Properties
        {
            get
            {
                if (this.m_Properties != null)
                {
                    return this.m_Properties;
                }
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                Regex regex = new Regex(@"^\[(.+?)\]:\s*?\[(.*?)\].*$", RegexOptions.Multiline);
                string[] command = new string[] { "shell", "getprop" };
                IEnumerator enumerator = regex.Matches(this.Exec(command, null, "Unable to retrieve device properties. Please make sure the Android SDK is installed and is properly configured in the Editor. See the Console for more details. ")).GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        Match current = (Match) enumerator.Current;
                        dictionary[current.Groups[1].Value] = current.Groups[2].Value;
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
                return (this.m_Properties = new PropertiesTable<string>(dictionary, ""));
            }
        }

        [CompilerGenerated]
        private sealed class <Exec>c__AnonStorey0
        {
            internal AndroidDevice $this;
            internal Command.WaitingForProcessToExit waitingForProcessToExit;

            internal void <>m__0(Program program)
            {
                if (this.waitingForProcessToExit != null)
                {
                    this.waitingForProcessToExit(program);
                }
                if (program != null)
                {
                    foreach (string str in program.GetErrorOutput())
                    {
                        if (str.StartsWith("- waiting for device -"))
                        {
                            if (!EditorUtility.DisplayDialog("Communication lost", this.$this.Describe() + ".\nTry reconnecting the USB cable.", "Retry", "Cancel"))
                            {
                                throw new ProcessAbortedException("Devices connection lost");
                            }
                            throw new AndroidDevice.RetryInvokationException();
                        }
                    }
                }
            }
        }

        public class PropertiesTable<TValue>
        {
            private TValue m_DefaultValue;
            private Dictionary<string, TValue> m_Dictionary;

            internal PropertiesTable(Dictionary<string, TValue> dictionary, TValue defaultValue)
            {
                this.m_Dictionary = dictionary;
                this.m_DefaultValue = defaultValue;
            }

            public Dictionary<string, TValue> Dictionary
            {
                get
                {
                    return this.m_Dictionary;
                }
            }

            public TValue this[string key]
            {
                get
                {
                    TValue local = default(TValue);
                    return (!this.m_Dictionary.TryGetValue(key, out local) ? this.m_DefaultValue : local);
                }
            }
        }

        private class RetryInvokationException : Exception
        {
        }
    }
}

