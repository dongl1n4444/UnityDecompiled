namespace UnityEditor.Android.PostProcessor.Tasks
{
    using System;
    using System.IO;
    using System.Threading;
    using UnityEditor;
    using UnityEditor.Android;
    using UnityEditor.Android.PostProcessor;
    using UnityEditor.Utils;
    using UnityEditorInternal;
    using UnityEngine;

    internal class NativePlugins : IPostProcessorTask
    {
        private string _aarFolder;
        private string _pluginsFolder;
        private string _stagingArea;

        public event ProgressHandler OnProgress;

        public void Execute(PostProcessorContext context)
        {
            if (this.OnProgress != null)
            {
                this.OnProgress(this, "Processing native plugins");
            }
            this._stagingArea = context.Get<string>("StagingArea");
            string str = context.Get<string>("TargetLibrariesFolder");
            context.Set<bool>("HasJarPlugins", false);
            this._pluginsFolder = Path.Combine(this._stagingArea, "plugins");
            Directory.CreateDirectory(this._pluginsFolder);
            this._aarFolder = Path.Combine(this._stagingArea, "aar");
            Directory.CreateDirectory(this._aarFolder);
            BuildTarget target = context.Get<BuildTarget>("BuildTarget");
            foreach (PluginDesc desc in PluginImporter.GetExtensionPlugins(target))
            {
                string pluginTargetCPU = desc.architecture.ToString();
                this.ProcessPlugin(context, desc.pluginPath, pluginTargetCPU);
            }
            foreach (PluginImporter importer in PluginImporter.GetImporters(target))
            {
                string assetPath = importer.assetPath;
                if (Directory.Exists(assetPath) && AndroidLibraries.IsAndroidLibraryProject(assetPath))
                {
                    FileUtil.CopyDirectoryRecursiveForPostprocess(assetPath, Path.Combine(str, Path.GetFileName(assetPath)), true);
                }
                this.UpgradePluginCPU(importer, target);
                string platformData = importer.GetPlatformData(target, "CPU");
                this.ProcessPlugin(context, assetPath, platformData);
            }
        }

        private void PrepareNativePlugin(PostProcessorContext context, string assetPath, string pluginTargetCPU)
        {
            string[] textArray1;
            string str;
            AndroidTargetDevice device = context.Get<AndroidTargetDevice>("TargetDevice");
            if ((device == AndroidTargetDevice.FAT) || device.ToString().Equals(pluginTargetCPU))
            {
                str = "";
                if (pluginTargetCPU != null)
                {
                    if (pluginTargetCPU != "ARMv7")
                    {
                        if (pluginTargetCPU == "x86")
                        {
                            str = "x86";
                            goto Label_0088;
                        }
                    }
                    else
                    {
                        str = "armeabi-v7a";
                        goto Label_0088;
                    }
                }
                Debug.LogWarning(string.Format("Unknown cpu architecture for .so library ({0})", assetPath));
            }
            return;
        Label_0088:
            textArray1 = new string[] { this._stagingArea, "libs", str };
            string path = Paths.Combine(textArray1);
            Directory.CreateDirectory(path);
            FileUtil.UnityFileCopy(assetPath, Path.Combine(path, Path.GetFileName(assetPath)));
        }

        private void ProcessPlugin(PostProcessorContext context, string pluginPath, string pluginTargetCPU)
        {
            string extension = Path.GetExtension(pluginPath);
            string fileName = Path.GetFileName(pluginPath);
            switch (extension)
            {
                case ".jar":
                    FileUtil.UnityFileCopy(pluginPath, Path.Combine(this._pluginsFolder, fileName), true);
                    context.Set<bool>("HasJarPlugins", true);
                    break;

                case ".aar":
                    FileUtil.UnityFileCopy(pluginPath, Path.Combine(this._aarFolder, fileName), true);
                    break;

                case ".so":
                    this.PrepareNativePlugin(context, pluginPath, pluginTargetCPU);
                    break;
            }
        }

        private void UpgradePluginCPU(PluginImporter plugin, BuildTarget buildTarget)
        {
            string extension = Path.GetExtension(plugin.assetPath);
            string platformData = plugin.GetPlatformData(buildTarget, "CPU");
            if ((extension == ".so") && (platformData == "AnyCPU"))
            {
                plugin.SetPlatformData(buildTarget, "CPU", "ARMv7");
            }
        }

        public string Name
        {
            get
            {
                return "Process plugins";
            }
        }
    }
}

