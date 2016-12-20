namespace UnityEditor.Android
{
    using System;
    using UnityEditor;
    using UnityEditor.Modules;

    internal class TargetExtension : DefaultPlatformSupportModule
    {
        private AndroidPluginImporterExtension pluginImporterExtension;
        private static AndroidBuildWindowExtension s_BuildWindow;

        public override IBuildPostprocessor CreateBuildPostprocessor()
        {
            return new AndroidBuildPostprocessor();
        }

        public override IBuildWindowExtension CreateBuildWindowExtension()
        {
            return ((s_BuildWindow != null) ? s_BuildWindow : (s_BuildWindow = new AndroidBuildWindowExtension()));
        }

        public override IPluginImporterExtension CreatePluginImporterExtension()
        {
            return ((this.pluginImporterExtension != null) ? this.pluginImporterExtension : (this.pluginImporterExtension = new AndroidPluginImporterExtension()));
        }

        public override IPreferenceWindowExtension CreatePreferenceWindowExtension()
        {
            return new AndroidPreferenceWindowExtension();
        }

        public override IScriptingImplementations CreateScriptingImplementations()
        {
            return new AndroidScriptingImplementations();
        }

        public override ISettingEditorExtension CreateSettingsEditorExtension()
        {
            return new PlayerSettingsEditorExtension();
        }

        public override string JamTarget
        {
            get
            {
                return "AndroidExtensions";
            }
        }

        public override string TargetName
        {
            get
            {
                return "Android";
            }
        }
    }
}

