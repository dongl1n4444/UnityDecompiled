namespace UnityEditor.TestTools.TestRunner
{
    using System;
    using UnityEngine;

    internal class EditModeLauncherContextSettings : IDisposable
    {
        private bool m_RunInBackground;

        public EditModeLauncherContextSettings()
        {
            this.SetupProjectParameters();
        }

        private void CleanupProjectParameters()
        {
            Application.runInBackground = this.m_RunInBackground;
        }

        public void Dispose()
        {
            this.CleanupProjectParameters();
        }

        private void SetupProjectParameters()
        {
            this.m_RunInBackground = Application.runInBackground;
            Application.runInBackground = true;
        }
    }
}

