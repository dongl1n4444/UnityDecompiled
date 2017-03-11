namespace UnityEditor.TestTools.TestRunner
{
    using System;
    using System.Text;
    using UnityEditor;

    internal class PlayerLauncherBuildOptions
    {
        public UnityEditor.BuildPlayerOptions BuildPlayerOptions;
        public string PlayerDirectory;

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("locationPathName = " + this.BuildPlayerOptions.locationPathName);
            builder.AppendLine("target = " + this.BuildPlayerOptions.target);
            builder.AppendLine("scenes = " + string.Join(", ", this.BuildPlayerOptions.scenes));
            builder.AppendLine("assetBundleManifestPath = " + this.BuildPlayerOptions.assetBundleManifestPath);
            builder.AppendLine("options.Il2Cpp = " + ((this.BuildPlayerOptions.options & BuildOptions.Il2CPP) != BuildOptions.CompressTextures));
            builder.AppendLine("options.Development = " + ((this.BuildPlayerOptions.options & BuildOptions.Development) != BuildOptions.CompressTextures));
            builder.AppendLine("options.AutoRunPlayer = " + ((this.BuildPlayerOptions.options & BuildOptions.AutoRunPlayer) != BuildOptions.CompressTextures));
            builder.AppendLine("options.ForceEnableAssertions = " + ((this.BuildPlayerOptions.options & BuildOptions.ForceEnableAssertions) != BuildOptions.CompressTextures));
            return builder.ToString();
        }
    }
}

