namespace UnityEditor.PlaymodeTestsRunner.TestLauncher
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal class PlayerLauncherContextSettings : IDisposable
    {
        private bool m_ApplicationRunInBackground;
        private ResolutionDialogSetting m_DisplayResolutionDialog;
        private EditorBuildSettingsScene[] m_EditorBuildSettings;
        private bool m_FullScreen;
        private bool m_ResizableWindow;
        private bool m_RunInBackground;

        public PlayerLauncherContextSettings()
        {
            this.SetupProjectParameters();
        }

        private void CleanupProjectParameters()
        {
            Application.runInBackground = this.m_ApplicationRunInBackground;
            EditorBuildSettings.scenes = this.m_EditorBuildSettings;
            PlayerSettings.defaultIsFullScreen = this.m_FullScreen;
            PlayerSettings.runInBackground = this.m_RunInBackground;
            PlayerSettings.displayResolutionDialog = this.m_DisplayResolutionDialog;
            PlayerSettings.resizableWindow = this.m_ResizableWindow;
        }

        public void Dispose()
        {
            this.CleanupProjectParameters();
        }

        private void SetupProjectParameters()
        {
            this.m_ApplicationRunInBackground = Application.runInBackground;
            Application.runInBackground = true;
            this.m_EditorBuildSettings = EditorBuildSettings.scenes;
            this.m_DisplayResolutionDialog = PlayerSettings.displayResolutionDialog;
            PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Disabled;
            this.m_RunInBackground = PlayerSettings.runInBackground;
            PlayerSettings.runInBackground = true;
            this.m_FullScreen = PlayerSettings.defaultIsFullScreen;
            PlayerSettings.defaultIsFullScreen = false;
            this.m_ResizableWindow = PlayerSettings.resizableWindow;
            PlayerSettings.resizableWindow = true;
        }
    }
}

