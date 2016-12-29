namespace UnityEditor.iOS.Xcode
{
    using System;
    using System.Reflection;

    public class JsonElement
    {
        protected JsonElement()
        {
        }

        public JsonElementArray AsArray() => 
            ((JsonElementArray) this);

        public bool AsBoolean() => 
            ((JsonElementBoolean) this).value;

        public JsonElementDict AsDict() => 
            ((JsonElementDict) this);

        public int AsInteger() => 
            ((JsonElementInteger) this).value;

        public string AsString() => 
            ((JsonElementString) this).value;

        public JsonElement this[string key]
        {
            get => 
                this.AsDict()[key];
            set
            {
                this.AsDict()[key] = value;
            }
        }
    }
}

