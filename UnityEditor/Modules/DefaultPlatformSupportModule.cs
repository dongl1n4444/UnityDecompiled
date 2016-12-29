namespace UnityEditor.Modules
{
    using System;
    using UnityEngine;

    internal abstract class DefaultPlatformSupportModule : IPlatformSupportModule
    {
        protected ICompilationExtension compilationExtension;
        protected ITextureImportSettingsExtension textureSettingsExtension;

        protected DefaultPlatformSupportModule()
        {
        }

        public virtual IBuildAnalyzer CreateBuildAnalyzer() => 
            null;

        public abstract IBuildPostprocessor CreateBuildPostprocessor();
        public virtual IBuildWindowExtension CreateBuildWindowExtension() => 
            null;

        public virtual ICompilationExtension CreateCompilationExtension() => 
            ((this.compilationExtension == null) ? (this.compilationExtension = new DefaultCompilationExtension()) : this.compilationExtension);

        public virtual IDevice CreateDevice(string id)
        {
            throw new NotSupportedException();
        }

        public virtual IPluginImporterExtension CreatePluginImporterExtension() => 
            null;

        public virtual IPreferenceWindowExtension CreatePreferenceWindowExtension() => 
            null;

        public virtual IProjectGeneratorExtension CreateProjectGeneratorExtension() => 
            null;

        public virtual IScriptingImplementations CreateScriptingImplementations() => 
            null;

        public virtual ISettingEditorExtension CreateSettingsEditorExtension() => 
            null;

        public virtual ITextureImportSettingsExtension CreateTextureImportSettingsExtension() => 
            ((this.textureSettingsExtension == null) ? (this.textureSettingsExtension = new DefaultTextureImportSettingsExtension()) : this.textureSettingsExtension);

        public virtual IUserAssembliesValidator CreateUserAssembliesValidatorExtension() => 
            null;

        public virtual GUIContent[] GetDisplayNames() => 
            null;

        public virtual void OnActivate()
        {
        }

        public virtual void OnDeactivate()
        {
        }

        public virtual void OnLoad()
        {
        }

        public virtual void OnUnload()
        {
        }

        public virtual void RegisterAdditionalUnityExtensions()
        {
        }

        public virtual string[] AssemblyReferencesForEditorCsharpProject =>
            new string[0];

        public virtual string[] AssemblyReferencesForUserScripts =>
            new string[0];

        public virtual string ExtensionVersion =>
            null;

        public abstract string JamTarget { get; }

        public virtual string[] NativeLibraries =>
            new string[0];

        public abstract string TargetName { get; }
    }
}

