namespace UnityEditor.iOS.Xcode
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class PlistElementDict : PlistElement
    {
        private SortedDictionary<string, PlistElement> m_PrivateValue = new SortedDictionary<string, PlistElement>();

        public PlistElementArray CreateArray(string key)
        {
            PlistElementArray array = new PlistElementArray();
            this.values[key] = array;
            return array;
        }

        public PlistElementDict CreateDict(string key)
        {
            PlistElementDict dict = new PlistElementDict();
            this.values[key] = dict;
            return dict;
        }

        public void SetBoolean(string key, bool val)
        {
            this.values[key] = new PlistElementBoolean(val);
        }

        public void SetInteger(string key, int val)
        {
            this.values[key] = new PlistElementInteger(val);
        }

        public void SetString(string key, string val)
        {
            this.values[key] = new PlistElementString(val);
        }

        public PlistElement this[string key]
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

        public IDictionary<string, PlistElement> values =>
            this.m_PrivateValue;
    }
}

