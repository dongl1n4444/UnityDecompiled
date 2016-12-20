namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class PrefKey : IPrefType
    {
        private string m_DefaultShortcut;
        private Event m_event;
        private bool m_Loaded;
        private string m_name;
        private string m_Shortcut;

        public PrefKey()
        {
            this.m_Loaded = true;
        }

        public PrefKey(string name, string shortcut)
        {
            this.m_name = name;
            this.m_Shortcut = shortcut;
            this.m_DefaultShortcut = shortcut;
            Settings.Add(this);
            this.m_Loaded = false;
        }

        public void FromUniqueString(string s)
        {
            this.Load();
            int index = s.IndexOf(";");
            if (index < 0)
            {
                Debug.LogError("Malformed string in Keyboard preferences");
            }
            else
            {
                this.m_name = s.Substring(0, index);
                this.m_event = Event.KeyboardEvent(s.Substring(index + 1));
            }
        }

        public void Load()
        {
            if (!this.m_Loaded)
            {
                this.m_Loaded = true;
                this.m_event = Event.KeyboardEvent(this.m_Shortcut);
                PrefKey key = Settings.Get<PrefKey>(this.m_name, this);
                this.m_name = key.Name;
                this.m_event = key.KeyboardEvent;
            }
        }

        public static implicit operator Event(PrefKey pkey)
        {
            pkey.Load();
            return pkey.m_event;
        }

        internal void ResetToDefault()
        {
            this.Load();
            this.m_event = Event.KeyboardEvent(this.m_DefaultShortcut);
        }

        public string ToUniqueString()
        {
            this.Load();
            object[] objArray1 = new object[] { this.m_name, ";", !this.m_event.alt ? "" : "&", !this.m_event.command ? "" : "%", !this.m_event.shift ? "" : "#", !this.m_event.control ? "" : "^", this.m_event.keyCode };
            return string.Concat(objArray1);
        }

        public bool activated
        {
            get
            {
                this.Load();
                return (Event.current.Equals(this) && !GUIUtility.textFieldInput);
            }
        }

        public Event KeyboardEvent
        {
            get
            {
                this.Load();
                return this.m_event;
            }
            set
            {
                this.Load();
                this.m_event = value;
            }
        }

        public string Name
        {
            get
            {
                this.Load();
                return this.m_name;
            }
        }
    }
}

