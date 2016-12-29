namespace UnityEditor.Tizen
{
    using System;
    using UnityEditor.Modules;

    internal class TargetExtension : DefaultPlatformSupportModule
    {
        public override IBuildPostprocessor CreateBuildPostprocessor() => 
            new TizenBuildPostprocessor();

        public override IBuildWindowExtension CreateBuildWindowExtension() => 
            null;

        public override IDevice CreateDevice(string id)
        {
            throw new NotSupportedException();
        }

        public override IPluginImporterExtension CreatePluginImporterExtension() => 
            null;

        public override IPreferenceWindowExtension CreatePreferenceWindowExtension() => 
            new TizenPreferenceWindowExtension();

        public override IScriptingImplementations CreateScriptingImplementations() => 
            null;

        public override ISettingEditorExtension CreateSettingsEditorExtension() => 
            new TizenSettingsEditorExtension();

        public override void OnActivate()
        {
        }

        public override void OnDeactivate()
        {
        }

        public override void OnLoad()
        {
        }

        public override void OnUnload()
        {
        }

        public override string JamTarget =>
            "TizenEditorExtensions";

        public override string TargetName =>
            "Tizen";
    }
}

