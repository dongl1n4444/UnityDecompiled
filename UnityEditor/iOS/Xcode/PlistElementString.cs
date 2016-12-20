namespace UnityEditor.iOS.Xcode
{
    using System;

    public class PlistElementString : PlistElement
    {
        public string value;

        public PlistElementString(string v)
        {
            this.value = v;
        }
    }
}

