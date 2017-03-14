namespace UnityEditor.TestTools.TestRunner
{
    using System;
    using UnityEditor;

    internal class TestRunnerWindowSettings
    {
        private readonly string m_PrefsKey;
        public bool verticalSplit;

        public TestRunnerWindowSettings(string prefsKey)
        {
            this.m_PrefsKey = prefsKey;
            this.verticalSplit = EditorPrefs.GetBool(this.m_PrefsKey + ".verticalSplit", true);
        }

        private void Save()
        {
            EditorPrefs.SetBool(this.m_PrefsKey + ".verticalSplit", this.verticalSplit);
        }

        public void ToggleVerticalSplit()
        {
            this.verticalSplit = !this.verticalSplit;
            this.Save();
        }
    }
}

