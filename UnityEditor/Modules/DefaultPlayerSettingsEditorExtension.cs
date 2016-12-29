namespace UnityEditor.Modules
{
    using System;
    using UnityEditor;

    internal abstract class DefaultPlayerSettingsEditorExtension : ISettingEditorExtension
    {
        protected DefaultPlayerSettingsEditorExtension()
        {
        }

        public virtual bool CanShowUnitySplashScreen() => 
            false;

        public virtual void ConfigurationSectionGUI()
        {
        }

        public virtual bool HasBundleIdentifier() => 
            true;

        public virtual bool HasIdentificationGUI() => 
            false;

        public virtual bool HasPublishSection() => 
            true;

        public virtual bool HasResolutionSection() => 
            false;

        public virtual void IconSectionGUI()
        {
        }

        public virtual void IdentificationSectionGUI()
        {
        }

        public virtual void OnEnable(PlayerSettingsEditor settingsEditor)
        {
        }

        public virtual void PublishSectionGUI(float h, float midWidth, float maxWidth)
        {
        }

        public virtual void ResolutionSectionGUI(float h, float midWidth, float maxWidth)
        {
        }

        public virtual void SplashSectionGUI()
        {
        }

        public virtual bool SupportsDynamicBatching() => 
            true;

        public virtual bool SupportsOrientation() => 
            false;

        public virtual bool SupportsStaticBatching() => 
            true;

        public virtual bool UsesStandardIcons() => 
            true;
    }
}

