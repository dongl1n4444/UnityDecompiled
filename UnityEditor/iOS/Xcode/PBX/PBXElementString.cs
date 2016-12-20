namespace UnityEditor.iOS.Xcode.PBX
{
    using System;

    internal class PBXElementString : PBXElement
    {
        public string value;

        public PBXElementString(string v)
        {
            this.value = v;
        }
    }
}

