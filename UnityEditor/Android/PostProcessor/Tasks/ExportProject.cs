namespace UnityEditor.Android.PostProcessor.Tasks
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEditor.Android;
    using UnityEditor.Android.PostProcessor;
    using UnityEngine;

    internal class ExportProject : IPostProcessorTask
    {
        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event ProgressHandler OnProgress;

        public void Execute(PostProcessorContext context)
        {
            if (context.Get<bool>("ExportAndroidProject"))
            {
                if (this.OnProgress != null)
                {
                    this.OnProgress(this, "Exporting Google Android project");
                }
                int system = context.Get<int>("ProjectType");
                AndroidProjectExport export = AndroidProjectExport.Create(system);
                if (export == null)
                {
                    throw new UnityException("Unknown project type: " + system);
                }
                string targetPath = context.Get<string>("InstallPath");
                export.Export(context, targetPath);
            }
        }

        public string Name =>
            "Exporting project";
    }
}

