namespace UnityEditor.WebGL
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.WebGL.Emscripten;
    using UnityEditorInternal;
    using UnityEngine;

    internal class WebsockifyEditorWrapper : ScriptableSingleton<WebsockifyEditorWrapper>
    {
        [CompilerGenerated]
        private static EventHandler <>f__mg$cache0;
        private static Process process;

        private WebsockifyEditorWrapper()
        {
            if (process == null)
            {
                string str = EmscriptenPaths.buildToolsDir + "/websockify";
                ProcessStartInfo info = new ProcessStartInfo(EmscriptenPaths.nodeExecutable) {
                    Arguments = "\"" + str + "/websockify.js\" 54998 localhost:" + ProfilerDriver.directConnectionPort,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                info.EnvironmentVariables["NODE_PATH"] = str + "/node_modules";
                process = new Process();
                process.StartInfo = info;
                process.Start();
                if (<>f__mg$cache0 == null)
                {
                    <>f__mg$cache0 = new EventHandler(WebsockifyEditorWrapper.OnUnload);
                }
                AppDomain.CurrentDomain.DomainUnload += <>f__mg$cache0;
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                if (process == null)
                {
                    UnityEngine.Debug.LogError("Could not start WebSocket bridge.");
                }
            }
        }

        public static void CreateIfNeeded()
        {
            if (ScriptableSingleton<WebsockifyEditorWrapper>.instance == null)
            {
                UnityEngine.Debug.LogError("No Websockify wrapper created");
            }
        }

        private static void OnUnload(object sender, EventArgs e)
        {
            if (process != null)
            {
                if (!process.HasExited)
                {
                    process.Kill();
                }
                process.CancelErrorRead();
                process.CancelOutputRead();
                process = null;
            }
        }
    }
}

