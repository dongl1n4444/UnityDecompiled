namespace UnityEditor
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using UnityEditor.Utils;
    using UnityEngine;
    using UnityEngine.Scripting;

    internal class LocalCacheServer : ScriptableSingleton<LocalCacheServer>
    {
        public const string CustomPathKey = "LocalCacheServerCustomPath";
        [SerializeField]
        public string path;
        public const string PathKey = "LocalCacheServerPath";
        [SerializeField]
        public int pid = -1;
        [SerializeField]
        public int port;
        [SerializeField]
        public ulong size;
        public const string SizeKey = "LocalCacheServerSize";
        [SerializeField]
        public string time;

        public static bool CheckCacheLocationExists() => 
            Directory.Exists(GetCacheLocation());

        public static bool CheckValidCacheLocation(string path)
        {
            if (Directory.Exists(path))
            {
                string[] fileSystemEntries = Directory.GetFileSystemEntries(path);
                foreach (string str in fileSystemEntries)
                {
                    string str2 = Path.GetFileName(str).ToLower();
                    if (((str2.Length != 2) && (str2 != "temp")) && ((str2 != ".ds_store") && (str2 != "desktop.ini")))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static void Clear()
        {
            Kill();
            string cacheLocation = GetCacheLocation();
            if (Directory.Exists(cacheLocation))
            {
                Directory.Delete(cacheLocation, true);
            }
        }

        private void Create(int _port, ulong _size)
        {
            string[] components = new string[] { EditorApplication.applicationContentsPath, "Tools", "nodejs" };
            string fileName = Paths.Combine(components);
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                string[] textArray2 = new string[] { fileName, "node.exe" };
                fileName = Paths.Combine(textArray2);
            }
            else
            {
                string[] textArray3 = new string[] { fileName, "bin", "node" };
                fileName = Paths.Combine(textArray3);
            }
            CreateCacheDirectory();
            this.path = GetCacheLocation();
            string[] textArray4 = new string[] { EditorApplication.applicationContentsPath, "Tools", "CacheServer", "main.js" };
            string str2 = Paths.Combine(textArray4);
            ProcessStartInfo info2 = new ProcessStartInfo(fileName);
            object[] objArray1 = new object[] { "\"", str2, "\" --port ", _port, " --path ", this.path, " --nolegacy --monitor-parent-process ", Process.GetCurrentProcess().Id, " --silent --size ", _size };
            info2.Arguments = string.Concat(objArray1);
            info2.UseShellExecute = false;
            info2.CreateNoWindow = true;
            ProcessStartInfo info = info2;
            Process process = new Process {
                StartInfo = info
            };
            process.Start();
            this.port = _port;
            this.pid = process.Id;
            this.size = _size;
            this.time = process.StartTime.ToString();
            this.Save(true);
        }

        public static void CreateCacheDirectory()
        {
            string cacheLocation = GetCacheLocation();
            if (!Directory.Exists(cacheLocation))
            {
                Directory.CreateDirectory(cacheLocation);
            }
        }

        public static void CreateIfNeeded()
        {
            Process processById = null;
            try
            {
                processById = Process.GetProcessById(ScriptableSingleton<LocalCacheServer>.instance.pid);
            }
            catch
            {
            }
            ulong num = (ulong) (((EditorPrefs.GetInt("LocalCacheServerSize", 10) * 0x400L) * 0x400L) * 0x400L);
            if ((processById != null) && (processById.StartTime.ToString() == ScriptableSingleton<LocalCacheServer>.instance.time))
            {
                if ((ScriptableSingleton<LocalCacheServer>.instance.size == num) && (ScriptableSingleton<LocalCacheServer>.instance.path == GetCacheLocation()))
                {
                    CreateCacheDirectory();
                    return;
                }
                Kill();
            }
            ScriptableSingleton<LocalCacheServer>.instance.Create(GetRandomUnusedPort(), num);
            WaitForServerToComeAlive(ScriptableSingleton<LocalCacheServer>.instance.port);
        }

        public static string GetCacheLocation()
        {
            string str = EditorPrefs.GetString("LocalCacheServerPath");
            bool @bool = EditorPrefs.GetBool("LocalCacheServerCustomPath");
            string str2 = str;
            if (@bool && !string.IsNullOrEmpty(str))
            {
                return str2;
            }
            string[] components = new string[] { OSUtil.GetDefaultCachePath(), "CacheServer" };
            return Paths.Combine(components);
        }

        [UsedByNativeCode]
        public static int GetLocalCacheServerPort()
        {
            Setup();
            return ScriptableSingleton<LocalCacheServer>.instance.port;
        }

        public static int GetRandomUnusedPort()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 0);
            listener.Start();
            int port = ((IPEndPoint) listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        public static void Kill()
        {
            if (ScriptableSingleton<LocalCacheServer>.instance.pid != -1)
            {
                try
                {
                    Process.GetProcessById(ScriptableSingleton<LocalCacheServer>.instance.pid).Kill();
                    ScriptableSingleton<LocalCacheServer>.instance.pid = -1;
                }
                catch
                {
                }
            }
        }

        public static bool PingHost(string host, int port, int timeout)
        {
            bool connected;
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    client.BeginConnect(host, port, null, null).AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds((double) timeout));
                    connected = client.Connected;
                }
            }
            catch
            {
                connected = false;
            }
            return connected;
        }

        public static void Setup()
        {
            if (EditorPrefs.GetInt("CacheServerMode") == 0)
            {
                CreateIfNeeded();
            }
            else
            {
                Kill();
            }
        }

        public static bool WaitForServerToComeAlive(int port)
        {
            for (int i = 0; i < 500; i++)
            {
                if (PingHost("localhost", port, 10))
                {
                    Console.WriteLine("Server Came alive after " + (i * 10) + "ms");
                    return true;
                }
            }
            return false;
        }
    }
}

