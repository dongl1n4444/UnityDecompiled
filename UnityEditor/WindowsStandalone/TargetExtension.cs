namespace UnityEditor.WindowsStandalone
{
    using System;
    using UnityEditor;
    using UnityEditor.Modules;

    internal class TargetExtension : DefaultPlatformSupportModule
    {
        public override IBuildPostprocessor CreateBuildPostprocessor() => 
            new WindowsDesktopStandalonePostProcessor();

        public override IBuildWindowExtension CreateBuildWindowExtension() => 
            new WindowsStandaloneBuildWindowExtension();

        public override IPluginImporterExtension CreatePluginImporterExtension() => 
            new DesktopPluginImporterExtension();

        public override IScriptingImplementations CreateScriptingImplementations() => 
            new DesktopStandalonePostProcessor.ScriptingImplementations();

        public override string JamTarget =>
            "WindowsStandaloneEditorExtensions";

        public override string TargetName =>
            "WindowsStandalone";
    }
}

