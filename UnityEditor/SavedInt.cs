namespace UnityEditor
{
    using System;

    internal class SavedInt
    {
        private bool m_Loaded;
        private string m_Name;
        private int m_Value;

        public SavedInt(string name, int value)
        {
            this.m_Name = name;
            this.m_Loaded = false;
        }

        private void Load()
        {
            if (!this.m_Loaded)
            {
                this.m_Loaded = true;
                this.m_Value = EditorPrefs.GetInt(this.m_Name, this.value);
            }
        }

        public static implicit operator int(SavedInt s) => 
            s.value;

        public int value
        {
            get
            {
                this.Load();
                return this.m_Value;
            }
            set
            {
                this.Load();
                if (this.m_Value != value)
                {
                    this.m_Value = value;
                    EditorPrefs.SetInt(this.m_Name, value);
                }
            }
        }
    }
}

