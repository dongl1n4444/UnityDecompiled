namespace UnityEditor.Android
{
    using System;
    using UnityEditor;
    using UnityEditor.Modules;

    internal class TargetExtension : DefaultPlatformSupportModule
    {
        private AndroidPluginImporterExtension pluginImporterExtension;
        private static AndroidBuildWindowExtension s_BuildWindow;

        public override IBuildPostprocessor CreateBuildPostprocessor() => 
            new AndroidBuildPostprocessor();

        public override IBuildWindowExtension CreateBuildWindowExtension() => 
            ((s_BuildWindow != null) ? s_BuildWindow : (s_BuildWindow = new AndroidBuildWindowExtension()));

        public override IPluginImporterExtension CreatePluginImporterExtension() => 
            ((this.pluginImporterExtension != null) ? this.pluginImporterExtension : (this.pluginImporterExtension = new AndroidPluginImporterExtension()));

        public override IPreferenceWindowExtension CreatePreferenceWindowExtension() => 
            new AndroidPreferenceWindowExtension();

        public override IScriptingImplementations CreateScriptingImplementations() => 
            new AndroidScriptingImplementations();

        public override ISettingEditorExtension CreateSettingsEditorExtension() => 
            new PlayerSettingsEditorExtension();

        public override string JamTarget =>
            "AndroidExtensions";

        public override string TargetName =>
            "Android";
    }
}

