namespace UnityEditor.iOS.Xcode
{
    using System;
    using System.Reflection;

    public class PlistElement
    {
        protected PlistElement()
        {
        }

        public PlistElementArray AsArray()
        {
            return (PlistElementArray) this;
        }

        public bool AsBoolean()
        {
            return ((PlistElementBoolean) this).value;
        }

        public PlistElementDict AsDict()
        {
            return (PlistElementDict) this;
        }

        public int AsInteger()
        {
            return ((PlistElementInteger) this).value;
        }

        public string AsString()
        {
            return ((PlistElementString) this).value;
        }

        public PlistElement this[string key]
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

