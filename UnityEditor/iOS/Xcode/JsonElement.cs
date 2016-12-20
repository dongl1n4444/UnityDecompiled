namespace UnityEditor.iOS.Xcode
{
    using System;
    using System.Reflection;

    public class JsonElement
    {
        protected JsonElement()
        {
        }

        public JsonElementArray AsArray()
        {
            return (JsonElementArray) this;
        }

        public bool AsBoolean()
        {
            return ((JsonElementBoolean) this).value;
        }

        public JsonElementDict AsDict()
        {
            return (JsonElementDict) this;
        }

        public int AsInteger()
        {
            return ((JsonElementInteger) this).value;
        }

        public string AsString()
        {
            return ((JsonElementString) this).value;
        }

        public JsonElement this[string key]
        {
            get
            {
                return this.AsDict()[key];
            }
            set
            {
                this.AsDict()[key] = value;
            }
        }
    }
}

