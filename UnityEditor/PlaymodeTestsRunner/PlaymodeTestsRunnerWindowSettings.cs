namespace UnityEditor.PlaymodeTestsRunner
{
    using System;
    using UnityEditor;

    internal class PlaymodeTestsRunnerWindowSettings
    {
        public bool blockUIWhenRunning = true;
        private readonly string m_PrefsKey;
        public bool pauseOnTestFailure;
        public bool verticalSplit;

        public PlaymodeTestsRunnerWindowSettings(string prefsKey)
        {
            this.m_PrefsKey = prefsKey;
            this.blockUIWhenRunning = EditorPrefs.GetBool(this.m_PrefsKey + ".blockUIWhenRunning", false);
            this.pauseOnTestFailure = EditorPrefs.GetBool(this.m_PrefsKey + ".pauseOnTestFailure", false);
            this.verticalSplit = EditorPrefs.GetBool(this.m_PrefsKey + ".verticalSplit", false);
        }

        private void Save()
        {
            EditorPrefs.SetBool(this.m_PrefsKey + ".blockUIWhenRunning", this.blockUIWhenRunning);
            EditorPrefs.SetBool(this.m_PrefsKey + ".pauseOnTestFailure", this.pauseOnTestFailure);
            EditorPrefs.SetBool(this.m_PrefsKey + ".verticalSplit", this.verticalSplit);
        }

        public void ToggleBlockUIWhenRunning()
        {
            this.blockUIWhenRunning = !this.blockUIWhenRunning;
            this.Save();
        }

        public void TogglePauseOnTestFailure()
        {
            this.pauseOnTestFailure = !this.pauseOnTestFailure;
            this.Save();
        }

        public void ToggleVerticalSplit()
        {
            this.verticalSplit = !this.verticalSplit;
            this.Save();
        }
    }
}

