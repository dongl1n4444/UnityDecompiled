namespace UnityEditor.iOS.Xcode.PBX
{
    using System;
    using System.Reflection;

    internal class PBXElement
    {
        protected PBXElement()
        {
        }

        public PBXElementArray AsArray()
        {
            return (PBXElementArray) this;
        }

        public PBXElementDict AsDict()
        {
            return (PBXElementDict) this;
        }

        public string AsString()
        {
            return ((PBXElementString) this).value;
        }

        public PBXElement this[string key]
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

