namespace UnityEditor.iOS.Xcode
{
    using System;

    public class PlistElementInteger : PlistElement
    {
        public int value;

        public PlistElementInteger(int v)
        {
            this.value = v;
        }
    }
}

