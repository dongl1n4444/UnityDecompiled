namespace UnityEditor.Android
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using UnityEditor;
    using UnityEditor.Utils;
    using UnityEngine;

    internal class AndroidSDKTools
    {
        private const string COMPATIBLE_KEY_IDENTIFIER = "[compatible]";
        private const string ConsoleMessage = "See the Console for more details. ";
        private const string CreateKeystoreError = "Unable to create keystore. Please make sure the location of the keystore is valid. See the Console for more details. ";
        private const string CreateKeystoreKeyError = "Unable to create key in keystore. Please make sure the location and password of the keystore is correct. See the Console for more details. ";
        private const string InstallPlatformError = "Unable to install additional SDK platform. Please run the SDK Manager manually to make sure you have the latest set of tools and the required platforms installed. See the Console for more details. ";
        private const string KeystoreMessage = "Please make sure the location and password of the keystore is correct. ";
        private const string ListKeystoreKeysError = "Unable to list keys in the keystore. Please make sure the location and password of the keystore is correct. See the Console for more details. ";
        private const string ListPlatformsError = "Unable to list target platforms. Please make sure the android sdk path is correct. See the Console for more details. ";
        private const string MergeManifestsError = "Unable to merge android manifests. See the Console for more details. ";
        private const string ResolvingBuildToolsDir = "Unable to resolve build tools directory. See the Console for more details. ";
        private static AndroidSDKTools s_Instance;
        private string SDKBuildToolsDir;
        private const string SDKManagerMessage = "Please run the SDK Manager manually to make sure you have the latest set of tools and the required platforms installed. ";
        private string SDKPlatformToolsDir;
        public readonly string SDKRootDir;
        private string SDKToolsDir;
        private const string ToolsVersionError = "Unable to determine the tools version of the Android SDK. Please run the SDK Manager manually to make sure you have the latest set of tools and the required platforms installed. See the Console for more details. ";
        private const string UpdateSDKError = "Unable to update the SDK. Please run the SDK Manager manually to make sure you have the latest set of tools and the required platforms installed. See the Console for more details. ";
        private static readonly Regex Warnings = new Regex("^Warning:(.*)$", RegexOptions.Multiline);

        private AndroidSDKTools(string sdkRoot)
        {
            this.SDKRootDir = sdkRoot;
            this.UpdateToolsDirectories();
        }

        private string BuildToolsExe(string command)
        {
            string path = Path.Combine(this.SDKBuildToolsDir, AndroidJavaTools.Exe(command));
            if (File.Exists(path))
            {
                return path;
            }
            return this.PlatformToolsExe(command);
        }

        public string BuildToolsVersion(Command.WaitingForProcessToExit waitingForProcessToExit)
        {
            return AndroidComponentVersion.GetComponentVersion(this.SDKBuildToolsDir);
        }

        public void CreateKey(string keystore, string storepass, string alias, string password, string dname, int validityInDays)
        {
            if (!File.Exists(keystore))
            {
                string[] textArray1 = new string[] { "keytool-createkeystore", keystore, storepass };
                this.RunCommand(textArray1, null, "Unable to create keystore. Please make sure the location of the keystore is valid. See the Console for more details. ");
            }
            string[] sdkToolCommand = new string[] { "keytool-genkey", keystore, storepass, "-keyalias", alias, "-keypass", password, "-validity", validityInDays.ToString(), "-dname", dname };
            this.RunCommand(sdkToolCommand, null, "Unable to create key in keystore. Please make sure the location and password of the keystore is correct. See the Console for more details. ");
        }

        public void DumpDiagnostics()
        {
            Console.WriteLine("AndroidSDKTools:");
            Console.WriteLine("");
            Console.WriteLine("\troot          : {0}", this.SDKRootDir);
            Console.WriteLine("\ttools         : {0}", this.SDKToolsDir);
            Console.WriteLine("\tplatform-tools: {0}", this.SDKPlatformToolsDir);
            Console.WriteLine("\tbuild-tools   : {0}", this.SDKBuildToolsDir);
            Console.WriteLine("");
            Console.WriteLine("\tadb      : {0}", this.ADB);
            Console.WriteLine("\taapt     : {0}", this.AAPT);
            Console.WriteLine("\tzipalign : {0}", this.ZIPALIGN);
            Console.WriteLine("");
        }

        private int ExtractApiLevel(string platform)
        {
            char[] separator = new char[] { '-' };
            string[] strArray = platform.Split(separator);
            if (strArray.Length > 1)
            {
                try
                {
                    return Convert.ToInt32(strArray[1]);
                }
                catch (FormatException)
                {
                }
            }
            return -1;
        }

        public string GetAndroidPlatformPath(int apiLevel)
        {
            string[] components = new string[] { this.SDKRootDir, "platforms", string.Format("android-{0}", apiLevel) };
            string str = Paths.Combine(components);
            if (File.Exists(Path.Combine(str, "android.jar")))
            {
                return str;
            }
            return null;
        }

        public static AndroidSDKTools GetInstance()
        {
            string path = EditorPrefs.GetString("AndroidSdkRoot");
            if (!AndroidSdkRoot.IsSdkDir(path))
            {
                path = AndroidSdkRoot.Browse(path);
                if (!AndroidSdkRoot.IsSdkDir(path))
                {
                    return null;
                }
                EditorPrefs.SetString("AndroidSdkRoot", path);
            }
            if ((s_Instance == null) || (path != s_Instance.SDKRootDir))
            {
                s_Instance = new AndroidSDKTools(path);
            }
            return s_Instance;
        }

        public static AndroidSDKTools GetInstanceOrThrowException()
        {
            AndroidSDKTools instance = GetInstance();
            if (instance == null)
            {
                throw new UnityException("Unable to locate Android SDK!");
            }
            return instance;
        }

        public int GetTopAndroidPlatformAvailable(Command.WaitingForProcessToExit waitingForProcessToExit)
        {
            int num = -1;
            string[] strArray = this.ListTargetPlatforms(waitingForProcessToExit);
            foreach (string str in strArray)
            {
                int apiLevel = this.ExtractApiLevel(str);
                if ((apiLevel > num) && (this.GetAndroidPlatformPath(apiLevel) != null))
                {
                    num = apiLevel;
                }
            }
            return num;
        }

        public string InstallPlatform(int apiLevel, Command.WaitingForProcessToExit waitingForProcessToExit)
        {
            string[] sdkToolCommand = new string[] { "android", "update", "sdk", "-u", "-t", string.Format("android-{0}", apiLevel) };
            return this.RunCommand(sdkToolCommand, waitingForProcessToExit, "Unable to install additional SDK platform. Please run the SDK Manager manually to make sure you have the latest set of tools and the required platforms installed. See the Console for more details. ");
        }

        public string[] ListTargetPlatforms(Command.WaitingForProcessToExit waitingForProcessToExit)
        {
            string[] sdkToolCommand = new string[] { "android", "list", "target", "-c" };
            string str = this.RunCommand(sdkToolCommand, waitingForProcessToExit, "Unable to list target platforms. Please make sure the android sdk path is correct. See the Console for more details. ");
            List<string> list = new List<string>();
            char[] separator = new char[] { '\n', '\r' };
            foreach (string str2 in str.Split(separator))
            {
                if (str2.StartsWith("android-"))
                {
                    list.Add(str2);
                }
            }
            return list.ToArray();
        }

        public void MergeManifests(string target, string mainManifest, string[] libraryManifests, Command.WaitingForProcessToExit waitingForProcessToExit)
        {
            List<string> list = new List<string>();
            string tempFileName = Path.GetTempFileName();
            string[] collection = new string[] { "manifmerger", "merge", "--out", tempFileName, "--main", mainManifest };
            list.AddRange(collection);
            foreach (string str2 in libraryManifests)
            {
                list.Add("--libs");
                list.Add(str2);
            }
            this.RunCommand(list.ToArray(), waitingForProcessToExit, "Unable to merge android manifests. See the Console for more details. ");
            File.Move(tempFileName, target);
        }

        private string PlatformToolsExe(string command)
        {
            string path = Path.Combine(this.SDKPlatformToolsDir, AndroidJavaTools.Exe(command));
            if (File.Exists(path))
            {
                return path;
            }
            return this.ToolsExe(command);
        }

        public string PlatformToolsVersion(Command.WaitingForProcessToExit waitingForProcessToExit)
        {
            return AndroidComponentVersion.GetComponentVersion(this.SDKPlatformToolsDir);
        }

        public string[] ReadAvailableKeys(string keystore, string storepass)
        {
            if ((keystore.Length == 0) || (storepass.Length == 0))
            {
                return null;
            }
            string[] sdkToolCommand = new string[] { "keytool-list", keystore, storepass };
            char[] separator = new char[] { '\r', '\n' };
            string[] strArray2 = this.RunCommand(sdkToolCommand, null, "Unable to list keys in the keystore. Please make sure the location and password of the keystore is correct. See the Console for more details. ").Split(separator, StringSplitOptions.RemoveEmptyEntries);
            List<string> list = new List<string>();
            for (int i = 0; i < strArray2.Length; i++)
            {
                if (strArray2[i].StartsWith("[compatible]"))
                {
                    list.Add(strArray2[i].Substring("[compatible]".Length + 1));
                }
            }
            return list.ToArray();
        }

        public string RunCommand(string[] sdkToolCommand, Command.WaitingForProcessToExit waitingForProcessToExit, string errorMsg)
        {
            return this.RunCommand(sdkToolCommand, null, waitingForProcessToExit, errorMsg);
        }

        public string RunCommand(string[] sdkToolCommand, string workingdir, Command.WaitingForProcessToExit waitingForProcessToExit, string errorMsg)
        {
            return this.RunCommand(sdkToolCommand, 0x800, workingdir, waitingForProcessToExit, errorMsg);
        }

        public string RunCommand(string[] sdkToolCommand, int memoryMB, string workingdir, Command.WaitingForProcessToExit waitingForProcessToExit, string errorMsg)
        {
            return RunCommandSafe(AndroidJavaTools.javaPath, this.SDKToolsDir, sdkToolCommand, memoryMB, workingdir, waitingForProcessToExit, errorMsg);
        }

        private static string RunCommandInternal(string javaExe, string sdkToolsDir, string[] sdkToolCommand, int memoryMB, string workingdir, Command.WaitingForProcessToExit waitingForProcessToExit, string errorMsg)
        {
            <RunCommandInternal>c__AnonStorey0 storey = new <RunCommandInternal>c__AnonStorey0 {
                sdkToolCommand = sdkToolCommand,
                waitingForProcessToExit = waitingForProcessToExit
            };
            if (workingdir == null)
            {
                workingdir = Directory.GetCurrentDirectory();
            }
            string str = "";
            string str2 = str;
            object[] objArray1 = new object[] { str2, "-Xmx", memoryMB, "M " };
            str = string.Concat(objArray1);
            if (sdkToolsDir != null)
            {
                str = str + "-Dcom.android.sdkmanager.toolsdir=\"" + sdkToolsDir + "\" ";
            }
            str = (str + "-Dfile.encoding=UTF8 ") + "-jar \"" + Path.Combine(BuildPipeline.GetBuildToolsDirectory(BuildTarget.Android), "sdktools.jar") + "\" ";
            if (((storey.sdkToolCommand.Length > 1) && (storey.sdkToolCommand[0] == "android")) && (storey.sdkToolCommand[1] == "update"))
            {
                str = str + string.Join(" ", storey.sdkToolCommand);
                storey.sdkToolCommand = new string[] { "y" };
            }
            else
            {
                str = str + "-";
            }
            ProcessStartInfo psi = new ProcessStartInfo {
                FileName = javaExe,
                Arguments = str,
                WorkingDirectory = workingdir,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8
            };
            storey.inputSent = false;
            string input = Command.Run(psi, new Command.WaitingForProcessToExit(storey.<>m__0), errorMsg);
            IEnumerator enumerator = Warnings.Matches(input).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Match current = (Match) enumerator.Current;
                    UnityEngine.Debug.LogWarning(current.Groups[1].Value);
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
            return Warnings.Replace(input, "");
        }

        public static string RunCommandSafe(string javaExe, string sdkToolsDir, string[] sdkToolCommand, int memoryMB, string workingdir, Command.WaitingForProcessToExit waitingForProcessToExit, string errorMsg)
        {
            string str;
            try
            {
                str = RunCommandInternal(javaExe, sdkToolsDir, sdkToolCommand, memoryMB, workingdir, waitingForProcessToExit, errorMsg);
            }
            catch (CommandInvokationFailure failure)
            {
                string str2 = null;
                List<string> list = new List<string>();
                string str3 = failure.StdOutString() + failure.StdErrString();
                if (str3.Contains("Could not reserve enough space for ") || str3.Contains("The specified size exceeds the maximum representable size"))
                {
                    if (memoryMB > 0x40)
                    {
                        return RunCommandSafe(javaExe, sdkToolsDir, sdkToolCommand, memoryMB / 2, workingdir, waitingForProcessToExit, errorMsg);
                    }
                    list.Add("Error: Out of memory (details: unable to create jvm process due to unsufficient continuous memory)");
                }
                foreach (string str4 in failure.StdErr)
                {
                    if (str4.StartsWith("Error:"))
                    {
                        list.Add(str2 = str4);
                    }
                    else if (str2 != null)
                    {
                        str2 = str2 + "\n" + str4;
                    }
                }
                failure.Errors = list.ToArray();
                throw failure;
            }
            return str;
        }

        private string ToolsExe(string command)
        {
            return Path.Combine(this.SDKToolsDir, AndroidJavaTools.Exe(command));
        }

        public string ToolsVersion(Command.WaitingForProcessToExit waitingForProcessToExit)
        {
            return AndroidComponentVersion.GetComponentVersion(this.SDKToolsDir);
        }

        public string UpdateSDK(Command.WaitingForProcessToExit waitingForProcessToExit)
        {
            string str;
            try
            {
                string[] sdkToolCommand = new string[] { "android", "update", "sdk", "-a", "-u", "-t", "tool,platform-tool,build-tools-23.0.2" };
                str = this.RunCommand(sdkToolCommand, waitingForProcessToExit, "Unable to update the SDK. Please run the SDK Manager manually to make sure you have the latest set of tools and the required platforms installed. See the Console for more details. ");
            }
            finally
            {
                this.UpdateToolsDirectories();
            }
            return str;
        }

        public void UpdateToolsDirectories()
        {
            this.SDKToolsDir = Path.Combine(this.SDKRootDir, "tools");
            this.SDKPlatformToolsDir = Path.Combine(this.SDKRootDir, "platform-tools");
            string[] sdkToolCommand = new string[] { "build-tool-dir" };
            this.SDKBuildToolsDir = this.RunCommand(sdkToolCommand, null, "Unable to resolve build tools directory. See the Console for more details. ").Trim();
            if (this.SDKBuildToolsDir.Length == 0)
            {
                this.SDKBuildToolsDir = this.SDKPlatformToolsDir;
            }
        }

        public string AAPT
        {
            get
            {
                return this.BuildToolsExe("aapt");
            }
        }

        public string ADB
        {
            get
            {
                return this.PlatformToolsExe("adb");
            }
        }

        public string ZIPALIGN
        {
            get
            {
                return this.BuildToolsExe("zipalign");
            }
        }

        [CompilerGenerated]
        private sealed class <RunCommandInternal>c__AnonStorey0
        {
            internal bool inputSent;
            internal string[] sdkToolCommand;
            internal Command.WaitingForProcessToExit waitingForProcessToExit;

            internal void <>m__0(Program program)
            {
                if (!this.inputSent && (program != null))
                {
                    StreamWriter writer = new StreamWriter(program.GetStandardInput());
                    foreach (string str in this.sdkToolCommand)
                    {
                        writer.WriteLine(str);
                    }
                    writer.Close();
                    this.inputSent = true;
                }
                if (this.waitingForProcessToExit != null)
                {
                    this.waitingForProcessToExit(program);
                }
            }
        }
    }
}

