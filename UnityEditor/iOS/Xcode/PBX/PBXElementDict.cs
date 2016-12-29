namespace UnityEditor.iOS.Xcode.PBX
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal class PBXElementDict : PBXElement
    {
        private Dictionary<string, PBXElement> m_PrivateValue = new Dictionary<string, PBXElement>();

        public bool Contains(string key) => 
            this.values.ContainsKey(key);

        public PBXElementArray CreateArray(string key)
        {
            PBXElementArray array = new PBXElementArray();
            this.values[key] = array;
            return array;
        }

        public PBXElementDict CreateDict(string key)
        {
            PBXElementDict dict = new PBXElementDict();
            this.values[key] = dict;
            return dict;
        }

        public void Remove(string key)
        {
            this.values.Remove(key);
        }

        public void SetString(string key, string val)
        {
            this.values[key] = new PBXElementString(val);
        }

        public PBXElement this[string key]
        {
            get
            {
                if (this.values.ContainsKey(key))
                {
                    return this.values[key];
                }
                return null;
            }
            set
            {
                this.values[key] = value;
            }
        }

        public IDictionary<string, PBXElement> values =>
            this.m_PrivateValue;
    }
}

