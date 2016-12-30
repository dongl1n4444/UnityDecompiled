namespace UnityEditor.Android.PostProcessor.Tasks
{
    using System;
    using System.IO;
    using System.Threading;
    using UnityEditor;
    using UnityEditor.Android.PostProcessor;

    internal class PrepareAPKResources : IPostProcessorTask
    {
        private string _stagingArea;

        public event ProgressHandler OnProgress;

        public void Execute(PostProcessorContext context)
        {
            if (this.OnProgress != null)
            {
                this.OnProgress(this, "Patching settings file");
            }
            this._stagingArea = context.Get<string>("StagingArea");
            bool flag = context.Get<bool>("UseObb");
            string str = context.Get<string>("AndroidPluginsPath");
            if (Directory.Exists(Path.Combine(str, "assets")))
            {
                FileUtil.CopyDirectoryRecursiveForPostprocess(Path.Combine(str, "assets"), Path.Combine(this._stagingArea, "assets"), true);
            }
            this.PatchStringsXml();
            int num = !PlayerSettings.advancedLicense ? 0 : ((int) PlayerSettings.Android.splashScreenScale);
            AndroidXmlDocument document = new AndroidXmlDocument(Path.Combine(this._stagingArea, "assets/bin/Data/settings.xml"));
            document.PatchStringRes("integer", "splash_mode", num.ToString());
            document.PatchStringRes("bool", "useObb", flag.ToString());
            document.PatchStringRes("bool", "showSplash", !PlayerSettings.virtualRealitySupported.ToString());
            context.Set<AndroidXmlDocument>("SettingsXml", document);
            document.Save();
        }

        private void PatchStringsXml()
        {
            AndroidXmlDocument document = new AndroidXmlDocument(Path.Combine(this._stagingArea, "res/values/strings.xml"));
            document.PatchStringRes("string", "app_name", PlayerSettings.productName);
            document.Save();
        }

        public string Name =>
            "Preparing APK resources";
    }
}

