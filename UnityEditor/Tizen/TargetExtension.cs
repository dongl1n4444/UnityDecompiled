namespace UnityEditor.Tizen
{
    using System;
    using UnityEditor.Modules;

    internal class TargetExtension : DefaultPlatformSupportModule
    {
        public override IBuildPostprocessor CreateBuildPostprocessor()
        {
            return new TizenBuildPostprocessor();
        }

        public override IBuildWindowExtension CreateBuildWindowExtension()
        {
            return null;
        }

        public override IDevice CreateDevice(string id)
        {
            throw new NotSupportedException();
        }

        public override IPluginImporterExtension CreatePluginImporterExtension()
        {
            return null;
        }

        public override IPreferenceWindowExtension CreatePreferenceWindowExtension()
        {
            return new TizenPreferenceWindowExtension();
        }

        public override IScriptingImplementations CreateScriptingImplementations()
        {
            return null;
        }

        public override ISettingEditorExtension CreateSettingsEditorExtension()
        {
            return new TizenSettingsEditorExtension();
        }

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

        public override string JamTarget
        {
            get
            {
                return "TizenEditorExtensions";
            }
        }

        public override string TargetName
        {
            get
            {
                return "Tizen";
            }
        }
    }
}

