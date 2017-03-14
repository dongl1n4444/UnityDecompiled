namespace UnityEditor.iOS.Xcode
{
    using System;

    internal class JsonElementInteger : JsonElement
    {
        public int value;

        public JsonElementInteger(int v)
        {
            this.value = v;
        }
    }
}

