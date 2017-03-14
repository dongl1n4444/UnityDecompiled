namespace UnityEditor.Android.PostProcessor.Tasks
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEditor;
    using UnityEditor.Android.PostProcessor;
    using UnityEditor.Utils;
    using UnityEngine;

    internal class ZipIl2cppSymbols : IPostProcessorTask
    {
        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event ProgressHandler OnProgress;

        public void Execute(PostProcessorContext context)
        {
            if (((ScriptingImplementation) context.Get<ScriptingImplementation>("ScriptingBackend")) == ScriptingImplementation.IL2CPP)
            {
                if (this.OnProgress != null)
                {
                    this.OnProgress(this, "Zipping IL2CPP symbols");
                }
                string str = context.Get<string>("StagingArea");
                string path = context.Get<string>("InstallPath");
                object[] objArray1 = new object[] { Path.GetFileNameWithoutExtension(path), "-", PlayerSettings.bundleVersion, "-v", PlayerSettings.Android.bundleVersionCode, ".symbols.zip" };
                string str3 = string.Concat(objArray1);
                string command = (Application.platform != RuntimePlatform.LinuxEditor) ? Paths.Combine(new string[] { EditorApplication.applicationContentsPath, "Tools", (Application.platform != RuntimePlatform.WindowsEditor) ? "7za" : "7z.exe" }) : "zip";
                string args = (Application.platform != RuntimePlatform.LinuxEditor) ? $"a -tzip -mx0 "{str3}" -r "*/libil2cpp.dbg.so"" : $"-0 -i *.dbg.so -r "{str3}" . ";
                TasksCommon.Exec(command, args, Path.Combine(str, "libs"), "Failed to compress IL2CPP symbol files.", 0);
                string directoryName = Path.GetDirectoryName(path);
                string[] components = new string[] { str, "libs", str3 };
                FileUtil.ReplaceFile(Paths.Combine(components), Path.Combine(directoryName, str3));
                string[] textArray3 = new string[] { str, "libs", str3 };
                FileUtil.DeleteFileOrDirectory(Paths.Combine(textArray3));
            }
        }

        public string Name =>
            "ZipIl2cppSymbols";
    }
}

