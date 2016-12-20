namespace UnityEditor.Modules
{
    using System;
    using UnityEngine;

    internal interface IPlatformSupportModule
    {
        IBuildAnalyzer CreateBuildAnalyzer();
        IBuildPostprocessor CreateBuildPostprocessor();
        IBuildWindowExtension CreateBuildWindowExtension();
        ICompilationExtension CreateCompilationExtension();
        IDevice CreateDevice(string id);
        IPluginImporterExtension CreatePluginImporterExtension();
        IPreferenceWindowExtension CreatePreferenceWindowExtension();
        IProjectGeneratorExtension CreateProjectGeneratorExtension();
        IScriptingImplementations CreateScriptingImplementations();
        ISettingEditorExtension CreateSettingsEditorExtension();
        ITextureImportSettingsExtension CreateTextureImportSettingsExtension();
        IUserAssembliesValidator CreateUserAssembliesValidatorExtension();
        GUIContent[] GetDisplayNames();
        void OnActivate();
        void OnDeactivate();
        void OnLoad();
        void OnUnload();
        void RegisterAdditionalUnityExtensions();

        string[] AssemblyReferencesForEditorCsharpProject { get; }

        string[] AssemblyReferencesForUserScripts { get; }

        string ExtensionVersion { get; }

        string JamTarget { get; }

        string[] NativeLibraries { get; }

        string TargetName { get; }
    }
}

