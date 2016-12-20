namespace UnityEditor.iOS.Xcode
{
    using System;

    public class JsonElementInteger : JsonElement
    {
        public int value;

        public JsonElementInteger(int v)
        {
            this.value = v;
        }
    }
}

