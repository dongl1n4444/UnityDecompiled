namespace UnityEditor.iOS.Xcode.PBX
{
    using System;
    using System.Collections.Generic;

    internal class PBXElementArray : PBXElement
    {
        public List<PBXElement> values = new List<PBXElement>();

        public PBXElementArray AddArray()
        {
            PBXElementArray item = new PBXElementArray();
            this.values.Add(item);
            return item;
        }

        public PBXElementDict AddDict()
        {
            PBXElementDict item = new PBXElementDict();
            this.values.Add(item);
            return item;
        }

        public void AddString(string val)
        {
            this.values.Add(new PBXElementString(val));
        }
    }
}

