namespace UnityEditor.SamsungTV
{
    using System;
    using UnityEditor;
    using UnityEditor.Modules;

    internal class TargetExtension : DefaultPlatformSupportModule
    {
        private static SamsungTVBuildWindowExtension buildWindow;
        private STVPluginImporterExtension pluginImporterExtension;

        public override IBuildPostprocessor CreateBuildPostprocessor() => 
            new SamsungTVBuildPostprocessor();

        public override IBuildWindowExtension CreateBuildWindowExtension() => 
            ((buildWindow != null) ? buildWindow : (buildWindow = new SamsungTVBuildWindowExtension()));

        public override IPluginImporterExtension CreatePluginImporterExtension() => 
            ((this.pluginImporterExtension != null) ? this.pluginImporterExtension : (this.pluginImporterExtension = new STVPluginImporterExtension()));

        public override ISettingEditorExtension CreateSettingsEditorExtension() => 
            new SamsungTVSettingsEditorExtension();

        public override string JamTarget =>
            "SamsungTVEditorExtensions";

        public override string TargetName =>
            "SamsungTV";
    }
}

