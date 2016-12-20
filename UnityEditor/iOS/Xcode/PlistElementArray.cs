namespace UnityEditor.iOS.Xcode
{
    using System;
    using System.Collections.Generic;

    public class PlistElementArray : PlistElement
    {
        public List<PlistElement> values = new List<PlistElement>();

        public PlistElementArray AddArray()
        {
            PlistElementArray item = new PlistElementArray();
            this.values.Add(item);
            return item;
        }

        public void AddBoolean(bool val)
        {
            this.values.Add(new PlistElementBoolean(val));
        }

        public PlistElementDict AddDict()
        {
            PlistElementDict item = new PlistElementDict();
            this.values.Add(item);
            return item;
        }

        public void AddInteger(int val)
        {
            this.values.Add(new PlistElementInteger(val));
        }

        public void AddString(string val)
        {
            this.values.Add(new PlistElementString(val));
        }
    }
}

