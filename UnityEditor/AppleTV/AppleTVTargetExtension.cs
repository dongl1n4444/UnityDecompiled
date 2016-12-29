namespace UnityEditor.AppleTV
{
    using System;
    using System.IO;
    using UnityEditor;
    using UnityEditor.iOS;
    using UnityEditor.Modules;
    using UnityEngine;

    internal class AppleTVTargetExtension : DefaultPlatformSupportModule
    {
        internal const string AppleTVTargetName = "tvOS";
        private string[] assemblyReferencesForUserScripts = new string[] { Path.Combine(UnityEditor.BuildPipeline.GetPlaybackEngineDirectory(BuildTarget.tvOS, BuildOptions.CompressTextures), "UnityEditor.iOS.Extensions.Xcode.dll") };
        private AppleTVBuildWindowExtension buildWindow;
        internal const string iOSTargetName = "iOS";
        private AppleTVPluginImporterExtension m_pluginImporterExtension;
        internal static Version MinimumOsVersion = new Version("9.0");
        private string[] nativeLibraries;
        private AppleTVSettingsEditorExtension settingsEditor;

        public AppleTVTargetExtension()
        {
            string playbackEngineDirectory = UnityEditor.BuildPipeline.GetPlaybackEngineDirectory(BuildTarget.tvOS, BuildOptions.CompressTextures);
            string str2 = "UnityEditor.iOS.Native";
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                str2 = Path.Combine((IntPtr.Size != 4) ? "x86_64" : "x86", str2);
            }
            this.nativeLibraries = new string[] { Path.Combine(playbackEngineDirectory, str2) };
        }

        public override IBuildPostprocessor CreateBuildPostprocessor()
        {
            PostProcessorSettings settings = new PostProcessorSettings {
                OsName = "tvOS",
                MinimumOsVersion = MinimumOsVersion
            };
            return new iOSBuildPostprocessor(settings);
        }

        public override IBuildWindowExtension CreateBuildWindowExtension()
        {
            if (this.buildWindow == null)
            {
                this.buildWindow = new AppleTVBuildWindowExtension();
            }
            return this.buildWindow;
        }

        public override IPluginImporterExtension CreatePluginImporterExtension()
        {
            if (this.m_pluginImporterExtension == null)
            {
                this.m_pluginImporterExtension = new AppleTVPluginImporterExtension();
            }
            return this.m_pluginImporterExtension;
        }

        public override IScriptingImplementations CreateScriptingImplementations() => 
            new AppleTVScriptingImplementations();

        public override ISettingEditorExtension CreateSettingsEditorExtension()
        {
            if (this.settingsEditor == null)
            {
                this.settingsEditor = new AppleTVSettingsEditorExtension();
            }
            return this.settingsEditor;
        }

        public override string[] AssemblyReferencesForEditorCsharpProject =>
            this.assemblyReferencesForUserScripts;

        public override string[] AssemblyReferencesForUserScripts =>
            this.assemblyReferencesForUserScripts;

        public override string JamTarget =>
            "iOSEditorExtensions";

        public override string[] NativeLibraries =>
            this.nativeLibraries;

        public override string TargetName =>
            "tvOS";
    }
}

