namespace UnityEditor.Android.PostProcessor.Tasks
{
    using System;
    using System.IO;
    using System.Threading;
    using UnityEditor;
    using UnityEditor.Android.PostProcessor;

    internal class MoveFinalPackage : IPostProcessorTask
    {
        private string _installDirectory;
        private string _installPath;

        public event ProgressHandler OnProgress;

        private void Clean()
        {
            if (Directory.Exists(this._installPath))
            {
                try
                {
                    Directory.Delete(this._installPath);
                }
                catch (IOException)
                {
                    CancelPostProcess.AbortBuild("Unable to create new apk!", $"Unable to write target apk because {this._installPath} is a non-empty directory", null);
                }
            }
            else
            {
                if (File.Exists(this._installPath))
                {
                    File.Delete(this._installPath);
                }
                if (File.Exists(this._installPath))
                {
                    CancelPostProcess.AbortBuild("Unable to delete old apk!", $"Target apk could not be overwritten: {this._installPath}", null);
                }
            }
            if (!Directory.Exists(this._installDirectory))
            {
                Directory.CreateDirectory(this._installDirectory);
            }
        }

        public void Execute(PostProcessorContext context)
        {
            this._installPath = context.Get<string>("InstallPath");
            this._installDirectory = Path.GetDirectoryName(this._installPath);
            if (this.OnProgress != null)
            {
                this.OnProgress(this, "Moving final Android package");
            }
            this.Clean();
            this.Move(context);
        }

        private void Move(PostProcessorContext context)
        {
            string str = context.Get<string>("StagingArea");
            bool flag = context.Get<bool>("UseObb");
            FileUtil.MoveFileOrDirectory(Path.Combine(str, "Package.apk"), this._installPath);
            if (!File.Exists(this._installPath))
            {
                CancelPostProcess.AbortBuild("Unable to create new apk!", $"Unable to move file '{Path.Combine(str, "Package.apk")}' -> '{this._installPath}", null);
            }
            if (flag && File.Exists(Path.Combine(str, "main.obb")))
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(this._installPath);
                string path = Path.Combine(this._installDirectory, $"{fileNameWithoutExtension}.main.obb");
                FileUtil.DeleteFileOrDirectory(path);
                FileUtil.MoveFileOrDirectory(Path.Combine(str, "main.obb"), path);
            }
        }

        public string Name =>
            "Moving output package";
    }
}

