namespace UnityEditor.iOS
{
    using Mono.Unix;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net.Sockets;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using UnityEditor;
    using UnityEngine;

    public class XcodeController : IDisposable, IXcodeController
    {
        private Socket _socket = null;
        private static string kXcodePluginDestination = (Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Library/Application Support/Developer/Shared/Xcode/Plug-ins/Unity4XC.xcplugin");
        private const string kXcodePluginName = "Unity4XC.xcplugin";
        private const string SocketDomainName = "/tmp/unityxplugin";

        public void BuildProject(string projectPath)
        {
            this.Connect();
            this.SendCommandReceiveResult(this, this.EncodeCommand(Command.BuildProject));
            this.Disconnect();
        }

        public void CleanProject(string projectPath)
        {
            this.RunSimpleCommand(Command.CleanProject);
        }

        public void CloseAllOpenUnityProjects(string buildToolsDirs, string projectPath)
        {
            XCodeUtils.LaunchXCode();
            try
            {
                if (this.XcodeIsRunning())
                {
                    this.InstallXcodePlugin(buildToolsDirs);
                    this.InternalCloseAllOpenUnityProjects();
                }
            }
            catch (Exception exception)
            {
                object[] param = new object[] { exception };
                this.Log("Close all open unity project in Xcode failed. {0}", param);
            }
        }

        private string CommandAsString(Command cmd)
        {
            return Convert.ToString((int) cmd);
        }

        public void Connect()
        {
            this.Log("Connecting to Xcode", new object[0]);
            this._socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);
            this._socket.ReceiveTimeout = 0x493e0;
            this._socket.Connect(new UnixEndPoint("/tmp/unityxplugin"));
            this.Log("Connected", new object[0]);
        }

        public void Disconnect()
        {
            this.Log("Disconnecting", new object[0]);
            if (this._socket != null)
            {
                this._socket.Close();
            }
            this._socket = null;
        }

        public void Dispose()
        {
            this.Disconnect();
        }

        private string[] EncodeCommand(Command cmd)
        {
            return new string[] { this.CommandAsString(cmd) };
        }

        private string[] EncodeCommand(Command cmd, string arg)
        {
            return new string[] { this.CommandAsString(cmd), arg };
        }

        public string GetMobileDeviceList(string projectPath)
        {
            string str = "";
            this.Connect();
            str = this.SendCommandReceiveResult(this, this.EncodeCommand(Command.MobileDeviceList));
            this.Disconnect();
            return str;
        }

        public bool InitializeXcodeApplication(string buildToolsDirs)
        {
            return this.LaunchAndWaitForXcode(buildToolsDirs);
        }

        private void InstallXcodePlugin(string buildToolsDir)
        {
            string path = this.PluginSourcePath(buildToolsDir);
            string str2 = Unsupported.ResolveSymlinks(kXcodePluginDestination);
            bool flag = Directory.Exists(kXcodePluginDestination);
            try
            {
                if (flag && !string.Equals(Path.GetFullPath(str2), Path.GetFullPath(path), StringComparison.InvariantCultureIgnoreCase))
                {
                    this.Log("Plugin points to wrong location. Removing.", new object[0]);
                    Directory.Delete(kXcodePluginDestination);
                    flag = false;
                }
                if (!flag)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(kXcodePluginDestination));
                    Utils.SymlinkFileAbsolute(path, kXcodePluginDestination);
                    XCodeUtils.TerminateXCode();
                }
            }
            catch (Exception exception)
            {
                object[] param = new object[] { exception };
                this.Log("{0}", param);
                UnityEngine.Debug.LogWarning("Could not install the Unity Xcode plugin. Automatic Build & Run won't be supported");
            }
        }

        private void InternalCloseAllOpenUnityProjects()
        {
            this.RunSimpleCommand(Command.CloseAllUnityProjects);
        }

        private bool LaunchAndWaitForXcode(string buildToolsDir)
        {
            this.InstallXcodePlugin(buildToolsDir);
            if (!XCodeUtils.CheckXCodeCompatibleWithPlugin(this.PluginSourcePath(buildToolsDir)))
            {
                UnityEngine.Debug.LogWarning("Unity xcode plugin has not current Xcode in its compatibility list.\n Please launch the project manually");
                return false;
            }
            if (!this.XcodeIsRunning() || !this.WaitForPluginStartup(10))
            {
                this.Log("Terminating Xcode if running", new object[0]);
                XCodeUtils.TerminateXCode();
                this.Log("Launching Xcode", new object[0]);
                XCodeUtils.LaunchXCode();
                this.WaitForPluginStartup(30);
            }
            return true;
        }

        public void Log(string msg, params object[] param)
        {
            Console.WriteLine(msg, param);
        }

        public void OpenProject(string path)
        {
            this.Connect();
            this.SendCommandReceiveResult(this, this.EncodeCommand(Command.OpenProject, path));
            this.SendCommandReceiveResult(this, this.EncodeCommand(Command.WaitForActivitiesFinish));
            this.Disconnect();
        }

        private string PluginSourcePath(string buildToolsDir)
        {
            return Path.Combine(Path.Combine(UnityEditor.BuildPipeline.GetBuildToolsDirectory(EditorUserBuildSettings.activeBuildTarget), Utils.GetOSPathPart()), "Unity4XC.xcplugin");
        }

        private string ReceiveResult()
        {
            List<byte> list = new List<byte>();
            byte[] buffer = new byte[1];
            int num = 0;
            do
            {
                num = this._socket.Receive(buffer);
                list.Add(buffer[0]);
            }
            while ((buffer[0] != 10) && (num > 0));
            string str = Encoding.UTF8.GetString(list.ToArray());
            this.Log(string.Format("Received results: {0}", str), new object[0]);
            return str;
        }

        public void RunProject(string projectPath)
        {
            this.RunSimpleCommand(Command.RunProject);
        }

        private void RunSimpleCommand(string[] cmd)
        {
            this.Connect();
            this.SendCommandReceiveResult(this, cmd);
            this.Disconnect();
        }

        private void RunSimpleCommand(Command cmd)
        {
            this.RunSimpleCommand(this.EncodeCommand(cmd));
        }

        public void SelectDeviceScheme(string projectPath)
        {
            this.RunSimpleCommand(Command.SelectDeviceScheme);
        }

        public void SelectSimulatorScheme(string version)
        {
            this.RunSimpleCommand(this.EncodeCommand(Command.SelectSimulatorScheme, version));
        }

        private void SendCommand(string[] cmd)
        {
            string str = string.Join("\t", cmd) + "\n";
            this.Log(string.Format("Sending command {0}", str), new object[0]);
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            this._socket.Send(bytes);
        }

        private string SendCommandReceiveResult(XcodeController controller, string[] cmd)
        {
            controller.SendCommand(cmd);
            return controller.ReceiveResult();
        }

        public bool WaitForPluginStartup(int seconds)
        {
            object[] param = new object[] { seconds };
            this.Log("Waiting for Xcode plugin for up to {0} seconds", param);
            for (int i = 0; i < seconds; i++)
            {
                try
                {
                    this.Connect();
                    this.Log("Successfully connected to the plugin", new object[0]);
                    this.Disconnect();
                    return true;
                }
                catch (Exception)
                {
                }
                Thread.Sleep(0x3e8);
            }
            this.Log("Waiting for plugin timed out", new object[0]);
            return false;
        }

        private bool XcodeIsRunning()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("/bin/ps") {
                Arguments = "axc -o pid,state,command",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            Process process = Process.Start(startInfo);
            process.WaitForExit(0x2710);
            char[] separator = new char[] { '\n' };
            string[] strArray = process.StandardOutput.ReadToEnd().Split(separator);
            foreach (string str in strArray)
            {
                Match match = Regex.Match(str, @"(\d+) (....) (.*)$");
                if (match.Success)
                {
                    string str2 = match.Groups[2].ToString();
                    if ((((str2[0] != 'T') && (str2[0] != 'Z')) && !str2.Contains("E")) && match.Groups[3].ToString().Contains("Xcode"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private enum Command
        {
            BuildProject = 9,
            CleanProject = 8,
            CloseAllUnityProjects = 1,
            MobileDeviceList = 10,
            OpenProject = 2,
            RunProject = 5,
            SelectDeviceScheme = 4,
            SelectSimulatorScheme = 3,
            Version = 7,
            WaitForActivitiesFinish = 6
        }
    }
}

