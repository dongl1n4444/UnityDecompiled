namespace UnityEditor.WebGL
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEditor.Scripting;
    using UnityEditor.Utils;
    using UnityEditor.WebGL.Emscripten;
    using UnityEngine;

    internal class HttpServerEditorWrapper : ScriptableSingleton<HttpServerEditorWrapper>
    {
        [SerializeField]
        public string path;
        [SerializeField]
        public int pid;
        [SerializeField]
        public int port;
        [SerializeField]
        public string time;

        private void Create(string _path, int _port)
        {
            string str2;
            string executable = EmscriptenPaths.buildToolsDir + "/SimpleWebServer.exe";
            object[] objArray1 = new object[] { "\"", _path, "\" ", _port, " ", Process.GetCurrentProcess().Id };
            ManagedProgram program = new ManagedProgram(MonoInstallationFinder.GetMonoInstallation("Mono"), "2.0", executable, string.Concat(objArray1), null);
            program._process.Start();
            do
            {
                str2 = program._process.StandardOutput.ReadLine();
            }
            while ((str2 != null) && !str2.Contains("Starting web server"));
            this.path = _path;
            this.port = _port;
            this.pid = program._process.Id;
            this.time = program._process.StartTime.ToString();
            this.Save(true);
        }

        public static void CreateIfNeeded(string path, out int port)
        {
            Process processById = null;
            try
            {
                processById = Process.GetProcessById(ScriptableSingleton<HttpServerEditorWrapper>.instance.pid);
            }
            catch
            {
            }
            if ((processById != null) && (processById.StartTime.ToString() == ScriptableSingleton<HttpServerEditorWrapper>.instance.time))
            {
                if (ScriptableSingleton<HttpServerEditorWrapper>.instance.path == path)
                {
                    port = ScriptableSingleton<HttpServerEditorWrapper>.instance.port;
                    return;
                }
                processById.Kill();
            }
            port = GetRandomUnusedPort();
            ScriptableSingleton<HttpServerEditorWrapper>.instance.Create(path, port);
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
            try
            {
                Process.GetProcessById(ScriptableSingleton<HttpServerEditorWrapper>.instance.pid).Kill();
            }
            catch
            {
            }
        }
    }
}

