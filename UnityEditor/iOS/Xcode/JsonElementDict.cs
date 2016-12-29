namespace UnityEditor.iOS.Xcode
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class JsonElementDict : JsonElement
    {
        private SortedDictionary<string, JsonElement> m_PrivateValue = new SortedDictionary<string, JsonElement>();

        public bool Contains(string key) => 
            this.values.ContainsKey(key);

        public JsonElementArray CreateArray(string key)
        {
            JsonElementArray array = new JsonElementArray();
            this.values[key] = array;
            return array;
        }

        public JsonElementDict CreateDict(string key)
        {
            JsonElementDict dict = new JsonElementDict();
            this.values[key] = dict;
            return dict;
        }

        public void Remove(string key)
        {
            this.values.Remove(key);
        }

        public void SetBoolean(string key, bool val)
        {
            this.values[key] = new JsonElementBoolean(val);
        }

        public void SetInteger(string key, int val)
        {
            this.values[key] = new JsonElementInteger(val);
        }

        public void SetString(string key, string val)
        {
            this.values[key] = new JsonElementString(val);
        }

        public JsonElement this[string key]
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

        public IDictionary<string, JsonElement> values =>
            this.m_PrivateValue;
    }
}

