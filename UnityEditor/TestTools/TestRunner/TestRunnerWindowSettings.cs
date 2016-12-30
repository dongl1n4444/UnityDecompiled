namespace UnityEditor.TestTools.TestRunner
{
    using System;
    using UnityEditor;

    internal class TestRunnerWindowSettings
    {
        public bool blockUIWhenRunning = true;
        private readonly string m_PrefsKey;
        public bool pauseOnTestFailure;
        public bool verticalSplit;

        public TestRunnerWindowSettings(string prefsKey)
        {
            this.m_PrefsKey = prefsKey;
            this.blockUIWhenRunning = EditorPrefs.GetBool(this.m_PrefsKey + ".blockUIWhenRunning", false);
            this.pauseOnTestFailure = EditorPrefs.GetBool(this.m_PrefsKey + ".pauseOnTestFailure", false);
            this.verticalSplit = EditorPrefs.GetBool(this.m_PrefsKey + ".verticalSplit", true);
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

