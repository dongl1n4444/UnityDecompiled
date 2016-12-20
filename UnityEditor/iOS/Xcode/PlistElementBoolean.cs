namespace UnityEditor.iOS.Xcode
{
    using System;

    public class PlistElementBoolean : PlistElement
    {
        public bool value;

        public PlistElementBoolean(bool v)
        {
            this.value = v;
        }
    }
}

