namespace UnityEditor.SamsungTV
{
    using System;
    using UnityEditor;
    using UnityEditor.Modules;

    internal class TargetExtension : DefaultPlatformSupportModule
    {
        private static SamsungTVBuildWindowExtension buildWindow;
        private STVPluginImporterExtension pluginImporterExtension;

        public override IBuildPostprocessor CreateBuildPostprocessor()
        {
            return new SamsungTVBuildPostprocessor();
        }

        public override IBuildWindowExtension CreateBuildWindowExtension()
        {
            return ((buildWindow != null) ? buildWindow : (buildWindow = new SamsungTVBuildWindowExtension()));
        }

        public override IPluginImporterExtension CreatePluginImporterExtension()
        {
            return ((this.pluginImporterExtension != null) ? this.pluginImporterExtension : (this.pluginImporterExtension = new STVPluginImporterExtension()));
        }

        public override ISettingEditorExtension CreateSettingsEditorExtension()
        {
            return new SamsungTVSettingsEditorExtension();
        }

        public override string JamTarget
        {
            get
            {
                return "SamsungTVEditorExtensions";
            }
        }

        public override string TargetName
        {
            get
            {
                return "SamsungTV";
            }
        }
    }
}

