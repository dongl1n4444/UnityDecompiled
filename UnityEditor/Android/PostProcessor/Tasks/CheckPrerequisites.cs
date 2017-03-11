﻿namespace UnityEditor.Android.PostProcessor.Tasks
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEditor;
    using UnityEditor.Android.PostProcessor;
    using UnityEditor.Utils;

    internal class CheckPrerequisites : IPostProcessorTask
    {
        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event ProgressHandler OnProgress;

        private bool ArePasswordsProvided() => 
            ((PlayerSettings.Android.keyaliasName.Length == 0) || ((PlayerSettings.Android.keystorePass.Length != 0) && (PlayerSettings.Android.keyaliasPass.Length != 0)));

        private void CheckUnityLibraryForArchitecture(PostProcessorContext context, string arch)
        {
            int num = context.Get<int>("ProjectType");
            bool flag = context.Get<bool>("SourceBuild");
            if ((num != 3) || !flag)
            {
                string[] components = new string[] { TasksCommon.GetLibsDirectory(context), arch, "libunity.so" };
                string path = Paths.Combine(components);
                if (!File.Exists(path))
                {
                    CancelPostProcess.AbortBuild("Unable to package apk", "Unity library missing for the selected architecture '" + arch + " (" + path + ") !", null);
                }
            }
        }

        private void EnsureUnityLibrariesAreAvailable(PostProcessorContext context)
        {
            AndroidTargetDevice device = context.Get<AndroidTargetDevice>("TargetDevice");
            switch (device)
            {
                case AndroidTargetDevice.FAT:
                case AndroidTargetDevice.ARMv7:
                    this.CheckUnityLibraryForArchitecture(context, "armeabi-v7a");
                    break;
            }
            if ((device == AndroidTargetDevice.FAT) || (device == AndroidTargetDevice.x86))
            {
                this.CheckUnityLibraryForArchitecture(context, "x86");
            }
        }

        public void Execute(PostProcessorContext context)
        {
            if (this.OnProgress != null)
            {
                this.OnProgress(this, "Starting Android build");
            }
            this.EnsureUnityLibrariesAreAvailable(context);
            if (!context.Get<bool>("ExportAndroidProject") && !this.ArePasswordsProvided())
            {
                CancelPostProcess.AbortBuild("Can not sign application", "Unable to sign application; please provide passwords!", null);
            }
        }

        public string Name =>
            "Checking prerequisutes";
    }
}

