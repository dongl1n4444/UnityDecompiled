namespace UnityEditor.iOS.Xcode.PBX
{
    using System;
    using System.Collections.Generic;

    internal class PBXObjectData
    {
        private static PropertyCommentChecker checkerData = new PropertyCommentChecker();
        public string guid;
        protected PBXElementDict m_Properties = new PBXElementDict();

        internal PBXElementDict GetPropertiesRaw()
        {
            this.UpdateProps();
            return this.m_Properties;
        }

        internal PBXElementDict GetPropertiesWhenSerializing()
        {
            return this.m_Properties;
        }

        protected List<string> GetPropertyList(string name)
        {
            PBXElement element = this.m_Properties[name];
            if (element == null)
            {
                return null;
            }
            List<string> list2 = new List<string>();
            foreach (PBXElement element2 in element.AsArray().values)
            {
                list2.Add(element2.AsString());
            }
            return list2;
        }

        protected string GetPropertyString(string name)
        {
            PBXElement element = this.m_Properties[name];
            if (element == null)
            {
                return null;
            }
            return element.AsString();
        }

        internal void SetPropertiesWhenSerializing(PBXElementDict props)
        {
            this.m_Properties = props;
        }

        protected void SetPropertyList(string name, List<string> value)
        {
            if (value == null)
            {
                this.m_Properties.Remove(name);
            }
            else
            {
                PBXElementArray array = this.m_Properties.CreateArray(name);
                foreach (string str in value)
                {
                    array.AddString(str);
                }
            }
        }

        protected void SetPropertyString(string name, string value)
        {
            if (value == null)
            {
                this.m_Properties.Remove(name);
            }
            else
            {
                this.m_Properties.SetString(name, value);
            }
        }

        public virtual void UpdateProps()
        {
        }

        public virtual void UpdateVars()
        {
        }

        internal virtual PropertyCommentChecker checker
        {
            get
            {
                return checkerData;
            }
        }

        internal virtual bool shouldCompact
        {
            get
            {
                return false;
            }
        }
    }
}

