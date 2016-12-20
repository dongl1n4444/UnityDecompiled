namespace UnityEditor.iOS.Xcode
{
    using System;
    using System.Collections.Generic;

    public class JsonElementArray : JsonElement
    {
        public List<JsonElement> values = new List<JsonElement>();

        public JsonElementArray AddArray()
        {
            JsonElementArray item = new JsonElementArray();
            this.values.Add(item);
            return item;
        }

        public void AddBoolean(bool val)
        {
            this.values.Add(new JsonElementBoolean(val));
        }

        public JsonElementDict AddDict()
        {
            JsonElementDict item = new JsonElementDict();
            this.values.Add(item);
            return item;
        }

        public void AddInteger(int val)
        {
            this.values.Add(new JsonElementInteger(val));
        }

        public void AddString(string val)
        {
            this.values.Add(new JsonElementString(val));
        }
    }
}

