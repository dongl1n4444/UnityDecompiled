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

    internal class PrepareUnityPackage : IPostProcessorTask
    {
        private string _playerPackage;
        private string _stagingArea;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event ProgressHandler OnProgress;

        public void Execute(PostProcessorContext context)
        {
            if (this.OnProgress != null)
            {
                this.OnProgress(this, "Copying unity libraries");
            }
            this._stagingArea = context.Get<string>("StagingArea");
            this._playerPackage = context.Get<string>("PlayerPackage");
            AndroidTargetDevice device = context.Get<AndroidTargetDevice>("TargetDevice");
            switch (device)
            {
                case AndroidTargetDevice.FAT:
                case AndroidTargetDevice.ARMv7:
                    this.PrepareNativeUnityLibs(context, "armeabi-v7a");
                    break;
            }
            if ((device == AndroidTargetDevice.FAT) || (device == AndroidTargetDevice.x86))
            {
                this.PrepareNativeUnityLibs(context, "x86");
            }
            string[] components = new string[] { this._playerPackage, "Apk", "assets" };
            FileUtil.CopyDirectoryRecursive(Paths.Combine(components), Path.Combine(this._stagingArea, "assets"), true);
            string[] textArray2 = new string[] { this._playerPackage, "Apk", "res" };
            FileUtil.CopyDirectoryRecursive(Paths.Combine(textArray2), Path.Combine(this._stagingArea, "res"), true);
        }

        private void PrepareNativeUnityLibs(PostProcessorContext context, string abi)
        {
            string pluginSourceFolder = context.Get<string>("AndroidPluginsPath");
            bool flag = context.Get<bool>("SourceBuild");
            string[] components = new string[] { this._stagingArea, "libs", abi };
            string to = Paths.Combine(components);
            string[] textArray2 = new string[] { this._stagingArea, "libs" };
            Directory.CreateDirectory(Paths.Combine(textArray2));
            if (!flag)
            {
                string libsDirectory = TasksCommon.GetLibsDirectory(context);
                string[] textArray3 = new string[] { libsDirectory, abi };
                FileUtil.CopyFileOrDirectory(Paths.Combine(textArray3), to);
            }
            string[] textArray4 = new string[] { pluginSourceFolder, "libs", abi };
            string str4 = Paths.Combine(textArray4);
            string[] textArray5 = new string[] { to, "gdbserver" };
            if (!File.Exists(Paths.Combine(textArray5)) && !PostprocessBuildPlayer.InstallPluginsByExtension(pluginSourceFolder, "gdbserver", string.Empty, to, false))
            {
                PostprocessBuildPlayer.InstallPluginsByExtension(str4, "gdbserver", string.Empty, to, false);
            }
        }

        public string Name =>
            "Preparing Unity libraries";
    }
}

