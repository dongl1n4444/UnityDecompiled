namespace UnityEditor
{
    using System;

    internal class SavedBool
    {
        private bool m_Loaded;
        private string m_Name;
        private bool m_Value;

        public SavedBool(string name, bool value)
        {
            this.m_Name = name;
            this.m_Loaded = false;
        }

        private void Load()
        {
            if (!this.m_Loaded)
            {
                this.m_Loaded = true;
                this.m_Value = EditorPrefs.GetBool(this.m_Name, this.value);
            }
        }

        public static implicit operator bool(SavedBool s) => 
            s.value;

        public bool value
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
                    EditorPrefs.SetBool(this.m_Name, value);
                }
            }
        }
    }
}

