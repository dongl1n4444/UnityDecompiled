namespace UnityEditor.Facebook
{
    using System;
    using UnityEditor.Modules;

    internal class TargetExtension : DefaultPlatformSupportModule
    {
        private BuildWindowExtension m_BuildWindow;
        private IPlatformSupportModule m_WebGLModule;
        private IPlatformSupportModule m_WindowsStandaloneModule;

        public override IBuildPostprocessor CreateBuildPostprocessor()
        {
            IBuildPostprocessor winProcessor = this.WindowsStandaloneModule?.CreateBuildPostprocessor();
            return new BuildProcessor(winProcessor, this.WebGLModule?.CreateBuildPostprocessor());
        }

        public override IBuildWindowExtension CreateBuildWindowExtension() => 
            ((this.m_BuildWindow != null) ? this.m_BuildWindow : (this.m_BuildWindow = new BuildWindowExtension()));

        public override ISettingEditorExtension CreateSettingsEditorExtension() => 
            new FacebookSettingsExtension();

        public override void OnActivate()
        {
        }

        public override void OnLoad()
        {
            SDKManager.Initialize();
        }

        public override void OnUnload()
        {
        }

        public override void RegisterAdditionalUnityExtensions()
        {
            SDKManager.RegisterAdditionalUnityExtensions();
        }

        public override string JamTarget =>
            "NoTarget";

        public override string TargetName =>
            "Facebook";

        private IPlatformSupportModule WebGLModule
        {
            get
            {
                if (this.m_WebGLModule == null)
                {
                    this.m_WebGLModule = ModuleManager.FindPlatformSupportModule("WebGL");
                }
                return this.m_WebGLModule;
            }
        }

        private IPlatformSupportModule WindowsStandaloneModule
        {
            get
            {
                if (this.m_WindowsStandaloneModule == null)
                {
                    this.m_WindowsStandaloneModule = ModuleManager.FindPlatformSupportModule("WindowsStandalone");
                }
                return this.m_WindowsStandaloneModule;
            }
        }
    }
}

