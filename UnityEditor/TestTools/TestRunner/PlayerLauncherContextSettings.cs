namespace UnityEditor.TestTools.TestRunner
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal class PlayerLauncherContextSettings : IDisposable
    {
        private bool m_ApplicationRunInBackground;
        private ResolutionDialogSetting m_DisplayResolutionDialog;
        private bool m_Disposed;
        private EditorBuildSettingsScene[] m_EditorBuildSettings;
        private bool m_FullScreen;
        private bool m_ResizableWindow;
        private bool m_RunInBackground;
        private bool m_ShowUnitySplashScreen;

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
            PlayerSettings.SplashScreen.show = this.m_ShowUnitySplashScreen;
            EditorApplication.UnlockReloadAssemblies();
        }

        public void Dispose()
        {
            if (!this.m_Disposed)
            {
                this.CleanupProjectParameters();
                this.m_Disposed = true;
            }
        }

        private void SetupProjectParameters()
        {
            EditorApplication.LockReloadAssemblies();
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
            this.m_ShowUnitySplashScreen = PlayerSettings.SplashScreen.show;
            PlayerSettings.SplashScreen.show = false;
        }
    }
}

