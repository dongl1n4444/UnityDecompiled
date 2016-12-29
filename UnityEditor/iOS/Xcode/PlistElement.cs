namespace UnityEditor.iOS.Xcode
{
    using System;
    using System.Reflection;

    public class PlistElement
    {
        protected PlistElement()
        {
        }

        public PlistElementArray AsArray() => 
            ((PlistElementArray) this);

        public bool AsBoolean() => 
            ((PlistElementBoolean) this).value;

        public PlistElementDict AsDict() => 
            ((PlistElementDict) this);

        public int AsInteger() => 
            ((PlistElementInteger) this).value;

        public string AsString() => 
            ((PlistElementString) this).value;

        public PlistElement this[string key]
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

