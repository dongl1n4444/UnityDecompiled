namespace UnityEditor.iOS.Xcode.PBX
{
    using System;
    using System.Reflection;

    internal class PBXElement
    {
        protected PBXElement()
        {
        }

        public PBXElementArray AsArray() => 
            ((PBXElementArray) this);

        public PBXElementDict AsDict() => 
            ((PBXElementDict) this);

        public string AsString() => 
            ((PBXElementString) this).value;

        public PBXElement this[string key]
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

