namespace UnityEditor.EditorTestsRunner
{
    using System;
    using UnityEditor;

    internal class EditorTestsRunnerSettings
    {
        public bool dontPromptForSaving;
        public bool horizontalSplit = true;
        private readonly string m_PrefsKey;
        public bool runOnRecompilation;

        public EditorTestsRunnerSettings(string prefsKey)
        {
            this.m_PrefsKey = prefsKey;
            this.runOnRecompilation = EditorPrefs.GetBool(this.m_PrefsKey + ".runOnRecompilation", false);
            this.horizontalSplit = EditorPrefs.GetBool(this.m_PrefsKey + ".horizontalSplit", true);
            this.dontPromptForSaving = EditorPrefs.GetBool(this.m_PrefsKey + ".dontPromptForSaving", false);
        }

        private void Save()
        {
            EditorPrefs.SetBool(this.m_PrefsKey + ".runOnRecompilation", this.runOnRecompilation);
            EditorPrefs.SetBool(this.m_PrefsKey + ".horizontalSplit", this.horizontalSplit);
            EditorPrefs.SetBool(this.m_PrefsKey + ".dontPromptForSaving", this.dontPromptForSaving);
        }

        public void ToggleDontPromptForSaving()
        {
            this.dontPromptForSaving = !this.dontPromptForSaving;
            this.Save();
        }

        public void ToggleHorizontalSplit()
        {
            this.horizontalSplit = !this.horizontalSplit;
            this.Save();
        }

        public void ToggleRunOnRecompilation()
        {
            this.runOnRecompilation = !this.runOnRecompilation;
            this.Save();
        }
    }
}

